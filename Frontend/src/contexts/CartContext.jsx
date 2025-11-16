import React, { createContext, useContext, useEffect, useState, useCallback } from 'react';
import { createOrder } from '../../Services/api.js';
import { useToast } from './ToastContext.jsx';

/**
 * Context for managing shopping cart state across the application.
 * 
 * Provides centralized cart management with persistence to localStorage,
 * allowing users to maintain their cart across sessions and page reloads.
 * Integrates with toast notifications for user feedback on cart operations.
 */
const CartContext = createContext(null);

/** Local storage key for persisting cart items */
const STORAGE_KEY = 'servicehub_cart_v1';

/**
 * Cart context provider component.
 * 
 * Manages shopping cart state, handles loading/saving to localStorage,
 * and provides cart operations (add, remove, clear, checkout).
 * Must wrap components that use the useCart hook.
 * 
 * @param {Object} props - Component props
 * @param {React.ReactNode} props.children - Child components that can access cart context
 * @returns {JSX.Element} Provider component
 */
export function CartProvider({ children }) {
  const [items, setItems] = useState([]);
  const { push } = useToast();

  /**
   * Load cart items from localStorage on component mount.
   * Gracefully handles corrupted or missing storage data.
   */
  useEffect(() => {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (raw) setItems(JSON.parse(raw));
    } catch (e) {
      console.error('Error loading cart from localStorage:', e);
    }
  }, []);

  /**
   * Persist cart items to localStorage whenever items change.
   * Gracefully handles storage quota exceeded errors.
   */
  useEffect(() => {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
    } catch (e) {
      console.error('Error saving cart to localStorage:', e);
    }
  }, [items]);

  /**
   * Add a service to the shopping cart.
   * 
   * Prevents duplicate items by checking service ID.
   * Provides user feedback via toast notifications.
   * 
   * @param {Object} service - Service object to add to cart
   * @param {number} service.id - Service ID
   * @param {string} [note=''] - Optional customer notes for the service
   * @returns {void}
   */
  const add = useCallback((service, note = '') => {
    setItems((prev) => {
      // Prevent duplicates by service id
      if (prev.find((p) => p.id === service.id)) {
        push('Service already in cart', 'info');
        return prev;
      }
      push('Service added to cart', 'success');
      return [...prev, { ...service, note }];
    });
  }, [push]);

  /**
   * Remove a service from the shopping cart.
   * 
   * @param {number} id - Service ID to remove
   * @returns {void}
   */
  const remove = useCallback((id) => {
    setItems((prev) => prev.filter((p) => p.id !== id));
    push('Service removed from cart', 'info');
  }, [push]);

  /**
   * Clear all items from the shopping cart.
   * 
   * @returns {void}
   */
  const clear = useCallback(() => {
    setItems([]);
  }, []);

  /**
   * Create an order from current cart items and process checkout.
   * 
   * Converts cart items to order format (each item as quantity 1),
   * submits order to backend API, clears cart on success,
   * and displays user feedback toast.
   * 
   * @param {string} token - JWT authentication token
   * @returns {Promise<Object>} Created order response
   * @throws {Error} If user is not authenticated (no token provided)
   */
  const checkout = useCallback(async (token) => {
    if (!token) throw new Error('You must be logged in to checkout');
    // Build order DTO with quantity 1 per service
    const orderDto = {
      orderItems: items.map((it) => ({ serviceId: it.id, quantity: 1 }))
    };
    const created = await createOrder(orderDto, token);
    clear();
    push('Order created successfully', 'success');
    return created;
  }, [items, clear, push]);

  return (
    <CartContext.Provider value={{ items, add, remove, clear, checkout }}>
      {children}
    </CartContext.Provider>
  );
}

/**
 * Hook to access shopping cart context.
 * 
 * Must be called within a CartProvider component tree.
 * 
 * @returns {Object} Cart context object
 * @returns {Array<Object>} items - Array of services in the cart
 * @returns {Function} add - Add service to cart
 * @returns {Function} remove - Remove service from cart
 * @returns {Function} clear - Clear all items from cart
 * @returns {Function} checkout - Create order from cart items
 * @throws {Error} If used outside of CartProvider
 */
export function useCart() {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCart must be used within CartProvider');
  return ctx;
}
