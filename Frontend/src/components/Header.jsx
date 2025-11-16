import React, { useEffect, useState, useRef } from 'react';
import { useAuth } from '../hooks/useAuth.js';
import CartIcon from './CartIcon.jsx';

/**
 * Application header and navigation component.
 * 
 * Displays the main navigation bar with logo, menu, cart icon, and user profile menu.
 * Supports both authenticated and unauthenticated states.
 * Shows admin-only dashboard link for users with admin role.
 * Implements profile dropdown menu with logout functionality.
 * 
 * @returns {JSX.Element} Header element with navigation
 */
export default function Header() {
  const { user, isAuthenticated, logout, mounted } = useAuth();
  const [showAdmin, setShowAdmin] = useState(false);
  const [showProfileMenu, setShowProfileMenu] = useState(false);
  const menuRef = useRef(null);

  /**
   * Handle user logout and redirect to home page.
   */
  const handleLogout = () => {
    logout();
    window.location.href = '/';
  };

  /**
   * Check if current user has admin role.
   * 
   * Attempts to determine admin status through multiple methods:
   * 1. Check user object roles property
   * 2. Decode JWT token and look for role claims
   * 3. Search for OpenID Connect role claim URI
   * 
   * @returns {boolean} True if user has admin role
   * @private
   */
  const isUserAdmin = () => {
    try {
      const roles = user?.roles || user?.Roles || user?.RolesList || [];
      if (Array.isArray(roles) && roles.length > 0) {
        const normalized = roles.map((r) => String(r).toLowerCase());
        return (
          normalized.includes('admin') ||
          normalized.includes('administrator')
        );
      }
      const token = user?.token || user?.Token || null;
      if (token && typeof token === 'string') {
        const parts = token.split('.');
        if (parts.length === 3) {
          const payload = JSON.parse(
            atob(parts[1].replace(/-/g, '+').replace(/_/g, '/'))
          );
          if (payload.role) {
            const r = Array.isArray(payload.role)
              ? payload.role
              : [payload.role];
            if (r.map((x) => String(x).toLowerCase()).includes('admin'))
              return true;
          }
          if (payload.roles) {
            const r = Array.isArray(payload.roles)
              ? payload.roles
              : [payload.roles];
            if (r.map((x) => String(x).toLowerCase()).includes('admin'))
              return true;
          }
          const roleClaimKey = Object.keys(payload).find((k) =>
            k.toLowerCase().includes('/role')
          );
          if (roleClaimKey) {
            const val = payload[roleClaimKey];
            const r = Array.isArray(val) ? val : [val];
            if (r.map((x) => String(x).toLowerCase()).includes('admin'))
              return true;
          }
        }
      }
    } catch (err) {
      console.error('Error checking admin status:', err);
    }
    return false;
  };

  /**
   * Update admin status when user or mount state changes.
   */
  useEffect(() => {
    if (!mounted) return;
    setShowAdmin(isUserAdmin());
  }, [mounted, user]);

  /**
   * Extract display name from user object with fallbacks.
   * 
   * @returns {string} User display name or email prefix
   * @private
   */
  const getDisplayName = () => {
    return (
      user?.displayName ||
      user?.Username ||
      user?.username ||
      user?.userName ||
      user?.email?.split('@')[0] ||
      'User'
    );
  };

  /**
   * Extract email from user object with fallback.
   * 
   * @returns {string} User email address
   * @private
   */
  const getEmail = () => {
    return user?.email || user?.Email || 'email@example.com';
  };

  /**
   * Close profile menu when clicking outside.
   */
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

  /**
   * Skeleton/placeholder header while component hydrates on client.
   * Maintains same structure as mounted component to prevent layout shift.
   */
  if (!mounted) {
    return (
      <header className="bg-primary-dark shadow-md">
        <div className="px-4 py-4 flex items-center gap-8">
          <a
            href="/"
            className="text-2xl font-bold text-primary-lightest hover:text-primary-accent"
          >
            ServiceHub
          </a>
          <nav className="flex gap-1">
            <a
              href="/"
              className="text-primary-light hover:text-primary-lightest px-3 py-2"
            >
              Home
            </a>
            <a
              href="#"
              className="text-primary-light hover:text-primary-lightest px-3 py-2"
            >
              Services
            </a>
            <a
              href="#"
              className="text-primary-light hover:text-primary-lightest px-3 py-2"
            >
              About
            </a>
          </nav>
          <div className="flex items-center gap-3 ml-auto">
            <div className="w-6 h-6"></div>
            <a
              href="/login"
              className="text-primary-light hover:text-primary-lightest px-3 py-2"
            >
              Login
            </a>
            <a
              href="/registro"
              className="bg-primary-accent hover:bg-opacity-80 text-white px-4 py-2 rounded-md transition"
            >
              Register
            </a>
          </div>
        </div>
      </header>
    );
  }

  /**
   * Fully mounted header with interactive elements and user state.
   */
  return (
    <header className="bg-primary-dark shadow-md">
      <div className="px-4 py-4 flex items-center gap-8">
        {/* Logo */}
        <a
          href="/"
          className="text-2xl font-bold text-primary-lightest hover:text-primary-accent"
        >
          ServiceHub
        </a>
        {/* Main navigation */}
        <nav className="flex gap-1">
          <a
            href="/"
            className="text-primary-light hover:text-primary-lightest px-3 py-2"
          >
            Home
          </a>
          <a
            href="#"
            className="text-primary-light hover:text-primary-lightest px-3 py-2"
          >
            Services
          </a>
          <a
            href="#"
            className="text-primary-light hover:text-primary-lightest px-3 py-2"
          >
            About
          </a>
          {showAdmin && (
            <a
              href="/dashboard"
              className="text-primary-light hover:text-primary-lightest px-3 py-2"
            >
              Dashboard
            </a>
          )}
        </nav>
        {/* Right side: cart and auth */}
        <div className="flex items-center gap-3 ml-auto">
          <CartIcon />
          {isAuthenticated ? (
            <div className="relative" ref={menuRef}>
              {/* Profile button */}
              <button
                onClick={() => setShowProfileMenu(!showProfileMenu)}
                className="flex items-center gap-2 hover:opacity-80 transition focus:outline-none focus:ring-2 focus:ring-primary-accent rounded-full"
                aria-label="Open profile menu"
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

              {/* Dropdown profile menu */}
              {showProfileMenu && (
                <div className="absolute right-0 mt-2 w-64 bg-primary-dark border border-primary-light/20 rounded-lg shadow-xl z-50 overflow-hidden">
                  {/* Menu header with user info */}
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

                  {/* Menu options */}
                  <div className="py-2">
                    <button
                      onClick={handleLogout}
                      className="w-full text-left px-4 py-3 text-primary-light hover:bg-primary-darker hover:text-primary-lightest transition flex items-center gap-2"
                      aria-label="Logout"
                    >
                      <svg
                        className="w-5 h-5"
                        fill="none"
                        stroke="currentColor"
                        viewBox="0 0 24 24"
                      >
                        <path
                          strokeLinecap="round"
                          strokeLinejoin="round"
                          strokeWidth={2}
                          d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"
                        />
                      </svg>
                      Logout
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
                Login
              </a>
              <a
                href="/registro"
                className="bg-primary-accent hover:bg-opacity-80 text-white px-4 py-2 rounded-md transition"
              >
                Register
              </a>
            </>
          )}
        </div>
      </div>
    </header>
  );
}