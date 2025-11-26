import { TrendingUp, TrendingDown, Minus } from "lucide-react";

interface KPICardProps {
  title: string;
  value: string | number;
  subtitle?: string;
  trend?: {
    value: number;
    isPositive: boolean;
  };
  icon?: React.ReactNode;
  loading?: boolean;
}

export default function KPICard({
  title,
  value,
  subtitle,
  trend,
  icon,
  loading = false,
}: KPICardProps) {
  return (
    <div className="bg-slate-800 rounded-lg p-6 border border-slate-700 hover:border-slate-600 transition-colors">
      {/* Header */}
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-sm font-medium text-gray-400">{title}</h3>
        {icon && <div className="text-gray-400">{icon}</div>}
      </div>

      {/* Value */}
      {loading ? (
        <div className="h-8 bg-slate-700 rounded animate-pulse"></div>
      ) : (
        <div className="mb-2">
          <p className="text-3xl font-bold text-white">{value}</p>
        </div>
      )}

      {/* Subtitle & Trend */}
      <div className="flex items-center justify-between">
        {subtitle && <p className="text-sm text-gray-400">{subtitle}</p>}

        {trend && (
          <div
            className={`flex items-center gap-1 text-sm font-medium ${
              trend.isPositive ? "text-green-500" : "text-red-500"
            }`}
          >
            {trend.isPositive ? (
              <TrendingUp className="w-4 h-4" />
            ) : trend.value === 0 ? (
              <Minus className="w-4 h-4" />
            ) : (
              <TrendingDown className="w-4 h-4" />
            )}
            <span>{Math.abs(trend.value)}%</span>
          </div>
        )}
      </div>
    </div>
  );
}
