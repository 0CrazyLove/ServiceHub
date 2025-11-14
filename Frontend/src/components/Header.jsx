import React, { useEffect, useState } from 'react';
import { useAuth } from '../hooks/useAuth';

export default function Header() {
  const { user, isAuthenticated, logout, mounted } = useAuth();

  const handleLogout = () => {
    logout();
    window.location.href = '/';
  };

  // Determina si el usuario tiene rol admin. Soporta user.roles (array) o token JWT con claim de role.
  const isUserAdmin = () => {
    try {
      const roles = user?.roles || user?.Roles || user?.RolesList || [];
      if (Array.isArray(roles) && roles.length > 0) {
        const normalized = roles.map(r => String(r).toLowerCase());
        return normalized.includes('admin') || normalized.includes('administrator');
      }

      // Fallback: si hay token, decodificar payload JWT y buscar claims de role
      const token = user?.token || user?.Token || null;
      if (token && typeof token === 'string') {
        const parts = token.split('.');
        if (parts.length === 3) {
          const payload = JSON.parse(atob(parts[1].replace(/-/g, '+').replace(/_/g, '/')));
          // posibles claves: 'role', 'roles', ClaimTypes.Role full uri
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

  // Guardar en estado para evitar evaluación durante SSR / render temprano
  const [showAdmin, setShowAdmin] = useState(false);

  useEffect(() => {
    if (mounted) {
      setShowAdmin(isUserAdmin());
    }
  }, [mounted, user]);

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
          {/* Mostrar enlace al Dashboard solo para admins. */}
          {showAdmin && (
            <a href="/dashboard" className="text-primary-light hover:text-primary-lightest px-3 py-2">Dashboard</a>
          )}
        </nav>
        
        <div className="flex items-center gap-3">
          {isAuthenticated ? (
            <>
              <span className="text-primary-light flex items-center gap-2">
                <strong>{user?.Username || user?.username || user?.userName || user?.email}</strong>
                {/* admin badge removed */}
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