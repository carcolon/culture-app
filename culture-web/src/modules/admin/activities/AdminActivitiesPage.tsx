import { ChevronDownCircle, Pencil, PlusCircle, Search, Sparkles } from 'lucide-react';
import { Button } from '../../../shared/ui/Button';

type ActivityRow = {
  id: string;
  activity: string;
  type: string;
  site: string;
  questions: number;
  status: 'Activa' | 'Inactiva';
  createdAt: string;
};

const activities: ActivityRow[] = [];

export function AdminActivitiesPage() {
  return (
    <section className="admin-page">
      <div className="admin-section-header">
        <div className="admin-page-title">
          <h1>
            Actividades <Sparkles aria-hidden size={22} />
          </h1>
          <p>Administra las actividades que realizaran los buddies en cada sede.</p>
        </div>
        <Button icon={<PlusCircle size={16} />}>Crear nueva actividad</Button>
      </div>

      <div className="table-toolbar">
        <label className="search-box">
          <Search size={16} />
          <input placeholder="Buscar actividad..." />
        </label>
        <button className="filter-control" type="button">
          Sede
          <ChevronDownCircle size={15} />
        </button>
        <button className="filter-control" type="button">
          Tipo de actividad
          <ChevronDownCircle size={15} />
        </button>
        <button className="filter-control" type="button">
          Estado
          <ChevronDownCircle size={15} />
        </button>
      </div>

      {activities.length > 0 ? (
        <div className="data-table">
          <table>
            <thead>
              <tr>
                <th>Actividad</th>
                <th>Tipo de actividad</th>
                <th>Sede</th>
                <th>Preguntas asociadas</th>
                <th>Estado</th>
                <th>Fecha de creacion</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {activities.map((activity) => (
                <tr key={activity.id}>
                  <td>{activity.activity}</td>
                  <td>{activity.type}</td>
                  <td>{activity.site}</td>
                  <td>{activity.questions}</td>
                  <td><span className={`status-badge ${activity.status === 'Activa' ? 'is-active' : 'is-inactive'}`}>{activity.status}</span></td>
                  <td>{activity.createdAt}</td>
                  <td>
                    <button className="table-action" type="button" aria-label={`Editar ${activity.activity}`}>
                      <Pencil size={15} />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : null}
    </section>
  );
}
