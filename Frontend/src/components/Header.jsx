import React from 'react';
import { useAuth } from '../hooks/useAuth';

export default function Header() {
  const { user, isAuthenticated, logout, mounted } = useAuth();

  const handleLogout = () => {
    logout();
    window.location.href = '/';
  };

  // No renderizar hasta que el hook esté montado
  if (!mounted) {
    return (
      <header className="bg-primary-dark shadow-md">
        <div className="container mx-auto px-4 py-4 flex justify-between items-center">
          <a href="/" className="text-2xl font-bold text-primary-lightest hover:text-primary-accent">
            ServiceHub
          </a>
          <nav>
            <a href="/" className="text-primary-light hover:text-primary-lightest px-3 py-2">Inicio</a>
            <a href="#" className="text-primary-light hover:text-primary-lightest px-3 py-2">Servicios</a>
            <a href="#" className="text-primary-light hover:text-primary-lightest px-3 py-2">Acerca de</a>
          </nav>
          <div className="flex items-center gap-3">
            <a 
              href="/login" 
              className="text-primary-light hover:text-primary-lightest px-3 py-2"
            >
              Iniciar Sesión
            </a>
            <a 
              href="/registro" 
              className="bg-primary-accent hover:bg-opacity-80 text-white px-4 py-2 rounded-md transition"
            >
              Registrarse
            </a>
          </div>
        </div>
      </header>
    );
  }

  return (
    <header className="bg-primary-dark shadow-md">
      <div className="container mx-auto px-4 py-4 flex justify-between items-center">
        <a href="/" className="text-2xl font-bold text-primary-lightest hover:text-primary-accent">
          ServiceHub
        </a>
        <nav>
          <a href="/" className="text-primary-light hover:text-primary-lightest px-3 py-2">Inicio</a>
          <a href="#" className="text-primary-light hover:text-primary-lightest px-3 py-2">Servicios</a>
          <a href="#" className="text-primary-light hover:text-primary-lightest px-3 py-2">Acerca de</a>
        </nav>
        <div className="flex items-center gap-3">
          {isAuthenticated ? (
            <>
              <span className="text-primary-light">
                <strong>{user?.username || user?.userName}</strong>
              </span>
              <button
                onClick={handleLogout}
                className="bg-primary-accent hover:bg-opacity-80 text-white px-4 py-2 rounded-md transition"
              >
                Cerrar Sesión
              </button>
            </>
          ) : (
            <>
              <a 
                href="/login" 
                className="text-primary-light hover:text-primary-lightest px-3 py-2"
              >
                Iniciar Sesión
              </a>
              <a 
                href="/registro" 
                className="bg-primary-accent hover:bg-opacity-80 text-white px-4 py-2 rounded-md transition"
              >
                Registrarse
              </a>
            </>
          )}
        </div>
      </div>
    </header>
  );
}