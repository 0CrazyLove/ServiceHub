import React, { useState, useEffect } from 'react';
import { useAuth } from '../hooks/useAuth.js';
import { createOrder } from '../../Services/api.js';

/**
 * Shopping cart checkout component.
 * 
 * Displays cart items from localStorage with options to:
 * - Remove individual items
 * - Clear entire cart
 * - Complete checkout (creates order and clears cart)
 * 
 * Syncs with localStorage changes across tabs/windows.
 * Requires authentication to complete checkout.
 * 
 * @returns {JSX.Element} Checkout cart display
 */
export default function Checkout() {
  const [mounted, setMounted] = useState(false);
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(false);

  const { token, isAuthenticated } = useAuth();

  /**
   * Load cart from localStorage and listen for changes.
   */
  useEffect(() => {
    setMounted(true);

    // Load cart from localStorage directly
    const loadCart = () => {
      try {
        const raw = localStorage.getItem('servicehub_cart_v1');
        if (raw) {
          const cartItems = JSON.parse(raw);
          setItems(cartItems);
        }
      } catch (e) {
        console.error('Error loading cart:', e);
      }
    };

    loadCart();

    // Listen to storage changes from other tabs or windows
    const handleStorageChange = () => {
      loadCart();
    };
    window.addEventListener('storage', handleStorageChange);

    return () => window.removeEventListener('storage', handleStorageChange);
  }, []);

  /**
   * Remove a single item from the cart.
   * 
   * @param {number} id - Service ID to remove
   */
  const handleRemove = (id) => {
    try {
      const raw = localStorage.getItem('servicehub_cart_v1') || '[]';
      const cartItems = JSON.parse(raw);
      const filtered = cartItems.filter((p) => p.id !== id);
      localStorage.setItem('servicehub_cart_v1', JSON.stringify(filtered));
      setItems(filtered);
    } catch (e) {
      console.error('Error removing item:', e);
    }
  };

  /**
   * Clear all items from the cart.
   */
  const handleClear = () => {
    try {
      localStorage.setItem('servicehub_cart_v1', JSON.stringify([]));
      setItems([]);
    } catch (e) {
      console.error('Error clearing cart:', e);
    }
  };

  /**
   * Complete checkout by creating an order.
   * Redirects to login if not authenticated.
   * Clears cart on success and redirects to dashboard.
   */
  const handleCheckout = async () => {
    if (!isAuthenticated) {
      window.location.href = '/login';
      return;
    }
    try {
      setLoading(true);
      
      // Create order from current cart items
      const orderDto = {
        orderItems: items.map((it) => ({ serviceId: it.id, quantity: 1 }))
      };
      
      const created = await createOrder(orderDto, token);
      
      // Clear cart
      localStorage.setItem('servicehub_cart_v1', JSON.stringify([]));
      setItems([]);
      
      // Redirect to dashboard
      window.location.href = '/dashboard';
    } catch (err) {
      console.error(err);
      alert('Error al crear la orden. Por favor intenta de nuevo.');
    } finally {
      setLoading(false);
    }
  };

  if (!mounted) {
    return (
      <div className="p-6 text-center text-primary-light">
        Cargando carrito...
      </div>
    );
  }

  // Calculate total
  const total = items.reduce((sum, item) => sum + item.price, 0);

  return (
    <div className="bg-primary-dark rounded-lg p-6">
      <h2 className="text-2xl font-semibold text-primary-lightest mb-4">
        Carrito
      </h2>
      {items.length === 0 ? (
        <div className="text-primary-light">Tu carrito está vacío.</div>
      ) : (
        <div className="space-y-4">
          {items.map((it) => (
            <div
              key={it.id}
              className="flex items-center justify-between bg-primary-darkest p-3 rounded"
            >
              <div className="flex items-center gap-3">
                {it.imageUrl ? (
                  <img
                    src={it.imageUrl}
                    alt={it.name}
                    className="w-16 h-16 object-cover rounded"
                  />
                ) : (
                  <div className="w-16 h-16 bg-primary-dark rounded" />
                )}
                <div>
                  <div className="text-primary-lightest font-semibold">
                    {it.name}
                  </div>
                  <div className="text-primary-light text-sm">
                    {it.provider}
                  </div>
                </div>
              </div>

              <div className="text-right">
                <div className="text-primary-accent font-semibold">
                  ${Number(it.price).toFixed(2)}
                </div>
                <div className="mt-2 flex gap-2 justify-end">
                  <button
                    onClick={() => handleRemove(it.id)}
                    className="px-3 py-1 rounded bg-red-600 text-white hover:bg-red-700"
                  >
                    Eliminar
                  </button>
                </div>
              </div>
            </div>
          ))}

          {/* Cart Summary */}
          <div className="pt-4 border-t border-primary-medium">
            <div className="flex justify-between items-center mb-4">
              <span className="text-primary-lightest font-semibold">
                Total:
              </span>
              <span className="text-2xl font-bold text-primary-accent">
                ${total.toFixed(2)}
              </span>
            </div>

            <div className="flex justify-between items-center gap-3">
              <button
                onClick={handleClear}
                className="px-4 py-2 bg-primary-darkest border border-primary-medium rounded text-primary-light hover:bg-primary-medium"
              >
                Vaciar
              </button>
              <button
                onClick={handleCheckout}
                disabled={loading}
                className="px-4 py-2 bg-primary-accent text-white rounded hover:bg-opacity-80 disabled:opacity-50"
              >
                {loading ? 'Procesando...' : 'Pagar'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
