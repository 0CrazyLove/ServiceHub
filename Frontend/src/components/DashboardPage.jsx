import React from 'react';
import { ToastProvider } from '../contexts/ToastContext.jsx';
import { CartProvider } from '../contexts/CartContext.jsx';
import Header from './Header.jsx';
import Dashboard from './Dashboard.jsx';
import Footer from './Footer.jsx';

/**
 * DashboardPage component - Main admin dashboard layout container.
 * 
 * Provides the complete page structure for the admin dashboard including:
 * - Toast notification context for feedback messages
 * - Shopping cart context for product state management
 * - Header navigation bar
 * - Admin dashboard statistics and user orders display
 * - Application footer
 * 
 * Serves as the root wrapper for the dashboard view with all necessary
 * context providers and layout components.
 * 
 * @returns {JSX.Element} Complete dashboard page with providers and layout
 */
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