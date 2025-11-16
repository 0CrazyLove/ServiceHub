import React, { useEffect, useState, useRef } from 'react';
import { useAuth } from '../hooks/useAuth.js';
import CartIcon from './CartIcon.jsx';

export default function Header() {
  const { user, isAuthenticated, logout, mounted } = useAuth();
  const [showAdmin, setShowAdmin] = useState(false);
  const [showProfileMenu, setShowProfileMenu] = useState(false);
  const menuRef = useRef(null);

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

  // Obtener email
  const getEmail = () => {
    return user?.email || user?.Email || 'email@ejemplo.com';
  };

  // Cerrar menú al hacer click fuera
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (menuRef.current && !menuRef.current.contains(event.target)) {
        setShowProfileMenu(false);
      }
    };

    if (showProfileMenu) {
      document.addEventListener('mousedown', handleClickOutside);
    }

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [showProfileMenu]);

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
            <div className="relative" ref={menuRef}>
              {/* Botón de perfil */}
              <button
                onClick={() => setShowProfileMenu(!showProfileMenu)}
                className="flex items-center gap-2 hover:opacity-80 transition focus:outline-none focus:ring-2 focus:ring-primary-accent rounded-full"
              >
                {user?.picture ? (
                  <img
                    src={user.picture}
                    alt={getDisplayName()}
                    className="w-10 h-10 rounded-full border-2 border-primary-accent object-cover"
                    referrerPolicy="no-referrer"
                  />
                ) : (
                  <div className="w-10 h-10 rounded-full bg-primary-accent flex items-center justify-center text-primary-lightest font-bold text-lg">
                    {getDisplayName().charAt(0).toUpperCase()}
                  </div>
                )}
              </button>

              {/* Menú desplegable */}
              {showProfileMenu && (
                <div className="absolute right-0 mt-2 w-64 bg-primary-dark border border-primary-light/20 rounded-lg shadow-xl z-50 overflow-hidden">
                  {/* Header del menú con foto y nombre */}
                  <div className="bg-primary-darker p-4 border-b border-primary-light/10">
                    <div className="flex items-center gap-3">
                      {user?.picture ? (
                        <img
                          src={user.picture}
                          alt={getDisplayName()}
                          className="w-12 h-12 rounded-full border-2 border-primary-accent object-cover"
                          referrerPolicy="no-referrer"
                        />
                      ) : (
                        <div className="w-12 h-12 rounded-full bg-primary-accent flex items-center justify-center text-primary-lightest font-bold text-xl">
                          {getDisplayName().charAt(0).toUpperCase()}
                        </div>
                      )}
                      <div className="flex-1 min-w-0">
                        <p className="text-primary-lightest font-semibold truncate">
                          {getDisplayName()}
                        </p>
                        <p className="text-primary-light text-sm truncate">
                          {getEmail()}
                        </p>
                      </div>
                    </div>
                  </div>

                  {/* Opciones del menú */}
                  <div className="py-2">
                    <button
                      onClick={handleLogout}
                      className="w-full text-left px-4 py-3 text-primary-light hover:bg-primary-darker hover:text-primary-lightest transition flex items-center gap-2"
                    >
                      <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                      </svg>
                      Cerrar Sesión
                    </button>
                  </div>
                </div>
              )}
            </div>
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