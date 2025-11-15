import React from 'react';
import { ToastProvider } from '../contexts/ToastContext.jsx';
import { CartProvider } from '../contexts/CartContext.jsx';
import Header from './Header.jsx';
import Dashboard from './Dashboard.jsx';
import Footer from './Footer.jsx';

export default function DashboardPage() {
  return (
    <ToastProvider>
      <CartProvider>
        <Header />
        <main className="bg-primary-darkest min-h-screen">
          <Dashboard />
        </main>
        <Footer />
      </CartProvider>
    </ToastProvider>
  );
}