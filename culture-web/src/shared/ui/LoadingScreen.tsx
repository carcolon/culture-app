type LoadingScreenProps = {
  label?: string;
};

export function LoadingScreen({ label = 'Cargando...' }: LoadingScreenProps) {
  return (
    <main className="loading-screen" aria-busy="true" aria-label={label}>
      <section className="loading-screen-content">
        <img className="loading-screen-logo" src="/mockups/image 8.svg" alt="Culture Solvo" />
        <div className="loading-progress" aria-hidden>
          <span />
        </div>
        <p>{label}</p>
      </section>
    </main>
  );
}
