import { useState, useEffect } from 'react';
import { useAuth } from '../hooks/useAuth.js';
import { getDashboardStats } from '../../Services/api.js';
import OrdersList from './OrdersList.jsx';

/**
 * Dashboard component displaying admin statistics and user orders.
 * 
 * Shows key performance indicators:
 * - Total sales revenue
 * - Service count
 * - Order count
 * 
 * Also displays the authenticated user's order history.
 * Requires admin authentication to view dashboard stats.
 * 
 * @returns {JSX.Element} Dashboard with stats and orders
 */
export default function Dashboard() {
  const { token, isAuthenticated, mounted } = useAuth();
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  /**
   * Load dashboard statistics from API when component mounts.
   */
  useEffect(() => {
    if (!mounted) return; // Wait for hook to mount

    const load = async () => {
      if (!isAuthenticated) {
        setError('Debes iniciar sesión con una cuenta de administrador para ver el panel.');
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        setError(null);
        const data = await getDashboardStats(token);
        setStats(data);
      } catch (err) {
        console.error(err);
        setError('No se pudieron cargar las estadísticas. Comprueba que tu sesión tenga permisos de administrador.');
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [token, isAuthenticated, mounted]);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-48">
        <div className="animate-spin rounded-full h-10 w-10 border-4 border-primary-medium border-t-primary-accent"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="bg-red-200 text-red-700 px-4 py-3 rounded-md shadow-md">
          {error}
        </div>
        <div className="mt-4">
          <a href="/login" className="text-primary-accent underline">
            Ir a iniciar sesión
          </a>
        </div>
      </div>
    );
  }

  const totalSales = stats?.totalSales ?? stats?.TotalSales ?? 0;
  const serviceCount = stats?.serviceCount ?? stats?.ServiceCount ?? 0;
  const orderCount = stats?.orderCount ?? stats?.OrderCount ?? 0;

  return (
    <div className="container mx-auto px-4 py-12">
      <h2 className="text-3xl font-bold text-primary-lightest mb-6">
            Panel
          </h2>

      {/* KPI Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-6">
        <div className="bg-primary-dark rounded-lg p-6 shadow-md border border-primary-medium">
          <p className="text-sm text-primary-light">Ventas totales</p>
          <p className="text-2xl font-semibold text-primary-accent mt-2">
            ${Number(totalSales).toFixed(2)}
          </p>
        </div>

        <div className="bg-primary-dark rounded-lg p-6 shadow-md border border-primary-medium">
          <p className="text-sm text-primary-light">Servicios</p>
          <p className="text-2xl font-semibold text-primary-accent mt-2">
            {serviceCount}
          </p>
        </div>

        <div className="bg-primary-dark rounded-lg p-6 shadow-md border border-primary-medium">
          <p className="text-sm text-primary-light">Órdenes</p>
          <p className="text-2xl font-semibold text-primary-accent mt-2">
            {orderCount}
          </p>
        </div>
      </div>
      {/* User orders list */}
      <OrdersList />
    </div>
  );
}

