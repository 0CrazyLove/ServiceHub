import React from 'react';

/**
 * Modern Hero component.
 * 
 * Displays a visually appealing banner with a background image,
 * large headline, and call-to-action.
 * 
 * @returns {JSX.Element} Hero section
 */
export default function Hero() {
  return (
    <section className="relative bg-primary-darkest text-primary-lightest min-h-[600px] flex items-center overflow-hidden">
      {/* Background Image with Overlay */}
      <div className="absolute inset-0 z-0">
        <img
          src="https://images.unsplash.com/photo-1600880292203-757bb62b4baf?ixlib=rb-4.0.3&auto=format&fit=crop&w=1920&q=80"
          alt="Office collaboration"
          className="w-full h-full object-cover opacity-40"
        />
        <div className="absolute inset-0 bg-gradient-to-r from-primary-darkest via-primary-darkest/80 to-transparent"></div>
      </div>

      {/* Content */}
      <div className="container mx-auto px-4 relative z-10">
        <div className="max-w-3xl">
          <h1 className="text-5xl md:text-6xl font-bold leading-tight mb-6">
            Transforma tus ideas en <span className="text-primary-accent">realidad</span>
          </h1>
          <p className="text-xl text-primary-light mb-8 max-w-2xl leading-relaxed">
            Accede a una red global de talento freelance. Desde desarrollo web hasta marketing digital, encuentra al experto perfecto para tu próximo proyecto.
          </p>
          <div className="flex flex-col sm:flex-row gap-4">
            <button className="px-8 py-4 bg-primary-accent text-white rounded-lg font-semibold text-lg hover:bg-opacity-90 transition-all shadow-lg hover:shadow-primary-accent/30 transform hover:-translate-y-1">
              Comenzar ahora
            </button>
            <button className="px-8 py-4 bg-primary-medium/30 text-white rounded-lg font-semibold text-lg hover:bg-primary-medium/50 transition-all backdrop-blur-sm border border-primary-light/10">
              Ver cómo funciona
            </button>
          </div>

          {/* Stats */}
          <div className="mt-12 flex gap-8 border-t border-primary-light/10 pt-8">
            <div>
              <p className="text-3xl font-bold text-white">50k+</p>
              <p className="text-sm text-primary-light">Freelancers</p>
            </div>
            <div>
              <p className="text-3xl font-bold text-white">100k+</p>
              <p className="text-sm text-primary-light">Proyectos</p>
            </div>
            <div>
              <p className="text-3xl font-bold text-white">4.9/5</p>
              <p className="text-sm text-primary-light">Valoración</p>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
