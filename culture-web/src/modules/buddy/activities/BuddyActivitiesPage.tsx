import { ArrowRight, Plus } from 'lucide-react';
import { Link } from 'react-router-dom';
import { Button } from '../../../shared/ui/Button';

const activities = [
  { id: 'pulse-check', name: 'Experiences Pulse', status: 'Disponible', location: 'Sede Barranquilla' },
  { id: 'soulver-survey', name: 'Soulver Survey', status: 'En progreso', location: 'Sede Medellin' },
];

export function BuddyActivitiesPage() {
  return (
    <section className="buddy-content">
      <div className="buddy-greeting">
        <p>Hola, Maria Ossa</p>
        <h1>Que actividad vas a realizar?</h1>
      </div>
      <div className="stack">
        {activities.map((activity) => (
          <article className="activity-tile" key={activity.id}>
            <span>
              <strong>{activity.name}</strong>
              <small>{activity.location}</small>
            </span>
            <Link to={`/buddy/activities/${activity.id}/check-in`}>
              <ArrowRight size={18} />
            </Link>
          </article>
        ))}
      </div>
      <Button icon={<Plus size={17} />}>Empezar Nueva Actividad</Button>
    </section>
  );
}
