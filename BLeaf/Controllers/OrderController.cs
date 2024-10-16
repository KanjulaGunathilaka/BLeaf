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
using BLeaf.Services;

namespace BLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailSender _emailSender;

        public OrderController(IOrderRepository orderRepository, IEmailSender emailSender)
        {
            _orderRepository = orderRepository;
            _emailSender = emailSender;
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

            var orderDetails = new List<OrderDetail>();
            foreach (var item in orderViewModel.Items)
            {
                var itemEntity = await _orderRepository.GetItemByIdAsync(item.Item.ItemId);
                if (itemEntity == null)
                {
                    return BadRequest($"Item with ID {item.Item.ItemId} not found.");
                }

                orderDetails.Add(new OrderDetail
                {
                    ItemId = item.Item.ItemId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Item.Price,
                    Item = itemEntity // Ensure the Item property is set
                });
            }

            var order = new Order
            {
                User = orderViewModel.User,
                Address = orderViewModel.Address,
                OrderTotal = orderViewModel.OrderTotal,
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid",
                OrderPlacedAt = DateTime.Now,
                OrderDetails = orderDetails
            };

            await _orderRepository.AddAsync(order);

            // Send email notifications
            try
            {
                await SendOrderConfirmationEmailToUser(order);
                await SendNewOrderNotificationEmailToRestaurant(order);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }

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

        private async Task SendOrderConfirmationEmailToUser(Order order)
        {
            var subject = "BLeaf Order Confirmation";
            var plainTextContent = $"Dear {order.User.FullName},\n\nThank you for placing an order with our restaurant. Your order number is {order.OrderId}.\n\nBest Regards,\nYour Restaurant Name";
            var htmlContent = $@"
                <html>
                <body>
                    <h1>Order Confirmation</h1>
                    <p>Dear {order.User.FullName},</p>
                    <p>Thank you for placing an order with our restaurant. Your order number is <strong>{order.OrderId}</strong>.</p>
                    <p><strong>Order Details:</strong></p>
                    <ul>
                        {string.Join("", order.OrderDetails.Select(d => $"<li>{d.Quantity} x {d.Item.Name} - ${d.UnitPrice * d.Quantity}</li>"))}
                    </ul>
                    <p><strong>Total Amount:</strong> ${order.OrderTotal}</p>
                    <p><strong>Total Order placed at:</strong> ${order.OrderPlacedAt}</p>
                    <p>We will notify you once your order is ready for pickup.</p>
                    <p>Best Regards,<br>BLeaf Cafe</p>
                </body>
                </html>";

            await _emailSender.SendEmailAsync(order.User.Email, subject, plainTextContent, htmlContent);
        }

        private async Task SendNewOrderNotificationEmailToRestaurant(Order order)
        {
            var restaurantEmail = "bleafcaf@gmail.com";
            var subject = "New Online Order Placed!";
            var plainTextContent = $"A new order has been placed by {order.User.FullName}. Order number: {order.OrderId}.";
            var htmlContent = $@"
                <html>
                <body>
                    <h1>New Order Notification</h1>
                    <p>A new order has been placed by <strong>{order.User.FullName}</strong>.</p>
                    <p><strong>Order Number:</strong> {order.OrderId}</p>
                    <p><strong>Order Details:</strong></p>
                    <ul>
                        {string.Join("", order.OrderDetails.Select(d => $"<li>{d.Quantity} x {d.Item.Name} - ${d.UnitPrice * d.Quantity}</li>"))}
                    </ul>
                    <p><strong>Total Amount:</strong> ${order.OrderTotal}</p>
                    <p><strong>Total Order placed at:</strong> ${order.OrderPlacedAt}</p>
                    <p>Please prepare the order for pickup.</p>
                </body>
                </html>";

            await _emailSender.SendEmailAsync(restaurantEmail, subject, plainTextContent, htmlContent);
        }
    }
}