import { LockKeyhole, Mail } from 'lucide-react';
import type { FormEvent } from 'react';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { appConfig } from '../../../app/config';
import type { AuthMode } from '../../../shared/auth/authMode';
import { getCsrfToken } from '../../../shared/security/csrf';
import { LoadingScreen } from '../../../shared/ui/LoadingScreen';
import { getAdminLoginUrl, loginAdminLocal, loginBuddy } from './authApi';

export function BuddyLoginPage() {
  const navigate = useNavigate();
  const [authMode, setAuthMode] = useState<AuthMode>('buddy');
  const [email, setEmail] = useState('buddy@solvoglobal.com');
  const [password, setPassword] = useState('ChangeMe123!');
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    if (authMode === 'admin' && !appConfig.enableLocalAdminLogin) {
      window.location.assign(getAdminLoginUrl());
      return;
    }

    setIsSubmitting(true);

    try {
      const csrfToken = await getCsrfToken();
      if (authMode === 'admin') {
        await loginAdminLocal({ email, password }, csrfToken);
        navigate('/admin');
        return;
      }

      await loginBuddy({ email, password }, csrfToken);
      navigate('/buddy/activities');
    } catch (loginError) {
      if (loginError instanceof Error && loginError.message === 'API_UNAVAILABLE') {
        setError('No pudimos conectar con la API local. Verifica que este levantada.');
        return;
      }

      if (loginError instanceof Error && loginError.message === 'HTTP_423') {
        setError('La cuenta esta bloqueada temporalmente. Intenta de nuevo mas tarde.');
        return;
      }

      if (loginError instanceof Error && loginError.message === 'HTTP_429') {
        setError('Demasiados intentos de inicio de sesion. Espera un momento e intenta de nuevo.');
        return;
      }

      setError('No pudimos iniciar sesion. Revisa tus credenciales.');
    } finally {
      setIsSubmitting(false);
    }
  }

  function selectMode(nextMode: AuthMode) {
    setAuthMode(nextMode);
    setError(null);

    if (nextMode === 'admin' && appConfig.enableLocalAdminLogin) {
      setEmail('admin@solvoglobal.com');
      setPassword('AdminChangeMe123!');
      return;
    }

    if (nextMode === 'buddy') {
      setEmail('buddy@solvoglobal.com');
      setPassword('ChangeMe123!');
    }
  }

  if (isSubmitting) {
    return <LoadingScreen label="Validando..." />;
  }

  return (
    <main className="login-page">
      <section className="login-panel">
        <img src="/brand/login-icon.png" alt="Culture & Experience" className="login-logo" />
        <div className="login-copy">
          <h1>Bienvenido a Culture & Experience</h1>
          <p>Ingresa para consultar y completar tus actividades asignadas.</p>
        </div>
        <form className="stack" onSubmit={handleSubmit}>
          <div className="auth-mode" role="radiogroup" aria-label="Tipo de acceso">
            <button
              aria-checked={authMode === 'buddy'}
              className={authMode === 'buddy' ? 'active' : ''}
              onClick={() => selectMode('buddy')}
              role="radio"
              type="button"
            >
              Buddy
            </button>
            <button
              aria-checked={authMode === 'admin'}
              className={authMode === 'admin' ? 'active' : ''}
              onClick={() => selectMode('admin')}
              role="radio"
              type="button"
            >
              Admin
            </button>
          </div>
          {authMode === 'buddy' || appConfig.enableLocalAdminLogin ? (
            <>
              <label>
                Email Corporativo
                <span className="input-shell">
                  <Mail size={16} />
                  <input
                    autoComplete="email"
                    inputMode="email"
                    onChange={(event) => setEmail(event.target.value)}
                    placeholder="nombre@solvoglobal.com"
                    required
                    type="email"
                    value={email}
                  />
                </span>
              </label>
              <label>
                Password
                <span className="input-shell">
                  <LockKeyhole size={16} />
                  <input
                    autoComplete="current-password"
                    minLength={8}
                    onChange={(event) => setPassword(event.target.value)}
                    placeholder="Password"
                    required
                    type="password"
                    value={password}
                  />
                </span>
              </label>
            </>
          ) : (
            <div className="admin-login-note">
              <strong>Acceso administrativo</strong>
              <span>Usa tu cuenta corporativa con Microsoft Entra.</span>
            </div>
          )}
          {error ? <p className="form-error" role="alert">{error}</p> : null}
          <button className="btn btn-primary full-width" disabled={isSubmitting} type="submit">
            {authMode === 'admin' && !appConfig.enableLocalAdminLogin
              ? 'Continuar con Entra'
              : isSubmitting
                ? 'Validando...'
                : 'Iniciar Sesion'}
          </button>
        </form>
      </section>
    </main>
  );
}
