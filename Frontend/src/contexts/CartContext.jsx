import React, { createContext, useContext, useEffect, useState, useCallback } from 'react';
import { createOrder } from '../../Services/api.js';
import { useToast } from './ToastContext.jsx';

const CartContext = createContext(null);

const STORAGE_KEY = 'servicehub_cart_v1';

export function CartProvider({ children }) {
  const [items, setItems] = useState([]);
  const { push } = useToast();

  useEffect(() => {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (raw) setItems(JSON.parse(raw));
    } catch (e) {}
  }, []);

  useEffect(() => {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
    } catch (e) {}
  }, [items]);

  const add = useCallback((service, note = '') => {
    setItems((prev) => {
      // avoid duplicates by service id
      if (prev.find((p) => p.id === service.id)) {
        push('El servicio ya está en el carrito', 'info');
        return prev;
      }
      push('Servicio agregado al carrito', 'success');
      return [...prev, { ...service, note }];
    });
  }, [push]);

  const remove = useCallback((id) => {
    setItems((prev) => prev.filter((p) => p.id !== id));
    push('Servicio removido del carrito', 'info');
  }, [push]);

  const clear = useCallback(() => {
    setItems([]);
  }, []);

  const checkout = useCallback(async (token) => {
    if (!token) throw new Error('Necesitas iniciar sesión');
    // build orderDto with quantity 1 per service
    const orderDto = {
      orderItems: items.map((it) => ({ serviceId: it.id, quantity: 1 }))
    };
    const created = await createOrder(orderDto, token);
    clear();
    push('Orden creada correctamente', 'success');
    return created;
  }, [items, clear, push]);

  return (
    <CartContext.Provider value={{ items, add, remove, clear, checkout }}>
      {children}
    </CartContext.Provider>
  );
}

export function useCart() {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCart must be used within CartProvider');
  return ctx;
}
