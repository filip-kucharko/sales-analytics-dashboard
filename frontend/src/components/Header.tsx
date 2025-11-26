import { Calendar } from "lucide-react";

export default function Header() {
  const currentDate = new Date().toLocaleDateString("en-US", {
    weekday: "long",
    year: "numeric",
    month: "long",
    day: "numeric",
  });

  return (
    <header className="bg-slate-800 border-b border-slate-700">
      <div className="px-6 py-4">
        <div className="flex items-center justify-between">
          {/* Title */}
          <div>
            <h1 className="text-2xl font-bold text-white">
              Sales Analytics Dashboard
            </h1>
            <p className="text-sm text-gray-400 mt-1 flex items-center gap-2">
              <Calendar className="w-4 h-4" />
              {currentDate}
            </p>
          </div>

          {/* Actions */}
          <div className="flex items-center gap-3">
            <button className="px-4 py-2 bg-slate-700 hover:bg-slate-600 text-white rounded-lg transition-colors text-sm font-medium">
              Export Report
            </button>
            <button className="px-4 py-2 bg-primary-600 hover:bg-primary-700 text-white rounded-lg transition-colors text-sm font-medium">
              Refresh Data
            </button>
          </div>
        </div>
      </div>
    </header>
  );
}
