using Microsoft.AspNetCore.Mvc;

namespace BLeaf.Controllers
{
    public class OrderViewController : Controller
    {
        [HttpGet]
        public IActionResult OrderSuccess(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View("~/Views/BLeaf/OrderSuccess.cshtml");
        }
    }
}