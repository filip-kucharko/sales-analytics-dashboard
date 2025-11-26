# Sales Analytics API

Backend API for Sales Analytics Dashboard

## Tech Stack

- .NET 8.0 Web API
- Entity Framework Core
- SQL Server
- Swagger

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server
- Postman (optional)

### Running the API

```bash
cd backend/SalesAnalytics.API
dotnet run
```

API will be available at:

- HTTP: http://localhost:5192

## API Documentation

### Testing with Postman

Import the Postman collection: `SalesAnalytics.postman_collection.json`

### Endpoints

#### Health

- `GET /api/health` - Basic health check
- `GET /api/health/database` - Database connection check

#### Sales

- `GET /api/sales/summary` - Overall sales KPIs
- `GET /api/sales/by-month` - Monthly trend
- `GET /api/sales/by-region` - Regional breakdown
- More endpoints in Postman collection...

## Database

Connection string in `appsettings.json`
