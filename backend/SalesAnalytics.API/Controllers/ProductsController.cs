using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesAnalytics.API.Data;
using SalesAnalytics.API.DTOs;

namespace SalesAnalytics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get top products by revenue
        /// </summary>
        [HttpGet("top")]
        public async Task<ActionResult<List<TopProductDTO>>> GetTopProducts(
            [FromQuery] int top = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var query = _context.Sales
                    .Include(s => s.Product)
                    .AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(s => s.InvoiceDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(s => s.InvoiceDate <= endDate.Value);

                var topProducts = await query
                    .GroupBy(s => new { s.Product!.StockCode, s.Product.Description })
                    .Select(g => new TopProductDTO
                    {
                        StockCode = g.Key.StockCode,
                        Description = g.Key.Description ?? "Unknown",
                        Revenue = g.Sum(s => s.TotalAmount),
                        UnitsSold = g.Sum(s => s.Quantity),
                        TransactionCount = g.Count()
                    })
                    .OrderByDescending(p => p.Revenue)
                    .Take(top)
                    .ToListAsync();

                return Ok(topProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving top products", error = ex.Message });
            }
        }

        /// <summary>
        /// Get total product count
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult> GetProductCount()
        {
            try
            {
                var count = await _context.Products.CountAsync();
                return Ok(new { totalProducts = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving product count", error = ex.Message });
            }
        }
    }
}