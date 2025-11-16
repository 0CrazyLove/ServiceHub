import { useState, useEffect } from 'react';

export function useAuth() {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(null);
  const [loading, setLoading] = useState(true);
  const [mounted, setMounted] = useState(false);

  // 🔥 Función corregida para decodificar JWT con soporte UTF-8
  const decodeToken = (token) => {
    try {
      const parts = token.split('.');
      if (parts.length === 3) {
        const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
        
        // 🔥 Decodificar correctamente UTF-8
        const jsonPayload = decodeURIComponent(
          atob(base64)
            .split('')
            .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
            .join('')
        );
        
        return JSON.parse(jsonPayload);
      }
    } catch (error) {
      console.error('Error decodificando token:', error);
    }
    return null;
  };

  // Solo ejecutar en el cliente
  useEffect(() => {
    setMounted(true);
    const storedToken = localStorage.getItem('authToken');
    const storedUser = localStorage.getItem('authUser');
    
    if (storedToken && storedUser) {
      const parsedUser = JSON.parse(storedUser);
      
      // Extraer información adicional del token (como la imagen de perfil)
      const tokenPayload = decodeToken(storedToken);
      if (tokenPayload) {
        // Agregar claims del token al objeto user
        parsedUser.picture = tokenPayload.picture;
        parsedUser.displayName = tokenPayload.display_name;
        
        // 🔥 Log para verificar
        console.log('Display name decodificado:', parsedUser.displayName);
      }
      
      setToken(storedToken);
      setUser(parsedUser);
    }
    setLoading(false);
  }, []);

  const login = (userData, authToken) => {
    // Extraer información adicional del token
    const tokenPayload = decodeToken(authToken);
    if (tokenPayload) {
      userData.picture = tokenPayload.picture;
      userData.displayName = tokenPayload.display_name;
      
      // 🔥 Log para verificar
      console.log('Display name al hacer login:', userData.displayName);
    }
    
    setToken(authToken);
    setUser(userData);
    localStorage.setItem('authToken', authToken);
    localStorage.setItem('authUser', JSON.stringify(userData));
  };

  const logout = () => {
    setToken(null);
    setUser(null);
    localStorage.removeItem('authToken');
    localStorage.removeItem('authUser');
  };

  const isAuthenticated = !!token && !!user && mounted;

  return { user, token, loading, isAuthenticated, login, logout, mounted };
}