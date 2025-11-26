# Sales Analytics Dashboard - Frontend

Modern, responsive sales analytics dashboard built with React and TypeScript.

![Dashboard Preview](../docs/dashboard-screenshot.png)

## Features

- **Interactive Data Visualization**

  - Sales trend chart with Month/Overall view
  - Geographic sales map with country-level data
  - Regional breakdown pie chart
  - Top 5 products bar chart

- **Real-time Metrics**
  - Total revenue with transaction count
  - Average order value
  - Unique customers count
  - Products sold

## Tech Stack

- **React 18** - UI framework
- **TypeScript** - Type safety
- **Vite** - Build tool & dev server
- **Tailwind CSS** - Styling
- **Recharts** - Chart library
- **react-simple-maps** - Geographic visualization
- **Axios** - HTTP client
- **Lucide React** - Icons

## Installation

```bash
# Install dependencies
npm install


# Start development server
npm run dev
```

## Configuration

### API Endpoint

Update the backend URL in `src/services/api.ts`:

```typescript
const API_BASE_URL = "http://localhost:5192/api";
```

## Project Structure

```
src/
├── components/
│   ├── Dashboard.tsx    # Main dashboard layout
│   ├── Header.tsx       # App header
│   ├── KPICard.tsx      # Reusable KPI card
│   ├── SalesTrendChart.tsx
│   ├── RegionalSalesToggle.tsx
│   ├── SalesMapChart.tsx
│   ├── RegionalSalesChart.tsx
│   └── TopProductsChart.tsx
├── services/            # API communication
│   └── api.ts
├── types/               # TypeScript interfaces
│   └── index.ts
├── hooks/               # Custom React hooks
│   └── useSalesData.ts
├── App.tsx
└── main.tsx
```

## Components

### Dashboard

Main layout component with 3-column grid:

- Left: KPI cards + Sales trend
- Middle: Map/Pie chart toggle
- Right: Top products

### KPICard

Reusable card component for displaying metrics with icons and optional trends.

### SalesTrendChart

Line chart with Month/Overall toggle and navigation arrows for monthly view.

### RegionalSalesToggle

Container with tab switcher for Map and Pie chart views.

### SalesMapChart

Interactive world map with color-coded countries based on revenue.

### RegionalSalesChart

Pie chart showing top 5 countries + others with percentage breakdown.

### TopProductsChart

Horizontal bar chart displaying top 5 products by revenue.

## API Integration

The app communicates with a .NET backend API:

- `GET /api/sales/summary` - Overall sales metrics
- `GET /api/sales/by-month` - Monthly sales data
- `GET /api/sales/by-region` - Regional breakdown
- `GET /api/products/top` - Top products

## Backend Repository

This frontend connects to a .NET Web API backend. See `../backend/README.md` for backend documentation.
