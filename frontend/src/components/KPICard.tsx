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
    <div className="bg-slate-800 rounded-lg p-4 border border-slate-700 hover:border-slate-600 transition-colors">
      {/* Header */}
      <div className="flex items-center justify-between mb-2">
        <h3 className="text-xs font-medium text-gray-400 uppercase tracking-wide">
          {title}
        </h3>
        {icon && <div className="text-gray-400">{icon}</div>}
      </div>

      {/* Value */}
      {loading ? (
        <div className="h-7 bg-slate-700 rounded animate-pulse"></div>
      ) : (
        <div className="mb-1">
          <p className="text-2xl font-bold text-white">{value}</p>
        </div>
      )}

      {/* Subtitle & Trend */}
      <div className="flex items-center justify-between">
        {subtitle && <p className="text-xs text-gray-400">{subtitle}</p>}

        {trend && (
          <div
            className={`flex items-center gap-1 text-xs font-medium ${
              trend.isPositive ? "text-green-500" : "text-red-500"
            }`}
          >
            {trend.isPositive ? (
              <TrendingUp className="w-3 h-3" />
            ) : trend.value === 0 ? (
              <Minus className="w-3 h-3" />
            ) : (
              <TrendingDown className="w-3 h-3" />
            )}
            <span>{Math.abs(trend.value)}%</span>
          </div>
        )}
      </div>
    </div>
  );
}
