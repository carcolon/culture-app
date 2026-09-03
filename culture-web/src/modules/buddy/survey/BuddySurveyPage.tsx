import { SendHorizontal } from 'lucide-react';
import { Button } from '../../../shared/ui/Button';

const ratings = ['1', '2', '3', '4', '5'];

export function BuddySurveyPage() {
  return (
    <section className="buddy-content">
      <div className="survey-title">
        <p>Experiences Check-in</p>
        <h1>Soulver</h1>
      </div>
      <article className="question-card">
        <p>Tu experiencia con la actividad fue:</p>
        <div className="rating-row">
          {ratings.map((rating) => (
            <button key={rating} type="button">{rating}</button>
          ))}
        </div>
      </article>
      <article className="question-card">
        <p>Que podemos mejorar?</p>
        <textarea placeholder="Escribe tus comentarios" rows={5} />
      </article>
      <Button icon={<SendHorizontal size={17} />}>Enviar Respuestas</Button>
    </section>
  );
}
