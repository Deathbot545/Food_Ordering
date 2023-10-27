using Core.DTO;
using Core.Services.CartSter;
using Core.Services.MenuS;
using Core.Services.Orderser;
using Infrastructure.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Order_API.Hubs;

namespace Order_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowMyOrigins")]
    public class OrderApiController : Controller
    {
       
        private readonly ICartService _cartService;
        private readonly IHubContext<OrderStatusHub> _hubContext;
        private readonly IOrderService _orderService;

        public OrderApiController(IOrderService orderService, ICartService cartService, IHubContext<OrderStatusHub> hubContext)
        {
            _orderService = orderService;
            _cartService = cartService;
            _hubContext = hubContext;
        }

        // Add item to cart
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] CartRequest request)
        {
            try
            {
                await _cartService.ProcessCartRequestAsync(request);
                return Ok(new { message = "Items added to cart successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(Order order)
        {
            // Use your service to update the order status in the database.
            await _orderService.UpdateOrderStatusAsync(order);

            // Once the order is updated in the database, notify all clients.
            await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", order);

            return Ok();
        }

        [HttpGet("GetOrdersForOutlet/{outletId}")]
        public async Task<IActionResult> GetOrdersForOutlet(int outletId)
        {
            try
            {
                var ordersDTO = _orderService.GetOrdersByOutletId(outletId);
                return Ok(ordersDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
