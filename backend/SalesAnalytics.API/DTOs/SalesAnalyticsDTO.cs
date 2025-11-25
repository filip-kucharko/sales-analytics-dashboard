namespace SalesAnalytics.API.DTOs
{
    // Summary KPIs
    public class SalesSummaryDTO
    {
        public decimal TotalRevenue { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int UniqueCustomers { get; set; }
        public int UniqueProducts { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    // Monthly trend data
    public class MonthlySalesDTO
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    // Regional breakdown
    public class RegionalSalesDTO
    {
        public string Country { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int TransactionCount { get; set; }
        public decimal Percentage { get; set; }
    }

    // Top products
    public class TopProductDTO
    {
        public string StockCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int UnitsSold { get; set; }
        public int TransactionCount { get; set; }
    }

    // Date range filter
    public class DateRangeDTO
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}