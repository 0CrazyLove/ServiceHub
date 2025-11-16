import { useState, useEffect } from 'react';

/**
 * Custom React hook for authentication state management.
 * 
 * Manages user authentication state, JWT token handling, and client-side login/logout.
 * Decodes JWT tokens to extract user claims including profile information from Google OAuth.
 * 
 * Persists authentication state to localStorage for session restoration across page reloads.
 * Handles client-side hydration with mounted state to prevent SSR mismatches.
 * 
 * @returns {Object} Authentication state and methods
 * @returns {Object|null} user - Authenticated user object with email, roles, and OAuth profile data
 * @returns {string|null} token - JWT bearer token for API requests
 * @returns {boolean} loading - Whether auth state is still being initialized
 * @returns {boolean} isAuthenticated - Whether user is currently logged in
 * @returns {boolean} mounted - Whether component has mounted on client
 * @returns {Function} login - Function to set user and token after successful login
 * @returns {Function} logout - Function to clear authentication state
 */
export function useAuth() {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(null);
  const [loading, setLoading] = useState(true);
  const [mounted, setMounted] = useState(false);

  /**
   * Decode JWT token to extract payload claims.
   * 
   * Handles UTF-8 decoding for international characters in user information.
   * Supports standard JWT structure (header.payload.signature).
   * 
   * @param {string} token - JWT token to decode
   * @returns {Object|null} Decoded token payload or null if invalid
   * @private
   */
  const decodeToken = (token) => {
    try {
      const parts = token.split('.');
      if (parts.length === 3) {
        const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
        
        // Decode UTF-8 characters properly
        const jsonPayload = decodeURIComponent(
          atob(base64)
            .split('')
            .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
            .join('')
        );
        
        return JSON.parse(jsonPayload);
      }
    } catch (error) {
      console.error('Error decoding JWT token:', error);
    }
    return null;
  };

  /**
   * Initialize authentication state from localStorage on client mount.
   * 
   * Restores stored JWT token and user data, decodes token claims to get
   * profile picture and display name from Google OAuth integration.
   */
  useEffect(() => {
    setMounted(true);
    const storedToken = localStorage.getItem('authToken');
    const storedUser = localStorage.getItem('authUser');
    
    if (storedToken && storedUser) {
      const parsedUser = JSON.parse(storedUser);
      
      // Extract additional information from JWT token
      const tokenPayload = decodeToken(storedToken);
      if (tokenPayload) {
        // Add OAuth claims to user object
        parsedUser.picture = tokenPayload.picture;
        parsedUser.displayName = tokenPayload.display_name;
      }
      
      setToken(storedToken);
      setUser(parsedUser);
    }
    setLoading(false);
  }, []);

  /**
   * Store user credentials and JWT token after successful authentication.
   * 
   * Extracts user profile data from JWT token and merges with user object,
   * then persists both to localStorage for session restoration.
   * 
   * @param {Object} userData - User information from auth response
   * @param {string} authToken - JWT authentication token
   */
  const login = (userData, authToken) => {
    // Extract additional information from token
    const tokenPayload = decodeToken(authToken);
    if (tokenPayload) {
      userData.picture = tokenPayload.picture;
      userData.displayName = tokenPayload.display_name;
    }
    
    setToken(authToken);
    setUser(userData);
    localStorage.setItem('authToken', authToken);
    localStorage.setItem('authUser', JSON.stringify(userData));
  };

  /**
   * Clear authentication state and remove stored credentials.
   */
  const logout = () => {
    setToken(null);
    setUser(null);
    localStorage.removeItem('authToken');
    localStorage.removeItem('authUser');
  };

  /**
   * Compute authentication status.
   * Only returns true after component has mounted to prevent SSR hydration mismatches.
   */
  const isAuthenticated = !!token && !!user && mounted;

  return { user, token, loading, isAuthenticated, login, logout, mounted };
}