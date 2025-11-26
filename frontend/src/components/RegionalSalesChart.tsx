import { useState, useEffect } from "react";
import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip } from "recharts";
import { salesApi } from "../services/api";
import type { RegionalSales } from "../types";

const COLORS = [
  "#3b82f6",
  "#8b5cf6",
  "#ec4899",
  "#f59e0b",
  "#10b981",
  "#94a3b8",
];

export default function RegionalSalesChart() {
  const [data, setData] = useState<RegionalSales[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const regionalData = await salesApi.getByRegion(undefined, 10);
        setData(regionalData);
      } catch (error) {
        console.error("Error fetching regional sales:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) {
    return <div className="h-full animate-pulse bg-slate-700 rounded"></div>;
  }

  // Take top 5 countries and group rest as "Others"
  const top5 = data.slice(0, 5);
  const rest = data.slice(5);

  const restRevenue = rest.reduce((sum, item) => sum + item.revenue, 0);
  const restPercentage = rest.reduce((sum, item) => sum + item.percentage, 0);

  // Format data for chart
  const chartData = [
    ...top5.map((item) => ({
      name: item.country,
      value: Math.round(item.revenue),
      percentage: item.percentage,
    })),
    ...(rest.length > 0
      ? [
          {
            name: "Others",
            value: Math.round(restRevenue),
            percentage: restPercentage,
          },
        ]
      : []),
  ];

  return (
    <div className="h-full flex flex-col">
      {/* Chart Container - WICHTIG: Nimmt Platz! */}
      <div className="flex-1 min-h-0">
        <ResponsiveContainer width="100%" height="100%">
          <PieChart>
            <Pie
              data={chartData}
              cx="50%"
              cy="50%"
              outerRadius="70%"
              fill="#8884d8"
              dataKey="value"
            >
              {chartData.map((entry, index) => (
                <Cell
                  key={`cell-${index}`}
                  fill={COLORS[index % COLORS.length]}
                />
              ))}
            </Pie>
            <Tooltip
              contentStyle={{
                backgroundColor: "#1e293b",
                border: "1px solid #334155",
                borderRadius: "8px",
                color: "#fff",
              }}
              labelStyle={{
                color: "#fff", // ← Label weiß
              }}
              itemStyle={{
                color: "#fff", // ← Wert weiß
              }}
              formatter={(value: number) => [
                `£${value.toLocaleString()}`,
                "Revenue",
              ]}
            />
          </PieChart>
        </ResponsiveContainer>
      </div>

      {/* Legend */}
      <div className="pt-3 space-y-1.5 border-t border-slate-700">
        {chartData.map((item, index) => (
          <div key={item.name} className="flex items-center justify-between">
            <div className="flex items-center gap-2">
              <div
                className="w-2.5 h-2.5 rounded-full flex-shrink-0"
                style={{ backgroundColor: COLORS[index] }}
              ></div>
              <span className="text-xs text-gray-300">{item.name}</span>
            </div>
            <span className="text-xs font-medium text-white">
              {item.percentage.toFixed(1)}%
            </span>
          </div>
        ))}
      </div>
    </div>
  );
}
