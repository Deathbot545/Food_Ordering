using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using ZXing.QrCode.Internal;
using Core.Services.OutletSer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Core.DTO;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Restaurant_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowMyOrigins")]
    public class OutletApiController : ControllerBase
    {
        private readonly IOutletService _outletService;

        public OutletApiController(IOutletService outletService)
        {
            _outletService = outletService;
        }

        [HttpGet("GetOutletsByOwner/{ownerId}")]
        public async Task<IActionResult> GetOutletsByOwner(Guid ownerId)
        {
            try
            {
                var outlets = await _outletService.GetOutletsByOwner(ownerId);
                return Ok(outlets);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost("RegisterOutlet")]
        public async Task<IActionResult> RegisterOutlet([FromBody] Outlet model, [FromQuery] string currentUserId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var registeredOutlet = await _outletService.RegisterOutletAsync(model, currentUserId);
                return Ok(new { Success = true, Outlet = registeredOutlet });
            }
            catch (ArgumentNullException)
            {
                return BadRequest(new { Success = false, Message = "Invalid outlet data" });
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
        [HttpGet("GetOutletWithQRCode/{id}")]
        public async Task<IActionResult> GetOutletWithQRCode(int id)
        {
            var outlet = await _outletService.GetOutletWithQRCodeAsync(id);
            if (outlet == null)
            {
                return NotFound();
            }

            // Convert to DTO
            OutletDtoAndConverter.OutletDto outletDto = OutletDtoAndConverter.ConvertToDto(outlet);

            // Serialize to JSON
            var jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            string json = JsonSerializer.Serialize(outletDto, jsonOptions);

            return Content(json, "application/json");
        }


    }
}
