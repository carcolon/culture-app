export const appConfig = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:7193',
  enableLocalAdminLogin: import.meta.env.DEV && import.meta.env.VITE_ENABLE_LOCAL_ADMIN_LOGIN === 'true',
};
