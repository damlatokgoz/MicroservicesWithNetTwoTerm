using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Stock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {

        [HttpGet("{productId:int}/{quantity:int}")]
        public async Task<IActionResult> CheckStock(int productId, int quantity)
        {
            return Ok(new { Status = true });
        }
    }
}
