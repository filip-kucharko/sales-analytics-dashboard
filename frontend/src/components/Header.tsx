import { Calendar } from "lucide-react";

export default function Header() {
  const currentDate = new Date().toLocaleDateString("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });

  return (
    <header className="bg-slate-800 border-b border-slate-700">
      <div className="px-6 py-3">
        <div className="flex items-center justify-between">
          {/* Title */}
          <div>
            <h1 className="text-xl font-bold text-white">
              Sales Analytics Dashboard
            </h1>
            <p className="text-xs text-gray-400 mt-0.5 flex items-center gap-1.5">
              <Calendar className="w-3 h-3" />
              {currentDate}
            </p>
          </div>
        </div>
      </div>
    </header>
  );
}
