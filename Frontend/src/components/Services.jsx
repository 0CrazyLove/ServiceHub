import React from 'react';

/**
 * Service categories showcase component.
 * 
 * Displays a grid of service categories available on the platform.
 * Each category shows name and brief description.
 * Responsive layout adapts to different screen sizes.
 * 
 * @returns {JSX.Element} Service categories grid
 */

const services = [
  { name: 'Desarrollo web', description: 'Crea tu sitio profesional.' },
  { name: 'Diseño gráfico', description: 'Logotipos, banners y más.' },
  { name: 'Marketing digital', description: 'Llega a más clientes.' },
  {
    name: 'Redacción y traducción',
    description: 'Contenido de calidad.'
  },
  { name: 'Video y gráficos en movimiento', description: 'Videos impactantes.' },
  { name: 'Música y audio', description: 'Producción de audio profesional.' }
];

export default function Services() {
  return (
    <section className="py-20 bg-primary-darkest">
      <div className="container mx-auto px-4">
        <h3 className="text-3xl font-bold text-center text-primary-lightest mb-12">
          Categorías de servicios
        </h3>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          {services.map((service) => (
            <div
              key={service.name}
              className="bg-primary-dark rounded-lg shadow-md p-6 text-center"
            >
              <h4 className="text-xl font-semibold text-primary-lightest mb-2">
                {service.name}
              </h4>
              <p className="text-primary-light">{service.description}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
