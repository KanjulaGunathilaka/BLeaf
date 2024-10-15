using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using BLeaf.Models.IRepository;
using BLeaf.Models;
using BLeaf.ViewModels;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Layout.Properties;

namespace BLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] OrderViewModel orderViewModel)
        {
            if (orderViewModel == null || !orderViewModel.Items.Any())
            {
                return BadRequest("Invalid order data.");
            }

            var order = new Order
            {
                User = orderViewModel.User,
                Address = orderViewModel.Address,
                OrderTotal = orderViewModel.OrderTotal,
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid",
                OrderPlacedAt = DateTime.Now,
                OrderDetails = orderViewModel.Items.Select(i => new OrderDetail
                {
                    ItemId = i.Item.ItemId,
                    Quantity = i.Quantity,
                    UnitPrice = i.Item.Price
                }).ToList()
            };

            await _orderRepository.AddAsync(order);

            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest("Order ID mismatch.");
            }

            await _orderRepository.UpdateAsync(order);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderRepository.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("DownloadReceipt")]
        public async Task<ActionResult> DownloadReceipt(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            using (MemoryStream stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Set up fonts
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // Add title
                Paragraph title = new Paragraph("BLeaf Order Receipt")
                    .SetFont(boldFont)
                    .SetFontSize(20)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20);
                document.Add(title);

                // Add order details
                document.Add(new Paragraph($"Order Number: {order.OrderId}")
                    .SetFont(boldFont)
                    .SetFontSize(12)
                    .SetMarginBottom(10));
                document.Add(new Paragraph($"Order Date: {order.OrderPlacedAt}")
                    .SetFont(font)
                    .SetFontSize(12)
                    .SetMarginBottom(10));
                document.Add(new Paragraph($"Customer Name: {order.User.FullName}")
                    .SetFont(font)
                    .SetFontSize(12)
                    .SetMarginBottom(10));
                document.Add(new Paragraph($"Total Amount: ${order.OrderTotal}")
                    .SetFont(font)
                    .SetFontSize(12)
                    .SetMarginBottom(20));

                // Add table for order items
                Table table = new Table(new float[] { 1, 3, 1, 1, 1 });
                table.SetWidth(UnitValue.CreatePercentValue(100));

                // Add table headers
                table.AddHeaderCell(new Cell().Add(new Paragraph("Item ID").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Item Name").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Quantity").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Unit Price").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Total").SetFont(boldFont)));

                // Add table rows
                foreach (var detail in order.OrderDetails)
                {
                    table.AddCell(new Cell().Add(new Paragraph(detail.ItemId.ToString()).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(detail.Item.Name).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(detail.Quantity.ToString()).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph($"${detail.UnitPrice}").SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph($"${detail.Quantity * detail.UnitPrice}").SetFont(font)));
                }

                document.Add(table);

                // Add footer
                Paragraph footer = new Paragraph("Thank you for your purchase!")
                    .SetFont(boldFont)
                    .SetFontSize(12)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(20);
                document.Add(footer);

                document.Close();

                // Generate filename with order ID and timestamp
                string filename = $"OrderReceipt_{order.OrderId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                byte[] fileBytes = stream.ToArray();
                return File(fileBytes, "application/pdf", filename);
            }
        }
    }
}