import { useQuery } from '@tanstack/react-query';
import type { ReactNode } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { http } from '../api/http';
import { LoadingScreen } from '../ui/LoadingScreen';

export type AuthenticatedUser = {
  id: string;
  email: string;
  name: string;
  role: string;
};

type RequireAuthProps = {
  children: ReactNode;
  area: 'admin' | 'buddy';
};

export function RequireAuth({ children, area }: RequireAuthProps) {
  const location = useLocation();
  const {
    data: user,
    error,
    isLoading,
  } = useQuery({
    queryKey: ['current-user'],
    queryFn: () => http<AuthenticatedUser>('/api/auth/me'),
    retry: false,
  });

  if (isLoading) {
    return <LoadingScreen />;
  }

  if (error || !user) {
    return <Navigate to="/buddy/login" replace state={{ from: location }} />;
  }

  if (area === 'buddy' && user.role !== 'Buddy') {
    return <Navigate to="/admin" replace />;
  }

  if (area === 'admin' && user.role === 'Buddy') {
    return <Navigate to="/buddy/activities" replace />;
  }

  return children;
}
