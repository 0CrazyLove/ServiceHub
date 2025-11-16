import React from 'react';
import { CartProvider } from '../contexts/CartContext';
import { ToastProvider } from '../contexts/ToastContext';
import { GoogleOAuthProvider } from '@react-oauth/google';

/**
 * Root provider component that wraps the entire application.
 * 
 * Combines all necessary context providers for:
 * - Google OAuth authentication
 * - Toast notifications (ToastProvider)
 * - Shopping cart functionality (CartProvider)
 * 
 * Must wrap the root component of the application to make all context available.
 * 
 * @param {Object} props - Component props
 * @param {React.ReactNode} props.children - Child components to wrap with providers
 * @returns {JSX.Element} Provider tree with children
 */
export default function Providers({ children }) {
  // Google OAuth client ID from environment variables
  const GOOGLE_CLIENT_ID = import.meta.env.PUBLIC_GOOGLE_CLIENT_ID;

  return (
    <GoogleOAuthProvider clientId={GOOGLE_CLIENT_ID}>
      <ToastProvider>
        <CartProvider>
          {children}
        </CartProvider>
      </ToastProvider>
    </GoogleOAuthProvider>
  );
}