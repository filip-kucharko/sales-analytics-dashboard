import axios from "axios";
import type {
  SalesSummary,
  MonthlySales,
  RegionalSales,
  TopProduct,
  TopCustomer,
  SalesComparison,
  DateRange,
} from "../types";

const API_BASE_URL = "http://localhost:5192/api";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Sales Endpoints
export const salesApi = {
  getSummary: async (dateRange?: DateRange) => {
    const { data } = await api.get<SalesSummary>("/sales/summary", {
      params: dateRange,
    });
    return data;
  },

  getByMonth: async (dateRange?: DateRange) => {
    const { data } = await api.get<MonthlySales[]>("/sales/by-month", {
      params: dateRange,
    });
    return data;
  },

  getByRegion: async (dateRange?: DateRange, top: number = 10) => {
    const { data } = await api.get<RegionalSales[]>("/sales/by-region", {
      params: { ...dateRange, top },
    });
    return data;
  },

  getComparison: async (year?: number, month?: number) => {
    const { data } = await api.get<SalesComparison>("/sales/comparison", {
      params: { year, month },
    });
    return data;
  },
};

// Products Endpoints
export const productsApi = {
  getTop: async (top: number = 10, dateRange?: DateRange) => {
    const { data } = await api.get<TopProduct[]>("/products/top", {
      params: { top, ...dateRange },
    });
    return data;
  },

  getCount: async () => {
    const { data } = await api.get<{ totalProducts: number }>(
      "/products/count"
    );
    return data;
  },
};

// Customers Endpoints
export const customersApi = {
  getTop: async (top: number = 10, dateRange?: DateRange) => {
    const { data } = await api.get<TopCustomer[]>("/customers/top", {
      params: { top, ...dateRange },
    });
    return data;
  },

  getCount: async () => {
    const { data } = await api.get<{ totalCustomers: number }>(
      "/customers/count"
    );
    return data;
  },
};

// Health Check
export const healthApi = {
  check: async () => {
    const { data } = await api.get("/health");
    return data;
  },

  checkDatabase: async () => {
    const { data } = await api.get("/health/database");
    return data;
  },
};

export default api;
