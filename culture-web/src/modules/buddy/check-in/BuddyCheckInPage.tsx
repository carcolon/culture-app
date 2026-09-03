import { ArrowLeft, CheckCircle2 } from 'lucide-react';
import { Link } from 'react-router-dom';

export function BuddyCheckInPage() {
  return (
    <section className="buddy-content">
      <Link className="back-link" to="/buddy/activities">
        <ArrowLeft size={17} />
        Estado de la Actividad
      </Link>
      <div className="status-pill">Actividad en progreso</div>
      <article className="check-card">
        <CheckCircle2 size={36} />
        <h1>Listo para recibir feedback?</h1>
        <p>Comparte la encuesta para iniciar el registro de experiencia de los soulvers.</p>
        <Link className="btn btn-primary full-width" to="../survey">Abrir encuesta para soulvers</Link>
      </article>
    </section>
  );
}
