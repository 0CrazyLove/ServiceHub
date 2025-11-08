import React from 'react';

const services = [
  { name: 'Desarrollo Web', description: 'Crea tu sitio web profesional.' },
  { name: 'Diseño Gráfico', description: 'Logos, banners y más.' },
  { name: 'Marketing Digital', description: 'Llega a más clientes.' },
  { name: 'Redacción y Traducción', description: 'Contenido de calidad.' },
  { name: 'Video y Animación', description: 'Videos que impactan.' },
  { name: 'Música y Audio', description: 'Producción de audio profesional.' },
];

export default function Services() {
  return (
    <section className="py-20 bg-gray-900">
      <div className="container mx-auto px-4">
        <h3 className="text-3xl font-bold text-center text-white mb-12">Categorías de Servicios</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          {services.map((service) => (
            <div key={service.name} className="bg-gray-800 rounded-lg shadow-md p-6 text-center">
              <h4 className="text-xl font-semibold text-white mb-2">{service.name}</h4>
              <p className="text-gray-400">{service.description}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
