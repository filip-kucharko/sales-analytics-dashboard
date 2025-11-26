import { useState } from "react";
import SalesMapChart from "./SalesMapChart";
import RegionalSalesChart from "./RegionalSalesChart";

export default function RegionalSalesToggle() {
  // to check wich state is active
  const [activeView, setActiveView] = useState<"map" | "pie">("map");

  return (
    <div className="bg-slate-800 rounded-lg p-4 border border-slate-700 h-full flex flex-col">
      {/* HEADER WITH TAB BUTTONS */}
      <div className="flex items-center justify-between mb-3">
        <div>
          <h2 className="text-base font-semibold text-white">Regional Sales</h2>
          <p className="text-xs text-gray-400">Geographic distribution</p>
        </div>

        {/* TAB BUTTONS - HERE THE USER SWITCHES */}
        <div className="flex bg-slate-700 rounded-md p-0.5">
          {/* MAP BUTTON */}
          <button
            onClick={() => setActiveView("map")}
            className={`px-3 py-1 text-xs font-medium rounded transition-colors ${
              activeView === "map"
                ? "bg-primary-600 text-white" // ← ACTIVE: Blue + White
                : "text-gray-400 hover:text-white" // ← INACTIVE: Gray
            }`}
          >
            Map View
          </button>

          {/* PIE CHART BUTTON */}
          <button
            onClick={() => setActiveView("pie")}
            className={`px-3 py-1 text-xs font-medium rounded transition-colors ${
              activeView === "pie"
                ? "bg-primary-600 text-white"
                : "text-gray-400 hover:text-white"
            }`}
          >
            Chart View
          </button>
        </div>
      </div>

      {/* CONDITIONAL RENDERING: Show Map OR Pie */}
      <div className="flex-1 min-h-0">
        {activeView === "map" ? <SalesMapChart /> : <RegionalSalesChart />}
      </div>
    </div>
  );
}
