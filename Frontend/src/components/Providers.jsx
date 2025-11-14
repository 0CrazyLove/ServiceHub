import React from 'react';
import { ToastProvider } from '../contexts/ToastContext.jsx';
import { CartProvider } from '../contexts/CartContext.jsx';

export default function Providers({ children }) {
  return (
    <ToastProvider>
      <CartProvider>
        {children}
      </CartProvider>
    </ToastProvider>
  );
}
