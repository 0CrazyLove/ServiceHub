using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        // Dummy data. In a real app, query the database.
        var stats = new
        {
            TotalSales = 12500.50m,
            ProductCount = 10,
            OrderCount = 50
        };
        return Ok(stats);
    }
}
