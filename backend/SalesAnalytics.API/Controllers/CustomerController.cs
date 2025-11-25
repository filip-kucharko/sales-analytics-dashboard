using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesAnalytics.API.Data;
using SalesAnalytics.API.DTOs;

namespace SalesAnalytics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get top customers by revenue
        /// </summary>
        [HttpGet("top")]
        public async Task<ActionResult<List<TopCustomerDTO>>> GetTopCustomers(
            [FromQuery] int top = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var query = _context.Sales
                    .Include(s => s.Customer)
                    .AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(s => s.InvoiceDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(s => s.InvoiceDate <= endDate.Value);

                var topCustomers = await query
                    .GroupBy(s => s.Customer!.CustomerCode)
                    .Select(g => new TopCustomerDTO
                    {
                        CustomerCode = g.Key,
                        TotalRevenue = g.Sum(s => s.TotalAmount),
                        OrderCount = g.Count(),
                        AverageOrderValue = g.Average(s => s.TotalAmount),
                        FirstPurchase = g.Min(s => s.InvoiceDate),
                        LastPurchase = g.Max(s => s.InvoiceDate)
                    })
                    .OrderByDescending(c => c.TotalRevenue)
                    .Take(top)
                    .ToListAsync();

                return Ok(topCustomers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving top customers", error = ex.Message });
            }
        }

        /// <summary>
        /// Get customer analytics summary
        /// </summary>
        [HttpGet("analytics")]
        public async Task<ActionResult<CustomerAnalyticsDTO>> GetCustomerAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var query = _context.Sales
                    .Include(s => s.Customer)
                    .AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(s => s.InvoiceDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(s => s.InvoiceDate <= endDate.Value);

                var totalCustomers = await query
                    .Select(s => s.CustomerId)
                    .Distinct()
                    .CountAsync();

                var totalRevenue = await query.SumAsync(s => s.TotalAmount);
                var totalOrders = await query.CountAsync();

                var topCustomers = await query
                    .GroupBy(s => s.Customer!.CustomerCode)
                    .Select(g => new TopCustomerDTO
                    {
                        CustomerCode = g.Key,
                        TotalRevenue = g.Sum(s => s.TotalAmount),
                        OrderCount = g.Count(),
                        AverageOrderValue = g.Average(s => s.TotalAmount),
                        FirstPurchase = g.Min(s => s.InvoiceDate),
                        LastPurchase = g.Max(s => s.InvoiceDate)
                    })
                    .OrderByDescending(c => c.TotalRevenue)
                    .Take(10)
                    .ToListAsync();

                return Ok(new CustomerAnalyticsDTO
                {
                    TotalCustomers = totalCustomers,
                    AverageCustomerValue = totalCustomers > 0 ? totalRevenue / totalCustomers : 0,
                    AverageOrdersPerCustomer = totalCustomers > 0 ? (decimal)totalOrders / totalCustomers : 0,
                    TopCustomers = topCustomers
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving customer analytics", error = ex.Message });
            }
        }

        /// <summary>
        /// Get total customer count
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult> GetCustomerCount()
        {
            try
            {
                var count = await _context.Customers.CountAsync();
                return Ok(new { totalCustomers = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving customer count", error = ex.Message });
            }
        }
    }
}