import React from 'react';

export default function Footer() {
  return (
    <footer className="bg-gray-800 text-white py-8">
      <div className="container mx-auto px-4">
        <div className="flex justify-between">
          <div>
            <h4 className="font-bold mb-2">ServiceHub</h4>
            <p className="text-sm">&copy; 2025 ServiceHub. Todos los derechos reservados.</p>
          </div>
          <div>
            <h4 className="font-bold mb-2">Navegación</h4>
            <ul>
              <li><a href="#" className="hover:text-red-500">Servicios</a></li>
              <li><a href="#" className="hover:text-red-500">Acerca de</a></li>
              <li><a href="#" className="hover:text-red-500">Contacto</a></li>
            </ul>
          </div>
          <div>
            <h4 className="font-bold mb-2">Legal</h4>
            <ul>
              <li><a href="#" className="hover:text-red-500">Términos y Condiciones</a></li>
              <li><a href="#" className="hover:text-red-500">Política de Privacidad</a></li>
            </ul>
          </div>
        </div>
      </div>
    </footer>
  );
}
