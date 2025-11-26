import { useState, useEffect } from "react";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { salesApi } from "../services/api";
import type { MonthlySales } from "../types";

type ViewMode = "month" | "overall";

export default function SalesTrendChart() {
  const [data, setData] = useState<MonthlySales[]>([]);
  const [loading, setLoading] = useState(true);
  const [viewMode, setViewMode] = useState<ViewMode>("overall");
  const [currentMonthIndex, setCurrentMonthIndex] = useState(0);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const monthlyData = await salesApi.getByMonth();
        setData(monthlyData);
        setCurrentMonthIndex(monthlyData.length - 1); // Start at last month
      } catch (error) {
        console.error("Error fetching monthly sales:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) {
    return (
      <div className="bg-slate-800 rounded-lg p-4 border border-slate-700 h-full">
        <div className="animate-pulse h-full bg-slate-700 rounded"></div>
      </div>
    );
  }

  // Get data based on view mode
  const getChartData = () => {
    if (viewMode === "overall") {
      return data.map((item) => ({
        name: `${item.month}/${item.year}`,
        revenue: Math.round(item.revenue),
      }));
    } else {
      // Show current month and surrounding context (6 months window)
      const start = Math.max(0, currentMonthIndex - 5);
      const end = currentMonthIndex + 1;
      return data.slice(start, end).map((item) => ({
        name: `${item.month}/${item.year}`,
        revenue: Math.round(item.revenue),
      }));
    }
  };

  const chartData = getChartData();
  const currentMonth = data[currentMonthIndex];

  const handlePrevMonth = () => {
    if (currentMonthIndex > 0) {
      setCurrentMonthIndex(currentMonthIndex - 1);
    }
  };

  const handleNextMonth = () => {
    if (currentMonthIndex < data.length - 1) {
      setCurrentMonthIndex(currentMonthIndex + 1);
    }
  };

  return (
    <div className="bg-slate-800 rounded-lg p-4 border border-slate-700 h-full flex flex-col">
      {/* Header with Controls */}
      <div className="flex items-center justify-between mb-3">
        <div>
          <h2 className="text-base font-semibold text-white">Sales Trend</h2>
          <p className="text-xs text-gray-400">
            {viewMode === "month" && currentMonth
              ? `${currentMonth.month}/${currentMonth.year} - £${Math.round(
                  currentMonth.revenue
                ).toLocaleString()}`
              : "Complete overview"}
          </p>
        </div>

        <div className="flex items-center gap-2">
          {/* Month Navigation (only visible in month mode) */}
          {viewMode === "month" && (
            <div className="flex items-center gap-1 mr-2">
              <button
                onClick={handlePrevMonth}
                disabled={currentMonthIndex === 0}
                className={`p-1 rounded transition-colors ${
                  currentMonthIndex === 0
                    ? "text-gray-600 cursor-not-allowed"
                    : "text-gray-400 hover:text-white hover:bg-slate-700"
                }`}
              >
                <ChevronLeft className="w-4 h-4" />
              </button>
              <button
                onClick={handleNextMonth}
                disabled={currentMonthIndex === data.length - 1}
                className={`p-1 rounded transition-colors ${
                  currentMonthIndex === data.length - 1
                    ? "text-gray-600 cursor-not-allowed"
                    : "text-gray-400 hover:text-white hover:bg-slate-700"
                }`}
              >
                <ChevronRight className="w-4 h-4" />
              </button>
            </div>
          )}

          {/* View Mode Toggle */}
          <div className="flex bg-slate-700 rounded-md p-0.5">
            <button
              onClick={() => setViewMode("month")}
              className={`px-3 py-1 text-xs font-medium rounded transition-colors ${
                viewMode === "month"
                  ? "bg-primary-600 text-white"
                  : "text-gray-400 hover:text-white"
              }`}
            >
              Month
            </button>
            <button
              onClick={() => setViewMode("overall")}
              className={`px-3 py-1 text-xs font-medium rounded transition-colors ${
                viewMode === "overall"
                  ? "bg-primary-600 text-white"
                  : "text-gray-400 hover:text-white"
              }`}
            >
              Overall
            </button>
          </div>
        </div>
      </div>

      {/* Chart */}
      <div className="flex-1">
        <ResponsiveContainer width="100%" height="100%">
          <LineChart data={chartData}>
            <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
            <XAxis
              dataKey="name"
              stroke="#94a3b8"
              style={{ fontSize: "10px" }}
            />
            <YAxis
              stroke="#94a3b8"
              style={{ fontSize: "10px" }}
              tickFormatter={(value) => `£${(value / 1000).toFixed(0)}k`}
            />
            <Tooltip
              contentStyle={{
                backgroundColor: "#1e293b",
                border: "1px solid #334155",
                borderRadius: "8px",
                color: "#fff",
                fontSize: "12px",
              }}
              formatter={(value: number) => [
                `£${value.toLocaleString()}`,
                "Revenue",
              ]}
            />
            <Line
              type="monotone"
              dataKey="revenue"
              stroke="#3b82f6"
              strokeWidth={2}
              dot={{ fill: "#3b82f6", r: 3 }}
              activeDot={{ r: 5 }}
              name="Revenue"
            />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
}
