import { http } from '../../../shared/api/http';

export type LoginRequest = {
  email: string;
  password: string;
};

export type AuthenticatedUser = {
  id: string;
  email: string;
  name: string;
  role: string;
};

export async function loginBuddy(request: LoginRequest, csrfToken: string): Promise<AuthenticatedUser> {
  return http<AuthenticatedUser>('/api/auth/buddy/login', {
    method: 'POST',
    body: JSON.stringify(request),
    csrfToken,
  });
}

export async function loginAdminLocal(request: LoginRequest, csrfToken: string): Promise<AuthenticatedUser> {
  return http<AuthenticatedUser>('/api/auth/admin/local-login', {
    method: 'POST',
    body: JSON.stringify(request),
    csrfToken,
  });
}

export async function logout(csrfToken: string): Promise<void> {
  await http<void>('/api/auth/logout', {
    method: 'POST',
    csrfToken,
  });
}

export function getAdminLoginUrl(): string {
  return `${import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:7193'}/api/auth/admin/challenge`;
}
