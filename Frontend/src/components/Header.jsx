import React, { useEffect, useState } from 'react';
import { useAuth } from '../hooks/useAuth.js';
import CartIcon from './CartIcon.jsx';

export default function Header() {
  const { user, isAuthenticated, logout, mounted } = useAuth();
  const [showAdmin, setShowAdmin] = useState(false);

  const handleLogout = () => {
    logout();
    window.location.href = '/';
  };

  // Determina si el usuario tiene rol admin
  const isUserAdmin = () => {
    try {
      const roles = user?.roles || user?.Roles || user?.RolesList || [];
      if (Array.isArray(roles) && roles.length > 0) {
        const normalized = roles.map(r => String(r).toLowerCase());
        return normalized.includes('admin') || normalized.includes('administrator');
      }

      const token = user?.token || user?.Token || null;
      if (token && typeof token === 'string') {
        const parts = token.split('.');
        if (parts.length === 3) {
          const payload = JSON.parse(atob(parts[1].replace(/-/g, '+').replace(/_/g, '/')));

          if (payload.role) {
            const r = Array.isArray(payload.role) ? payload.role : [payload.role];
            if (r.map(x => String(x).toLowerCase()).includes('admin')) return true;
          }
          if (payload.roles) {
            const r = Array.isArray(payload.roles) ? payload.roles : [payload.roles];
            if (r.map(x => String(x).toLowerCase()).includes('admin')) return true;
          }

          const roleClaimKey = Object.keys(payload).find(k => k.toLowerCase().includes('/role'));
          if (roleClaimKey) {
            const val = payload[roleClaimKey];
            const r = Array.isArray(val) ? val : [val];
            if (r.map(x => String(x).toLowerCase()).includes('admin')) return true;
          }
        }
      }
    } catch (err) {
      // ignore parse errors
    }
    return false;
  };

  useEffect(() => {
    if (!mounted) return;
    setShowAdmin(isUserAdmin());
  }, [mounted, user]);

  // Obtener nombre a mostrar
  const getDisplayName = () => {
    return user?.displayName || user?.Username || user?.username || user?.userName || user?.email?.split('@')[0] || 'Usuario';
  };

  // SKELETON con la MISMA estructura que el componente montado
  if (!mounted) {
    return (
      <header className="bg-primary-dark shadow-md">
        <div className="px-4 py-4 flex items-center gap-8">
          <a href="/" className="text-2xl font-bold text-primary-lightest hover:text-primary-accent">
            ServiceHub
          </a>
          <nav className="flex gap-1">
            <a href="/" className="text-primary-light hover:text-primary-lightest px-3 py-2">Inicio</a>
            <a href="#" className="text-primary-light hover:text-primary-lightest px-3 py-2">Servicios</a>
            <a href="#" className="text-primary-light hover:text-primary-lightest px-3 py-2">Acerca de</a>
          </nav>
          <div className="flex items-center gap-3 ml-auto">
            <div className="w-6 h-6"></div>
            <a>
              href="/login"
              className="text-primary-light hover:text-primary-lightest px-3 py-2"
              Iniciar Sesión
            </a>

            <a
              href="/registro"
              className="bg-primary-accent hover:bg-opacity-80 text-white px-4 py-2 rounded-md transition"
            >
              Registrarse
            </a>

          </div>
        </div >
      </header >
    );
  }

  // Componente completamente montado
  return (
    <header className="bg-primary-dark shadow-md">
      <div className="px-4 py-4 flex items-center gap-8">
        <a href="/" className="text-2xl font-bold text-primary-lightest hover:text-primary-accent">
          ServiceHub
        </a>
        <nav className="flex gap-1">
          <a href="/" className="text-primary-light hover:text-primary-lightest px-3 py-2">Inicio</a>
          <a href="#" className="text-primary-light hover:text-primary-lightest px-3 py-2">Servicios</a>
          <a href="#" className="text-primary-light hover:text-primary-lightest px-3 py-2">Acerca de</a>
          {showAdmin && (
            <a href="/dashboard" className="text-primary-light hover:text-primary-lightest px-3 py-2">Dashboard</a>
          )}
        </nav>
        <div className="flex items-center gap-3 ml-auto">
          <CartIcon />
          {isAuthenticated ? (
            <>
              <div className="flex items-center gap-2">
                {/* Imagen de perfil */}
                {user?.picture ? (
                  <img
                    src={user.picture}
                    alt={getDisplayName()}
                    className="w-8 h-8 rounded-full border-2 border-primary-accent object-cover"
                    referrerPolicy="no-referrer"
                  />
                ) : (
                  <div className="w-8 h-8 rounded-full bg-primary-accent flex items-center justify-center text-primary-lightest font-bold">
                    {getDisplayName().charAt(0).toUpperCase()}
                  </div>
                )}
                <span className="text-primary-light">
                  <strong>{getDisplayName()}</strong>
                </span>
              </div>
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
      </div >
    </header >
  );
}