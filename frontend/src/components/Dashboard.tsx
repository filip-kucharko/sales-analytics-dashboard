import { DollarSign, ShoppingCart, Users, Package } from "lucide-react";
import KPICard from "./KPICard";
import SalesTrendChart from "./SalesTrendChart";
import TopProductsChart from "./TopProductsChart";
import { useSalesData } from "../hooks/useSalesData";
import RegionalSalesToggle from "./RegionalSalesToggle";

export default function Dashboard() {
  const { data, loading, error } = useSalesData();

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat("en-GB", {
      style: "currency",
      currency: "GBP",
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(value);
  };

  const formatNumber = (value: number) => {
    return new Intl.NumberFormat("en-US").format(value);
  };

  if (error) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-center">
          <p className="text-red-500 mb-2">Error loading data</p>
          <p className="text-gray-400 text-sm">{error}</p>
        </div>
      </div>
    );
  }

  return (
    <div
      className="grid grid-cols-12 gap-4"
      style={{ height: "calc(100vh - 80px)" }}
    >
      {/* LINKE SPALTE: KPIs + Sales Trend */}
      <div className="col-span-4 flex flex-col gap-4">
        {/* KPI Cards 2x2 Grid */}
        <div className="grid grid-cols-2 gap-4">
          <KPICard
            title="Revenue"
            value={data ? formatCurrency(data.totalRevenue) : "---"}
            subtitle={
              data ? `${formatNumber(data.totalTransactions)} orders` : "---"
            }
            icon={<DollarSign className="w-4 h-4" />}
            loading={loading}
          />

          <KPICard
            title="Avg Order"
            value={data ? formatCurrency(data.averageOrderValue) : "---"}
            subtitle="Per order"
            icon={<ShoppingCart className="w-4 h-4" />}
            loading={loading}
          />

          <KPICard
            title="Customers"
            value={data ? formatNumber(data.uniqueCustomers) : "---"}
            subtitle="Active"
            icon={<Users className="w-4 h-4" />}
            loading={loading}
          />

          <KPICard
            title="Products"
            value={data ? formatNumber(data.uniqueProducts) : "---"}
            subtitle="Unique"
            icon={<Package className="w-4 h-4" />}
            loading={loading}
          />
        </div>

        {/* Sales Trend Chart */}
        <div className="flex-1 min-h-0">
          <SalesTrendChart />
        </div>
      </div>

      {/* MITTLERE SPALTE: Map/Pie Toggle */}
      <div className="col-span-4">
        <RegionalSalesToggle />
      </div>

      {/* RECHTE SPALTE: Top Products */}
      <div className="col-span-4">
        <TopProductsChart />
      </div>
    </div>
  );
}
