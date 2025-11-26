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

                // First, get the grouped data with Month as int
                var monthlySalesData = await query
                    .GroupBy(s => new { 
                        Year = s.InvoiceDate.Year, 
                        Month = s.InvoiceDate.Month 
                    })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,  // Keep as int for now
                        Revenue = g.Sum(s => s.TotalAmount),
                        TransactionCount = g.Count(),
                        AverageOrderValue = g.Average(s => s.TotalAmount)
                    })
                    .OrderBy(m => m.Year)
                    .ThenBy(m => m.Month)
                    .ToListAsync();

                // Then, convert to DTO with Month as formatted string in C# code
                var monthlySales = monthlySalesData.Select(m => new MonthlySalesDTO
                {
                    Year = m.Year,
                    Month = m.Month.ToString("D2"), // Format here in C# code!
                    Revenue = m.Revenue,
                    TransactionCount = m.TransactionCount,
                    AverageOrderValue = m.AverageOrderValue
                }).ToList();

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

                // Get all sales data first
                var allSales = await query
                    .Select(s => new 
                    { 
                        s.InvoiceDate,
                        s.TotalAmount 
                    })
                    .ToListAsync();

                // Group by day of week in C# code (client-side)
                var salesByDay = allSales
                    .GroupBy(s => s.InvoiceDate.DayOfWeek)
                    .Select(g => new
                    {
                        DayOfWeek = g.Key.ToString(),
                        Revenue = g.Sum(s => s.TotalAmount),
                        TransactionCount = g.Count(),
                        AverageOrderValue = g.Average(s => s.TotalAmount)
                    })
                    .ToList();

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

        /// <summary>
        /// Compare two consecutive months from the dataset with growth metrics
        /// </summary>
        [HttpGet("comparison")]
        public async Task<ActionResult<SalesComparisonDTO>> GetComparison(
        [FromQuery] int? year = null,
        [FromQuery] int? month = null)
        {
        try
        {
            // If no year/month provided, use the latest month in data
            DateTime currentMonthStart;
            
            if (year.HasValue && month.HasValue)
            {
                currentMonthStart = new DateTime(year.Value, month.Value, 1);
            }
            else
            {
                // Get the latest date in dataset
                var latestDate = await _context.Sales.MaxAsync(s => s.InvoiceDate);
                currentMonthStart = new DateTime(latestDate.Year, latestDate.Month, 1);
            }
            
            var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);
            var previousMonthStart = currentMonthStart.AddMonths(-1);
            var previousMonthEnd = currentMonthStart.AddDays(-1);

            // Current month data
            var currentData = await GetPeriodData(currentMonthStart, currentMonthEnd);
            
            // Previous month data
            var previousData = await GetPeriodData(previousMonthStart, previousMonthEnd);

            // Calculate growth
            var growth = new GrowthMetricsDTO
            {
                RevenueGrowth = CalculateGrowth(previousData.Revenue, currentData.Revenue),
                TransactionGrowth = CalculateGrowth(previousData.TransactionCount, currentData.TransactionCount),
                AverageOrderGrowth = CalculateGrowth(previousData.AverageOrderValue, currentData.AverageOrderValue),
                CustomerGrowth = CalculateGrowth(previousData.UniqueCustomers, currentData.UniqueCustomers),
                Trend = currentData.Revenue > previousData.Revenue ? "up" : 
                        currentData.Revenue < previousData.Revenue ? "down" : "neutral"
            };

            return Ok(new SalesComparisonDTO
            {
                CurrentPeriod = currentData,
                PreviousPeriod = previousData,
                Growth = growth
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving comparison data", error = ex.Message });
        }
        }

        /// <summary>
        /// Get sales pattern by hour of day
        /// </summary>
        [HttpGet("by-hour")]
        public async Task<ActionResult<List<HourlySalesDTO>>> GetSalesByHour(
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

                var hourlySales = await query
                    .GroupBy(s => s.InvoiceDate.Hour)
                    .Select(g => new HourlySalesDTO
                    {
                        Hour = g.Key,
                        Revenue = g.Sum(s => s.TotalAmount),
                        TransactionCount = g.Count(),
                        AverageOrderValue = g.Average(s => s.TotalAmount)
                    })
                    .OrderBy(h => h.Hour)
                    .ToListAsync();

                return Ok(hourlySales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving hourly sales", error = ex.Message });
            }
        }

        /// <summary>
        /// Get most recent transactions
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<List<RecentSaleDTO>>> GetRecentSales(
            [FromQuery] int limit = 10)
        {
            try
            {
                var recentSales = await _context.Sales
                    .Include(s => s.Product)
                    .Include(s => s.Region)
                    .OrderByDescending(s => s.InvoiceDate)
                    .Take(limit)
                    .Select(s => new RecentSaleDTO
                    {
                        InvoiceNo = s.InvoiceNo,
                        InvoiceDate = s.InvoiceDate,
                        ProductDescription = s.Product!.Description ?? "Unknown",
                        Country = s.Region!.Country,
                        Quantity = s.Quantity,
                        TotalAmount = s.TotalAmount
                    })
                    .ToListAsync();

                return Ok(recentSales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving recent sales", error = ex.Message });
            }
        }

        // Helper method for comparison
        private async Task<PeriodDataDTO> GetPeriodData(DateTime startDate, DateTime endDate)
        {
            var query = _context.Sales
                .Where(s => s.InvoiceDate >= startDate && s.InvoiceDate <= endDate);

            return new PeriodDataDTO
            {
                Revenue = await query.SumAsync(s => s.TotalAmount),
                TransactionCount = await query.CountAsync(),
                AverageOrderValue = await query.AverageAsync(s => s.TotalAmount),
                UniqueCustomers = await query.Select(s => s.CustomerId).Distinct().CountAsync(),
                StartDate = startDate,
                EndDate = endDate
            };
        }

        // Helper method to calculate growth percentage
        private decimal CalculateGrowth(decimal oldValue, decimal newValue)
        {
            if (oldValue == 0) return 0;
            return Math.Round(((newValue - oldValue) / oldValue) * 100, 2);
        }

        private decimal CalculateGrowth(int oldValue, int newValue)
        {
            if (oldValue == 0) return 0;
            return Math.Round(((decimal)(newValue - oldValue) / oldValue) * 100, 2);
        }
    }
}