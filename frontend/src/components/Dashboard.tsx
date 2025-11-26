import { DollarSign, ShoppingCart, Users, Package } from "lucide-react";
import KPICard from "./KPICard";
import { useSalesData } from "../hooks/useSalesData";

export default function Dashboard() {
  const { data, loading, error } = useSalesData();

  // Format currency
  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat("en-GB", {
      style: "currency",
      currency: "GBP",
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(value);
  };

  // Format number with commas
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
    <div className="space-y-6">
      {/* KPI Cards Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <KPICard
          title="Total Revenue"
          value={data ? formatCurrency(data.totalRevenue) : "---"}
          subtitle={
            data
              ? `${formatNumber(data.totalTransactions)} transactions`
              : "---"
          }
          icon={<DollarSign className="w-5 h-5" />}
          loading={loading}
        />

        <KPICard
          title="Average Order Value"
          value={data ? formatCurrency(data.averageOrderValue) : "---"}
          subtitle="Per transaction"
          icon={<ShoppingCart className="w-5 h-5" />}
          loading={loading}
        />

        <KPICard
          title="Unique Customers"
          value={data ? formatNumber(data.uniqueCustomers) : "---"}
          subtitle="Active customers"
          icon={<Users className="w-5 h-5" />}
          loading={loading}
        />

        <KPICard
          title="Products Sold"
          value={data ? formatNumber(data.uniqueProducts) : "---"}
          subtitle="Unique items"
          icon={<Package className="w-5 h-5" />}
          loading={loading}
        />
      </div>

      {/* Date Range Info */}
      {data && (
        <div className="bg-slate-800 rounded-lg p-4 border border-slate-700">
          <p className="text-sm text-gray-400">
            Data from{" "}
            <span className="text-white font-medium">
              {new Date(data.startDate).toLocaleDateString("en-US", {
                year: "numeric",
                month: "long",
                day: "numeric",
              })}
            </span>
            {" to "}
            <span className="text-white font-medium">
              {new Date(data.endDate).toLocaleDateString("en-US", {
                year: "numeric",
                month: "long",
                day: "numeric",
              })}
            </span>
          </p>
        </div>
      )}

      {/* Placeholder for Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-slate-800 rounded-lg p-6 border border-slate-700 h-80 flex items-center justify-center">
          <p className="text-gray-400">Sales Trend Chart (Coming soon)</p>
        </div>
        <div className="bg-slate-800 rounded-lg p-6 border border-slate-700 h-80 flex items-center justify-center">
          <p className="text-gray-400">Regional Sales Chart (Coming soon)</p>
        </div>
      </div>
    </div>
  );
}
