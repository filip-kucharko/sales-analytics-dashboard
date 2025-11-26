export interface SalesSummary {
  totalRevenue: number;
  totalTransactions: number;
  averageOrderValue: number;
  uniqueCustomers: number;
  uniqueProducts: number;
  startDate: string;
  endDate: string;
}

export interface MonthlySales {
  month: string;
  year: number;
  revenue: number;
  transactionCount: number;
  averageOrderValue: number;
}

export interface RegionalSales {
  country: string;
  revenue: number;
  transactionCount: number;
  percentage: number;
}

export interface TopProduct {
  stockCode: string;
  description: string;
  revenue: number;
  unitsSold: number;
  transactionCount: number;
}

export interface TopCustomer {
  customerCode: string;
  totalRevenue: number;
  orderCount: number;
  averageOrderValue: number;
  firstPurchase: string;
  lastPurchase: string;
}

export interface SalesComparison {
  currentPeriod: PeriodData;
  previousPeriod: PeriodData;
  growth: GrowthMetrics;
}

export interface PeriodData {
  revenue: number;
  transactionCount: number;
  averageOrderValue: number;
  uniqueCustomers: number;
  startDate: string;
  endDate: string;
}

export interface GrowthMetrics {
  revenueGrowth: number;
  transactionGrowth: number;
  averageOrderGrowth: number;
  customerGrowth: number;
  trend: "up" | "down" | "neutral";
}

export interface DateRange {
  startDate?: string;
  endDate?: string;
}
