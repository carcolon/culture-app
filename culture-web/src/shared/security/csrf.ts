import { appConfig } from '../../app/config';

let cachedToken: string | null = null;

type CsrfResponse = {
  token: string;
};

export async function getCsrfToken(): Promise<string> {
  if (cachedToken) {
    return cachedToken;
  }

  const response = await fetch(`${appConfig.apiBaseUrl}/api/auth/csrf`, {
    method: 'GET',
    credentials: 'include',
    headers: {
      Accept: 'application/json',
    },
  });

  if (!response.ok) {
    throw new Error(`CSRF request failed with status ${response.status}`);
  }

  const body = (await response.json()) as CsrfResponse;
  cachedToken = body.token;
  return cachedToken;
}

export function clearCsrfToken() {
  cachedToken = null;
}
