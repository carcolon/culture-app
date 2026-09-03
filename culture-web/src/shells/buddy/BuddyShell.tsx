import { CalendarDays, Home, LogOut, UserRound } from 'lucide-react';
import { useEffect, useRef, useState } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useQueryClient } from '@tanstack/react-query';
import { http } from '../../shared/api/http';
import { clearCsrfToken } from '../../shared/security/csrf';
import { LoadingScreen } from '../../shared/ui/LoadingScreen';

export function BuddyShell() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [isProfileMenuOpen, setIsProfileMenuOpen] = useState(false);
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const profileMenuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handlePointerDown(event: PointerEvent) {
      if (!profileMenuRef.current?.contains(event.target as Node)) {
        setIsProfileMenuOpen(false);
      }
    }

    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === 'Escape') {
        setIsProfileMenuOpen(false);
      }
    }

    document.addEventListener('pointerdown', handlePointerDown);
    document.addEventListener('keydown', handleKeyDown);

    return () => {
      document.removeEventListener('pointerdown', handlePointerDown);
      document.removeEventListener('keydown', handleKeyDown);
    };
  }, []);

  async function handleLogout() {
    setIsProfileMenuOpen(false);
    setIsLoggingOut(true);

    try {
      await http<void>('/api/auth/logout', { method: 'POST' });
    } finally {
      clearCsrfToken();
      queryClient.clear();
      navigate('/buddy/login', { replace: true });
    }
  }

  if (isLoggingOut) {
    return <LoadingScreen label="Cerrando sesión..." />;
  }

  return (
    <main className="buddy-shell">
      <section className="buddy-device">
        <header className="buddy-topbar">
          <img src="/mockups/image 3.svg" alt="Culture & Experience" className="buddy-mark" />
          <div className="user-menu-wrap buddy-profile-menu-wrap" ref={profileMenuRef}>
            <button
              aria-expanded={isProfileMenuOpen}
              aria-haspopup="menu"
              className="icon-btn"
              onClick={() => setIsProfileMenuOpen((value) => !value)}
              type="button"
              aria-label="Perfil"
            >
              <UserRound size={18} />
            </button>
            {isProfileMenuOpen ? (
              <div className="user-dropdown buddy-user-dropdown" role="menu">
                <button className="user-menu-item" onClick={handleLogout} role="menuitem" type="button">
                  <LogOut size={17} />
                  <span>Cerrar sesión</span>
                </button>
              </div>
            ) : null}
          </div>
        </header>
        <Outlet />
        <nav className="buddy-nav" aria-label="Buddy">
          <NavLink to="/buddy/activities">
            <Home size={18} />
            Inicio
          </NavLink>
          <NavLink to="/buddy/activities">
            <CalendarDays size={18} />
            Actividades
          </NavLink>
        </nav>
      </section>
    </main>
  );
}
