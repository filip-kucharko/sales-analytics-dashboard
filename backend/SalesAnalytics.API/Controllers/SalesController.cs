using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesAnalytics.API.Data;
using SalesAnalytics.API.DTOs;

namespace SalesAnalytics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SalesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get overall sales summary with KPIs
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<SalesSummaryDTO>> GetSummary(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                // Default to all data if no dates provided
                var query = _context.Sales.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(s => s.InvoiceDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(s => s.InvoiceDate <= endDate.Value);

                var summary = new SalesSummaryDTO
                {
                    TotalRevenue = await query.SumAsync(s => s.TotalAmount),
                    TotalTransactions = await query.CountAsync(),
                    AverageOrderValue = await query.AverageAsync(s => s.TotalAmount),
                    UniqueCustomers = await query.Select(s => s.CustomerId).Distinct().CountAsync(),
                    UniqueProducts = await query.Select(s => s.ProductId).Distinct().CountAsync(),
                    StartDate = await query.MinAsync(s => s.InvoiceDate),
                    EndDate = await query.MaxAsync(s => s.InvoiceDate)
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving sales summary", error = ex.Message });
            }
        }

        /// <summary>
        /// Get monthly sales trend
        /// </summary>
        [HttpGet("by-month")]
        public async Task<ActionResult<List<MonthlySalesDTO>>> GetSalesByMonth(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var query = _context.Sales.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(s => s.InvoiceDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(s => s.InvoiceDate <= endDate.Value);

                var monthlySales = await query
                    .GroupBy(s => new { 
                        Year = s.InvoiceDate.Year, 
                        Month = s.InvoiceDate.Month 
                    })
                    .Select(g => new MonthlySalesDTO
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month.ToString("D2"), // Format: 01, 02, etc.
                        Revenue = g.Sum(s => s.TotalAmount),
                        TransactionCount = g.Count(),
                        AverageOrderValue = g.Average(s => s.TotalAmount)
                    })
                    .OrderBy(m => m.Year)
                    .ThenBy(m => m.Month)
                    .ToListAsync();

                return Ok(monthlySales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving monthly sales", error = ex.Message });
            }
        }

        /// <summary>
        /// Get sales breakdown by region/country
        /// </summary>
        [HttpGet("by-region")]
        public async Task<ActionResult<List<RegionalSalesDTO>>> GetSalesByRegion(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int top = 10)
        {
            try
            {
                var query = _context.Sales
                    .Include(s => s.Region)
                    .AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(s => s.InvoiceDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(s => s.InvoiceDate <= endDate.Value);

                // Calculate total revenue for percentage
                var totalRevenue = await query.SumAsync(s => s.TotalAmount);

                var regionalSales = await query
                    .GroupBy(s => s.Region!.Country)
                    .Select(g => new RegionalSalesDTO
                    {
                        Country = g.Key,
                        Revenue = g.Sum(s => s.TotalAmount),
                        TransactionCount = g.Count(),
                        Percentage = 0 // Will calculate after
                    })
                    .OrderByDescending(r => r.Revenue)
                    .Take(top)
                    .ToListAsync();

                // Calculate percentages
                foreach (var region in regionalSales)
                {
                    region.Percentage = totalRevenue > 0 
                        ? Math.Round((region.Revenue / totalRevenue) * 100, 2) 
                        : 0;
                }

                return Ok(regionalSales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving regional sales", error = ex.Message });
            }
        }

        /// <summary>
        /// Get sales by day of week
        /// </summary>
        [HttpGet("by-day-of-week")]
        public async Task<ActionResult> GetSalesByDayOfWeek(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var query = _context.Sales.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(s => s.InvoiceDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(s => s.InvoiceDate <= endDate.Value);

                var salesByDay = await query
                    .GroupBy(s => s.InvoiceDate.DayOfWeek)
                    .Select(g => new
                    {
                        DayOfWeek = g.Key.ToString(),
                        Revenue = g.Sum(s => s.TotalAmount),
                        TransactionCount = g.Count(),
                        AverageOrderValue = g.Average(s => s.TotalAmount)
                    })
                    .ToListAsync();

                // Order by day of week (Monday first)
                var orderedDays = new[] { 
                    DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, 
                    DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday 
                };

                var result = orderedDays
                    .Select(day => salesByDay.FirstOrDefault(s => s.DayOfWeek == day.ToString()))
                    .Where(s => s != null)
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving sales by day", error = ex.Message });
            }
        }
    }
}