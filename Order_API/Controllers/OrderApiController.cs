using Core.DTO;
using Core.Services.CartSter;
using Core.Services.MenuS;
using Infrastructure.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Order_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowMyOrigins")]
    public class OrderApiController : Controller
    {
       
        private readonly ICartService _cartService;
        private readonly IMenuService _menuService;
        public OrderApiController(ICartService cartService, IMenuService menuService) {
            _cartService = cartService;
            _menuService = menuService;
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


    }
}
