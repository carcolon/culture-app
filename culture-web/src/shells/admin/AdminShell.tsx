import { ChevronDown, LogOut, Menu, UserCircle2 } from 'lucide-react';
import type { CSSProperties } from 'react';
import { useEffect, useRef, useState } from 'react';
import { NavLink, Outlet, useLocation, useNavigate } from 'react-router-dom';
import { http } from '../../shared/api/http';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { clearCsrfToken } from '../../shared/security/csrf';
import { LoadingScreen } from '../../shared/ui/LoadingScreen';

type AdminNavIconName = 'dashboard' | 'activities' | 'responses';

const navItems: Array<{ to: string; label: string; icon: AdminNavIconName; className: string }> = [
  { to: '/admin', label: 'Dashboard', icon: 'dashboard', className: 'dashboard-link' },
  { to: '/admin/activities', label: 'Actividades', icon: 'activities', className: 'activities-link' },
  { to: '/admin/responses', label: 'Respuestas', icon: 'responses', className: 'responses-link' },
];

function AdminNavIcon({ name }: { name: AdminNavIconName }) {
  if (name === 'dashboard') {
    return (
      <svg aria-hidden className="admin-nav-icon" viewBox="0 0 35 35">
        <path d="M8.6 16.8 17.5 9.8l8.9 7" />
        <path d="M11.2 15.8v9.7h12.6v-9.7" />
        <path d="M15.2 25.5v-5.9h4.6v5.9" />
      </svg>
    );
  }

  if (name === 'activities') {
    return (
      <svg aria-hidden className="admin-nav-icon" viewBox="0 0 35 35">
        <circle cx="17.5" cy="13.1" r="2.35" />
        <path d="M13.4 25.1c.35-2.7 1.8-4.1 4.1-4.1s3.75 1.4 4.1 4.1" />
        <circle cx="10.7" cy="15.2" r="1.9" />
        <path d="M7.2 24.9c.25-2.1 1.45-3.2 3.45-3.2.85 0 1.55.25 2.1.75" />
        <circle cx="24.3" cy="15.2" r="1.9" />
        <path d="M22.25 22.45c.55-.5 1.25-.75 2.1-.75 2 0 3.2 1.1 3.45 3.2" />
      </svg>
    );
  }

  return (
    <svg aria-hidden className="admin-nav-icon" viewBox="0 0 35 35">
      <path d="M9.1 9.9h16.8a2.1 2.1 0 0 1 2.1 2.1v8.8a2.1 2.1 0 0 1-2.1 2.1H14.4l-5.3 3.6v-3.6A2.1 2.1 0 0 1 7 20.8V12a2.1 2.1 0 0 1 2.1-2.1Z" />
      <path d="M11.8 14.4h11.4" />
      <path d="M11.8 18.3h8.2" />
    </svg>
  );
}

export function AdminShell() {
  const location = useLocation();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [sidebarScale, setSidebarScale] = useState(1);
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const userMenuRef = useRef<HTMLDivElement>(null);
  const { data: user } = useQuery({
    queryKey: ['current-user'],
    queryFn: () => http<{ name: string; email: string; role: string }>('/api/auth/me'),
    retry: false,
  });
  const currentItem = navItems.find((item) => item.to === location.pathname) ?? navItems[0];
  const displayName = user?.name?.trim() || 'Admin';

  useEffect(() => {
    const updateScale = () => {
      setSidebarScale(Math.min(1, window.innerHeight / 1080));
    };

    updateScale();
    window.addEventListener('resize', updateScale);
    return () => window.removeEventListener('resize', updateScale);
  }, []);

  useEffect(() => {
    function handlePointerDown(event: PointerEvent) {
      if (!userMenuRef.current?.contains(event.target as Node)) {
        setIsUserMenuOpen(false);
      }
    }

    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === 'Escape') {
        setIsUserMenuOpen(false);
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
    setIsUserMenuOpen(false);
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
    <main className="admin-shell" style={{ '--admin-sidebar-scale': sidebarScale } as CSSProperties}>
      <aside className="admin-sidebar">
        <div className="admin-sidebar-canvas">
          <img src="/mockups/image 8.svg" alt="Culture Solvo" className="admin-logo" />
          <nav aria-label="Admin">
            {navItems.map((item) => {
              const isActive = item.to === '/admin'
                ? location.pathname === '/admin'
                : location.pathname.startsWith(item.to);

              return (
                <NavLink
                  className={() => `${item.className}${isActive ? ' active' : ''}`}
                  key={item.label}
                  to={item.to}
                  end={item.to === '/admin'}
                >
                  <AdminNavIcon name={item.icon} />
                  <span>{item.label}</span>
                </NavLink>
              );
            })}
          </nav>
          <div className="admin-sidebar-art" aria-hidden>
            <img className="admin-star" src="/brand/admin-star.png" alt="" />
            <img className="admin-wave" src="/brand/admin-wave.png" alt="" />
          </div>
        </div>
      </aside>
      <section className="admin-main">
        <header className="admin-header">
          <div className="admin-breadcrumb">
            <button className="admin-menu-btn" type="button" aria-label="Menu">
              <Menu size={23} />
            </button>
            <span>{currentItem.label}</span>
          </div>
          <div className="user-menu-wrap" ref={userMenuRef}>
            <button
              aria-expanded={isUserMenuOpen}
              aria-haspopup="menu"
              className="admin-user"
              onClick={() => setIsUserMenuOpen((value) => !value)}
              type="button"
            >
              <UserCircle2 size={27} fill="#5376ba" strokeWidth={1.8} />
              <span>{displayName}</span>
              <ChevronDown size={15} />
            </button>
            {isUserMenuOpen ? (
              <div className="user-dropdown admin-user-dropdown" role="menu">
                <button className="user-menu-item" onClick={handleLogout} role="menuitem" type="button">
                  <LogOut size={17} />
                  <span>Cerrar sesión</span>
                </button>
              </div>
            ) : null}
          </div>
        </header>
        <Outlet context={{ displayName }} />
      </section>
    </main>
  );
}
