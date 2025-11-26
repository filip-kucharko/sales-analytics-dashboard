import { useState, useEffect } from "react";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";
import { productsApi } from "../services/api";
import type { TopProduct } from "../types";

export default function TopProductsChart() {
  const [data, setData] = useState<TopProduct[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const productsData = await productsApi.getTop(5);
        setData(productsData);
      } catch (error) {
        console.error("Error fetching top products:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) {
    return (
      <div className="bg-slate-800 rounded-lg p-4 border border-slate-700 h-80">
        <div className="animate-pulse h-full bg-slate-700 rounded"></div>
      </div>
    );
  }

  // Format data for chart - shorten product names
  const chartData = data.map((item) => ({
    name:
      item.description.length > 35
        ? item.description.substring(0, 30) + "..."
        : item.description,
    fullName: item.description,
    revenue: Math.round(item.revenue),
    units: item.unitsSold,
  }));

  return (
    <div className="bg-slate-800 rounded-lg p-4 border border-slate-700">
      <div className="mb-3">
        <h2 className="text-base font-semibold text-white">Top 5 Products</h2>
        <p className="text-xs text-gray-400">Best sellers by revenue</p>
      </div>

      <ResponsiveContainer width="100%" height={220}>
        <BarChart
          data={chartData}
          layout="vertical"
          margin={{ left: 20, right: 30 }}
        >
          <CartesianGrid
            strokeDasharray="3 3"
            stroke="#334155"
            horizontal={false}
          />
          <XAxis
            type="number"
            stroke="#94a3b8"
            style={{ fontSize: "12px" }}
            tickFormatter={(value) => `£${(value / 1000).toFixed(0)}k`}
          />
          <YAxis
            type="category"
            dataKey="name"
            stroke="#94a3b8"
            style={{ fontSize: "10px" }}
            width={200}
          />
          <Tooltip
            contentStyle={{
              backgroundColor: "#1e293b",
              border: "1px solid #334155",
              borderRadius: "8px",
              color: "#fff",
            }}
            formatter={(value: number, name: string, props: any) => {
              return [`£${value.toLocaleString()}`, "Revenue"];
            }}
            labelFormatter={(label) => {
              const item = chartData.find((d) => d.name === label);
              return item?.fullName || label;
            }}
          />
          <Bar
            dataKey="revenue"
            fill="#10b981"
            radius={[0, 4, 4, 0]}
            barSize={25}
          />
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}
