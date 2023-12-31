using DeliveryServiceApi.Data;
using DeliveryServiceApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ApplicationDbContext _dbContext;

        public DeliveryController(IOrderService orderService, ApplicationDbContext dbContext)
        {
            _orderService = orderService;
            _dbContext = dbContext;
        }

        [HttpGet("checkStatus")]
        public IActionResult CheckStatus()
        {
            return Ok("Active");
        }

        [HttpPost("sendOrder")]
        public IActionResult SendOrder()
        {
            try
            {
                if (_orderService.IsFreeCourierAvailable())
                {
                    return Ok("Order has been sent");
                }

                return NotFound("Available courier not found");
            }
            catch
            {
                return BadRequest("The order is denied");
            }
        }

        [HttpGet("ordersCount")]
        public IActionResult GetOrdersCount()
        {
            int ordersCount = _dbContext.Orders.Count();
            return Ok(ordersCount);
        }
    }
}
