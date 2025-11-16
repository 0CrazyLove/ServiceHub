import React, { useEffect, useState } from 'react';
import { getOrders } from '../../Services/api.js';
import { useAuth } from '../hooks/useAuth.js';
import { useToast } from '../contexts/ToastContext.jsx';

/**
 * Orders list component for authenticated users.
 * 
 * Displays all orders created by the current user including:
 * - Order ID and date
 * - Total amount
 * - Line items with service names and quantities
 * 
 * Requires authentication to view orders.
 * Displays error message if not authenticated.
 * 
 * @returns {JSX.Element} Authenticated user's orders list
 */
export default function OrdersList() {
  const { token, isAuthenticated, mounted } = useAuth();
  const { push } = useToast();
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  /**
   * Load user's orders from API when component mounts and user is authenticated.
   */
  useEffect(() => {
    if (!mounted) return;
    
    if (!isAuthenticated) {
      setLoading(false);
      setError('Inicia sesión para ver tus órdenes.');
      return;
    }

    const load = async () => {
      try {
        setLoading(true);
        const data = await getOrders(token);
        setOrders(data);
      } catch (err) {
        console.error(err);
        setError('No se pudieron recuperar tus órdenes. Intenta más tarde.');
        push('Error al cargar órdenes', 'error');
      } finally {
        setLoading(false);
      }
    };
    
    load();
  }, [token, isAuthenticated, mounted, push]);

  if (loading)
    return <div className="p-6 text-primary-light">Cargando órdenes...</div>;
  if (error) return <div className="p-6 text-red-400">{error}</div>;

  return (
    <div className="mt-8">
      <h3 className="text-xl font-semibold text-primary-lightest mb-4">
        Tus órdenes
      </h3>
      {orders.length === 0 ? (
        <div className="text-primary-light">
          Aún no tienes órdenes.
        </div>
      ) : (
        <div className="space-y-4">
          {orders.map((o) => (
            <div
              key={o.id}
              className="bg-primary-dark rounded p-4 border border-primary-medium"
            >
              <div className="flex justify-between items-center">
                <div>
                  <div className="text-primary-lightest font-semibold">
                    Order #{o.id}
                  </div>
                  <div className="text-primary-light text-sm">
                    {new Date(o.orderDate).toLocaleString()}
                  </div>
                </div>
                <div className="text-primary-accent font-semibold">
                  ${Number(o.totalAmount).toFixed(2)}
                </div>
              </div>
              <div className="mt-3 text-primary-light text-sm">
                <ul className="space-y-1">
                  {o.orderItems.map((it) => (
                    <li key={it.id}>
                      {it.serviceName} — {it.quantity} ×{' '}
                      ${Number(it.price).toFixed(2)}
                    </li>
                  ))}
                </ul>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}