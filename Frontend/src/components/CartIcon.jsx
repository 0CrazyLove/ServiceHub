import React, { useState, useEffect } from 'react';

/**
 * Shopping cart icon component with item count badge.
 * 
 * Displays a cart icon in the navigation with a badge showing the number of items.
 * Synchronizes with localStorage to track cart changes across browser tabs.
 * Only renders on client to avoid SSR hydration mismatches.
 * 
 * @returns {JSX.Element|null} Cart icon with count badge or null if not mounted
 */
export default function CartIcon() {
  const [mounted, setMounted] = useState(false);
  const [cartCount, setCartCount] = useState(0);

  /**
   * Initialize cart count from localStorage and listen for storage changes.
   * 
   * Handles cross-tab synchronization by listening to storage events,
   * allowing cart updates in one tab to be reflected in other tabs.
   */
  useEffect(() => {
    setMounted(true);
    
    // Load cart from localStorage directly (no hook needed for SSR compatibility)
    try {
      const raw = localStorage.getItem('servicehub_cart_v1');
      if (raw) {
        const items = JSON.parse(raw);
        setCartCount(Array.isArray(items) ? items.length : 0);
      }
    } catch (e) {
      console.error('Error loading cart count:', e);
    }

    /**
     * Handle storage changes from other tabs/windows.
     */
    const handleStorageChange = () => {
      try {
        const raw = localStorage.getItem('servicehub_cart_v1');
        if (raw) {
          const items = JSON.parse(raw);
          setCartCount(Array.isArray(items) ? items.length : 0);
        } else {
          setCartCount(0);
        }
      } catch (e) {
        console.error('Error updating cart count:', e);
      }
    };

    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, []);

  // Only render on client to prevent hydration mismatches
  if (!mounted) {
    return null;
  }

  return (
    <a
      href="/checkout"
      className="relative text-primary-light hover:text-primary-lightest px-3 py-2 flex items-center gap-1"
      aria-label="Shopping cart"
    >
      🛒 Cart
      {cartCount > 0 && (
        <span className="ml-2 bg-primary-accent text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
          {cartCount}
        </span>
      )}
    </a>
  );
}
