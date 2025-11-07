import React from 'react';

export default function Hero() {
  return (
    <section className="bg-gray-900 text-white">
      <div className="container mx-auto px-4 py-20 text-center">
        <h2 className="text-4xl font-bold mb-4">Encuentra los mejores servicios</h2>
        <p className="text-lg mb-8">Conectando profesionales con clientes en todo el mundo</p>
        <div className="flex justify-center">
          <input type="text" placeholder="¿Qué servicio estás buscando?" className="w-1/2 px-4 py-2 rounded-l-md bg-gray-800 text-white border border-gray-700 focus:outline-none focus:border-red-500" />
          <button className="bg-red-500 hover:bg-red-600 text-white px-6 py-2 rounded-r-md font-semibold">Buscar</button>
        </div>
      </div>
    </section>
  );
}
