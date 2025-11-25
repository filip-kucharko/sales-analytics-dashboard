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

    // Growth Comparison
    public class SalesComparisonDTO
    {
        public PeriodDataDTO CurrentPeriod { get; set; } = new();
        public PeriodDataDTO PreviousPeriod { get; set; } = new();
        public GrowthMetricsDTO Growth { get; set; } = new();
    }

    public class PeriodDataDTO
    {
        public decimal Revenue { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int UniqueCustomers { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GrowthMetricsDTO
    {
        public decimal RevenueGrowth { get; set; }
        public decimal TransactionGrowth { get; set; }
        public decimal AverageOrderGrowth { get; set; }
        public decimal CustomerGrowth { get; set; }
        public string Trend { get; set; } = "neutral"; // up, down, neutral
    }

    // Hourly sales pattern
    public class HourlySalesDTO
    {
        public int Hour { get; set; }
        public decimal Revenue { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    // Recent transactions
    public class RecentSaleDTO
    {
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }

    // Top customers
    public class TopCustomerDTO
    {
        public string CustomerCode { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime FirstPurchase { get; set; }
        public DateTime LastPurchase { get; set; }
    }

    // Customer analytics summary
    public class CustomerAnalyticsDTO
    {
        public int TotalCustomers { get; set; }
        public decimal AverageCustomerValue { get; set; }
        public decimal AverageOrdersPerCustomer { get; set; }
        public List<TopCustomerDTO> TopCustomers { get; set; } = new();
    }
}