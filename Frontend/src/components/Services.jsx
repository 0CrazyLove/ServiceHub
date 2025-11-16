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
  { name: 'Web Development', description: 'Create your professional website.' },
  { name: 'Graphic Design', description: 'Logos, banners and more.' },
  { name: 'Digital Marketing', description: 'Reach more clients.' },
  {
    name: 'Writing and Translation',
    description: 'Quality content.'
  },
  { name: 'Video and Motion Graphics', description: 'Impactful videos.' },
  { name: 'Music and Audio', description: 'Professional audio production.' }
];

export default function Services() {
  return (
    <section className="py-20 bg-primary-darkest">
      <div className="container mx-auto px-4">
        <h3 className="text-3xl font-bold text-center text-primary-lightest mb-12">
          Service Categories
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
