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

  // Calculate total
  const total = items.reduce((sum, item) => sum + item.price, 0);

  if (!mounted) {
    return (
      <div className="min-h-[400px] flex items-center justify-center">
        <div className="animate-pulse flex flex-col items-center gap-4">
          <div className="h-12 w-12 rounded-full border-4 border-primary-medium border-t-primary-accent animate-spin"></div>
          <p className="text-primary-light font-medium">Cargando tu carrito...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div className="flex items-center justify-between mb-8">
        <h2 className="text-3xl font-bold text-primary-lightest tracking-tight">
          Tu Carrito
          <span className="ml-3 text-lg font-medium text-primary-light bg-primary-dark px-3 py-1 rounded-full">
            {items.length} {items.length === 1 ? 'item' : 'items'}
          </span>
        </h2>
        <button
          onClick={() => window.location.href = '/services'}
          className="text-primary-accent hover:text-primary-lightest transition-colors font-medium flex items-center gap-2"
        >
          ← Seguir comprando
        </button>
      </div>

      {items.length === 0 ? (
        <div className="bg-primary-dark rounded-2xl p-12 text-center border border-primary-medium/30 shadow-xl">
          <div className="w-24 h-24 bg-primary-medium/20 rounded-full flex items-center justify-center mx-auto mb-6">
            <svg xmlns="http://www.w3.org/2000/svg" className="h-10 w-10 text-primary-light" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
            </svg>
          </div>
          <h3 className="text-xl font-semibold text-primary-lightest mb-2">Tu carrito está vacío</h3>
          <p className="text-primary-light mb-8 max-w-md mx-auto">
            Parece que aún no has agregado ningún servicio. Explora nuestro catálogo para encontrar lo que necesitas.
          </p>
          <button
            onClick={() => window.location.href = '/services'}
            className="px-8 py-3 bg-primary-accent text-white rounded-xl font-medium hover:bg-opacity-90 transition-all shadow-lg hover:shadow-primary-accent/20"
          >
            Explorar Servicios
          </button>
        </div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
          {/* Cart Items List */}
          <div className="lg:col-span-8 space-y-4">
            {items.map((it) => (
              <div
                key={it.id}
                className="group bg-primary-dark rounded-xl p-4 sm:p-6 border border-primary-medium/30 hover:border-primary-accent/50 transition-all duration-300 shadow-sm hover:shadow-md flex flex-col sm:flex-row gap-6 items-start sm:items-center"
              >
                <div className="relative flex-shrink-0">
                  {it.imageUrl ? (
                    <img
                      src={it.imageUrl}
                      alt={it.name}
                      className="w-24 h-24 sm:w-32 sm:h-32 object-cover rounded-lg shadow-inner"
                    />
                  ) : (
                    <div className="w-24 h-24 sm:w-32 sm:h-32 bg-primary-medium/30 rounded-lg flex items-center justify-center text-primary-light">
                      <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8 opacity-50" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                      </svg>
                    </div>
                  )}
                </div>

                <div className="flex-grow min-w-0">
                  <div className="flex justify-between items-start mb-2">
                    <h3 className="text-lg font-semibold text-primary-lightest truncate pr-4">
                      {it.name}
                    </h3>
                    <span className="text-lg font-bold text-primary-lightest">
                      ${Number(it.price).toFixed(2)}
                    </span>
                  </div>

                  <p className="text-primary-light text-sm mb-4 flex items-center gap-2">
                    <span className="w-2 h-2 rounded-full bg-primary-accent"></span>
                    {it.provider}
                  </p>

                  <div className="flex items-center justify-between mt-4 pt-4 border-t border-primary-medium/30">
                    <button
                      onClick={() => handleRemove(it.id)}
                      className="text-sm text-red-400 hover:text-red-300 transition-colors flex items-center gap-1.5 group/btn"
                    >
                      <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4 group-hover/btn:scale-110 transition-transform" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                      </svg>
                      Eliminar
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Order Summary Sidebar */}
          <div className="lg:col-span-4">
            <div className="bg-primary-dark rounded-2xl p-6 border border-primary-medium/30 shadow-lg sticky top-8">
              <h3 className="text-xl font-semibold text-primary-lightest mb-6">Resumen de Orden</h3>

              <div className="space-y-3 mb-6">
                <div className="flex justify-between text-primary-light">
                  <span>Subtotal</span>
                  <span>${total.toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-primary-light">
                  <span>Impuestos est.</span>
                  <span>$0.00</span>
                </div>
                <div className="h-px bg-primary-medium/50 my-4"></div>
                <div className="flex justify-between items-end">
                  <span className="text-primary-lightest font-medium">Total</span>
                  <span className="text-3xl font-bold text-primary-accent">${total.toFixed(2)}</span>
                </div>
              </div>

              <button
                onClick={handleCheckout}
                disabled={loading}
                className="w-full py-4 bg-primary-accent text-white rounded-xl font-bold text-lg hover:bg-opacity-90 disabled:opacity-50 disabled:cursor-not-allowed transition-all shadow-lg hover:shadow-primary-accent/25 mb-4 flex justify-center items-center gap-2"
              >
                {loading ? (
                  <>
                    <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    Procesando...
                  </>
                ) : (
                  'Proceder al Pago'
                )}
              </button>

              <button
                onClick={handleClear}
                className="w-full py-2 text-primary-light hover:text-red-400 text-sm transition-colors border border-transparent hover:border-red-400/20 rounded-lg"
              >
                Vaciar Carrito
              </button>

              <div className="mt-6 flex items-center justify-center gap-2 text-primary-light/60 text-xs">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                </svg>
                Checkout Seguro
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
