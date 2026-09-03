import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Navigate, RouterProvider, createBrowserRouter } from 'react-router-dom';
import { AdminShell } from '../shells/admin/AdminShell';
import { BuddyShell } from '../shells/buddy/BuddyShell';
import { AdminActivitiesPage } from '../modules/admin/activities/AdminActivitiesPage';
import { AdminDashboardPage } from '../modules/admin/dashboard/AdminDashboardPage';
import { AdminResponsesPage } from '../modules/admin/responses/AdminResponsesPage';
import { BuddyActivitiesPage } from '../modules/buddy/activities/BuddyActivitiesPage';
import { BuddyCheckInPage } from '../modules/buddy/check-in/BuddyCheckInPage';
import { BuddyLoginPage } from '../modules/buddy/auth/BuddyLoginPage';
import { BuddySurveyPage } from '../modules/buddy/survey/BuddySurveyPage';
import { LoadingPreviewPage } from '../modules/loading/LoadingPreviewPage';
import { RequireAuth } from '../shared/auth/RequireAuth';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      refetchOnWindowFocus: false,
    },
  },
});

const router = createBrowserRouter([
  { path: '/', element: <Navigate to="/buddy/login" replace /> },
  { path: '/loading-preview', element: <LoadingPreviewPage /> },
  { path: '/buddy/login', element: <BuddyLoginPage /> },
  {
    path: '/buddy',
    element: (
      <RequireAuth area="buddy">
        <BuddyShell />
      </RequireAuth>
    ),
    children: [
      { index: true, element: <Navigate to="/buddy/activities" replace /> },
      { path: 'activities', element: <BuddyActivitiesPage /> },
      { path: 'activities/:activityId/check-in', element: <BuddyCheckInPage /> },
      { path: 'activities/:activityId/survey', element: <BuddySurveyPage /> },
    ],
  },
  {
    path: '/admin',
    element: (
      <RequireAuth area="admin">
        <AdminShell />
      </RequireAuth>
    ),
    children: [
      { index: true, element: <AdminDashboardPage /> },
      { path: 'activities', element: <AdminActivitiesPage /> },
      { path: 'responses', element: <AdminResponsesPage /> },
    ],
  },
]);

export function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
    </QueryClientProvider>
  );
}
