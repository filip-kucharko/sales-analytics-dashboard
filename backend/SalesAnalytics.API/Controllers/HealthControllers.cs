using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesAnalytics.API.Data;

namespace SalesAnalytics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HealthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/health
        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.Now,
                message = "API is running!"
            });
        }

        // GET: api/health/db
        [HttpGet("database")]
        public async Task<IActionResult> GetDatabaseHealth()
        {
            try
            {
                // Test database connection
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    return StatusCode(503, new
                    {
                        status = "unhealthy",
                        message = "Cannot connect to database"
                    });
                }

                // Get record counts
                var salesCount = await _context.Sales.CountAsync();
                var productsCount = await _context.Products.CountAsync();
                var customersCount = await _context.Customers.CountAsync();
                var regionsCount = await _context.Regions.CountAsync();

                return Ok(new
                {
                    status = "healthy",
                    database = "connected",
                    timestamp = DateTime.Now,
                    records = new
                    {
                        sales = salesCount,
                        products = productsCount,
                        customers = customersCount,
                        regions = regionsCount
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "unhealthy",
                    message = "Database error",
                    error = ex.Message
                });
            }
        }
    }
}