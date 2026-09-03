import { Sparkles } from 'lucide-react';
import { useOutletContext } from 'react-router-dom';

type AdminOutletContext = {
  displayName: string;
};

export function AdminDashboardPage() {
  const { displayName } = useOutletContext<AdminOutletContext>();

  return (
    <section className="admin-page admin-dashboard-page">
      <div className="admin-page-title">
        <h1>
          Hola, {displayName}! <Sparkles aria-hidden size={22} />
        </h1>
        <p>Bienvenido a la plataforma de gestion de actividades de cultura y experiencia Solvo.</p>
      </div>

      <section className="dashboard-placeholder" aria-label="Dashboard sin datos">
        <div className="dashboard-summary">
          <span />
          <span />
          <span />
          <span />
        </div>
        <div className="dashboard-body">
          <span className="placeholder-large" />
          <span className="placeholder-large" />
        </div>
        <span className="placeholder-wide" />
        <div className="dashboard-side">
          <span />
          <span />
        </div>
      </section>
    </section>
  );
}
