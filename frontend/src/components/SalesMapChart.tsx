import { useState, useEffect } from "react";
import { ComposableMap, Geographies, Geography } from "react-simple-maps";
import { salesApi } from "../services/api";
import type { RegionalSales } from "../types";

// TopoJSON URL for world map
const geoUrl = "https://cdn.jsdelivr.net/npm/world-atlas@2/countries-110m.json";

// Country name mapping (API names -> TopoJSON names)
const countryMapping: Record<string, string> = {
  "United Kingdom": "United Kingdom",
  EIRE: "Ireland",
  Germany: "Germany",
  France: "France",
  Netherlands: "Netherlands",
  Belgium: "Belgium",
  Switzerland: "Switzerland",
  Spain: "Spain",
  Norway: "Norway",
  Portugal: "Portugal",
  Sweden: "Sweden",
  Italy: "Italy",
  Austria: "Austria",
  Denmark: "Denmark",
  Australia: "Australia",
  Japan: "Japan",
  USA: "United States of America",
  Canada: "Canada",
};

export default function SalesMapChart() {
  const [data, setData] = useState<RegionalSales[]>([]);
  const [loading, setLoading] = useState(true);
  const [hoveredCountry, setHoveredCountry] = useState<string | null>(null);

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
    return (
      <div className="bg-slate-800 rounded-lg p-4 border border-slate-700 h-full">
        <div className="animate-pulse h-full bg-slate-700 rounded"></div>
      </div>
    );
  }

  // Create country data map
  const countryData = new Map<string, RegionalSales>();
  data.forEach((item) => {
    const mappedName = countryMapping[item.country];
    if (mappedName) {
      countryData.set(mappedName, item);
    }
  });

  // Get max revenue for color scaling
  const maxRevenue = Math.max(...data.map((d) => d.revenue));

  // Get color based on revenue
  const getCountryColor = (countryName: string) => {
    const countryInfo = countryData.get(countryName);
    if (!countryInfo) return "#334155"; // Default gray for no data

    const intensity = countryInfo.revenue / maxRevenue;

    // Color scale from light to dark blue
    if (intensity > 0.8) return "#1e40af"; // Very dark blue
    if (intensity > 0.6) return "#2563eb"; // Dark blue
    if (intensity > 0.4) return "#3b82f6"; // Medium blue
    if (intensity > 0.2) return "#60a5fa"; // Light blue
    return "#93c5fd"; // Very light blue
  };

  return (
    <>
      {/* Map */}
      <div className="flex-1 relative">
        <ComposableMap
          projection="geoMercator"
          projectionConfig={{
            scale: 400,
            center: [10, 52],
          }}
          style={{
            width: "100%",
            height: "100%",
          }}
        >
          <Geographies geography={geoUrl}>
            {({ geographies }) =>
              geographies.map((geo) => {
                const countryName = geo.properties.name;
                const isHovered = hoveredCountry === countryName;
                const hasData = countryData.has(countryName);

                return (
                  <Geography
                    key={geo.rsmKey}
                    geography={geo}
                    fill={getCountryColor(countryName)}
                    stroke="#1e293b"
                    strokeWidth={0.5}
                    style={{
                      default: { outline: "none" },
                      hover: {
                        fill: hasData ? "#1d4ed8" : "#475569",
                        outline: "none",
                        cursor: hasData ? "pointer" : "default",
                      },
                      pressed: { outline: "none" },
                    }}
                    onMouseEnter={() => {
                      if (hasData) setHoveredCountry(countryName);
                    }}
                    onMouseLeave={() => {
                      setHoveredCountry(null);
                    }}
                  />
                );
              })
            }
          </Geographies>
        </ComposableMap>

        {/* Tooltip */}
        {hoveredCountry && countryData.has(hoveredCountry) && (
          <div className="absolute top-2 right-2 bg-slate-900 border border-slate-700 rounded-lg p-3 shadow-lg">
            <p className="text-sm font-semibold text-white">{hoveredCountry}</p>
            <p className="text-xs text-gray-400 mt-1">
              Revenue: £
              {countryData.get(hoveredCountry)!.revenue.toLocaleString()}
            </p>
            <p className="text-xs text-gray-400">
              {countryData.get(hoveredCountry)!.percentage.toFixed(1)}% of total
            </p>
          </div>
        )}
      </div>

      {/* Legend */}
      <div className="mt-3 pt-3 border-t border-slate-700">
        <div className="flex items-center justify-between text-xs">
          <span className="text-gray-400">Revenue</span>
          <div className="flex items-center gap-2">
            <span className="text-gray-500">Low</span>
            <div className="flex gap-1">
              <div className="w-4 h-3 bg-[#93c5fd] rounded"></div>
              <div className="w-4 h-3 bg-[#60a5fa] rounded"></div>
              <div className="w-4 h-3 bg-[#3b82f6] rounded"></div>
              <div className="w-4 h-3 bg-[#2563eb] rounded"></div>
              <div className="w-4 h-3 bg-[#1e40af] rounded"></div>
            </div>
            <span className="text-gray-500">High</span>
          </div>
        </div>
      </div>

      {/* Top Countries */}
      <div className="mt-3 pt-3 border-t border-slate-700 grid grid-cols-2 gap-2">
        {data.slice(0, 6).map((item) => (
          <div key={item.country} className="flex items-center justify-between">
            <span className="text-xs text-gray-300 truncate">
              {item.country}
            </span>
            <span className="text-xs font-medium text-white ml-2">
              {item.percentage.toFixed(1)}%
            </span>
          </div>
        ))}
      </div>
    </>
  );
}
