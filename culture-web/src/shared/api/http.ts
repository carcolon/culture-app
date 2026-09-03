import { appConfig } from '../../app/config';
import { getCsrfToken } from '../security/csrf';

type HttpOptions = RequestInit & {
  csrfToken?: string;
};

const mutatingMethods = new Set(['POST', 'PUT', 'PATCH', 'DELETE']);

export async function http<TResponse>(path: string, options: HttpOptions = {}): Promise<TResponse> {
  const headers = new Headers(options.headers);
  headers.set('Accept', 'application/json');
  const method = (options.method ?? 'GET').toUpperCase();

  if (options.body && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json');
  }

  if (mutatingMethods.has(method)) {
    headers.set('X-CSRF-TOKEN', options.csrfToken ?? await getCsrfToken());
  }

  let response: Response;

  try {
    response = await fetch(`${appConfig.apiBaseUrl}${path}`, {
      ...options,
      headers,
      credentials: 'include',
    });
  } catch {
    throw new Error('API_UNAVAILABLE');
  }

  if (!response.ok) {
    throw new Error(`HTTP_${response.status}`);
  }

  if (response.status === 204) {
    return undefined as TResponse;
  }

  return response.json() as Promise<TResponse>;
}
