import React, { useEffect, useState } from 'react';
import { getServiceById, createOrder } from '../../Services/api.js';
import { useAuth } from '../hooks/useAuth.js';
import Modal from './Modal.jsx';

export default function ServiceDetails({ serviceId }) {
  const [service, setService] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [submitting, setSubmitting] = useState(false);
  const [orderModalOpen, setOrderModalOpen] = useState(false);
  const [createdOrder, setCreatedOrder] = useState(null);
  const [mounted, setMounted] = useState(false);

  const { token, isAuthenticated, mounted: authMounted } = useAuth();

  useEffect(() => {
    setMounted(true);
  }, []);

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);
        const data = await getServiceById(serviceId);
        setService(data);
      } catch (err) {
        console.error(err);
        setError('No se pudo cargar el servicio. Intenta de nuevo.');
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [serviceId]);

  const handleRequest = async () => {
    if (!isAuthenticated) {
      window.location.href = '/login';
      return;
    }

    try {
      setSubmitting(true);
      // Request -> create order immediately (one service per order)
      const orderDto = { orderItems: [{ serviceId: service.id, quantity: 1 }] };
      const created = await createOrder(orderDto, token);

      setCreatedOrder(created);
      setOrderModalOpen(true);
    } catch (err) {
      console.error(err);
    } finally {
      setSubmitting(false);
    }
  };

  const handleAddToCart = () => {
    if (!mounted) return;
    
    console.log('🛒 handleAddToCart llamado');
    console.log('Service:', service);
    
    try {
      const STORAGE_KEY = 'servicehub_cart_v1';
      const raw = localStorage.getItem(STORAGE_KEY) || '[]';
      const items = JSON.parse(raw);
      
      console.log('Items actuales en localStorage:', items);
      console.log('IDs en carrito:', items.map(i => i.id));
      console.log('¿Ya existe?', items.find((p) => p.id === service.id));
      
      // Check if already in cart
      if (items.find((p) => p.id === service.id)) {
        console.log('⚠️ El servicio ya está en el carrito');
        alert('El servicio ya está en el carrito');
        return;
      }
      
      // Add to cart
      const newItem = { 
        id: service.id, 
        name: service.name, 
        price: service.price, 
        provider: service.provider, 
        imageUrl: service.imageUrl, 
        deliveryTime: service.deliveryTime 
      };
      
      console.log('Nuevo item a agregar:', newItem);
      items.push(newItem);
      
      localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
      console.log('✅ Item guardado en localStorage:', localStorage.getItem(STORAGE_KEY));
      
      // Verify it was saved
      const verify = localStorage.getItem(STORAGE_KEY);
      console.log('🔍 Verificación post-guardado:', verify);
      
      // Show success and navigate to home
      alert('Servicio agregado al carrito');
      
      // Dispatch storage event for CartIcon to update
      window.dispatchEvent(new Event('storage'));
      
      // Navigate to home after a longer delay
      setTimeout(() => {
        window.location.href = '/?t=' + Date.now();
      }, 1000);
    } catch (e) {
      console.error('Error adding to cart:', e);
      alert('Error al agregar al carrito');
    }
  };

  if (loading) return <div className="p-6 text-center">Cargando servicio...</div>;
  if (error) return <div className="p-6 text-red-400">{error}</div>;
  if (!service) return null;

  return (
    <div className="py-8">
      {/* Hero / Image Section */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8 mb-12">
        <div className="bg-primary-dark rounded-lg p-6 flex items-center justify-center min-h-96">
          {service.imageUrl ? (
            <img
              src={service.imageUrl}
              alt={service.name}
              className="w-full h-full object-cover rounded"
            />
          ) : (
            <div className="w-full h-full flex items-center justify-center text-primary-accent text-3xl">
              {service.name.substring(0, 1)}
            </div>
          )}
        </div>

        {/* Quick Actions */}
        <div className="flex flex-col justify-between">
          <div className="bg-primary-dark rounded-lg p-6 space-y-6">
            <div>
              <h1 className="text-3xl font-bold text-primary-lightest mb-2">{service.name}</h1>
              <p className="text-primary-accent text-lg font-semibold">por {service.provider}</p>
              {service.verified && <p className="text-blue-400 text-sm mt-2">✓ Verificado</p>}
            </div>

            {/* Rating & Stats */}
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <span className="text-yellow-400">{'★'.repeat(Math.floor(service.rating))}{'☆'.repeat(5 - Math.floor(service.rating))}</span>
                <span className="text-primary-light text-sm">({service.rating.toFixed(1)})</span>
              </div>
              <p className="text-primary-light text-sm">
                {service.reviewCount} reseñas • {service.completedJobs} trabajos completados
              </p>
            </div>

            {/* Price */}
            <div className="border-t border-primary-medium pt-4">
              <div className="text-sm text-primary-light mb-1">Precio</div>
              <div className="text-3xl font-bold text-primary-accent">
                ${Number(service.price).toFixed(2)}
                <span className="text-lg text-primary-light">/{service.priceType}</span>
              </div>
            </div>

            {/* Delivery & Details */}
            <div className="border-t border-primary-medium pt-4 space-y-2">
              <div>
                <span className="text-primary-light text-sm">Tiempo de entrega:</span>
                <p className="text-primary-lightest font-semibold">{service.deliveryTime}</p>
              </div>
              {service.languages?.length > 0 && (
                <div>
                  <span className="text-primary-light text-sm">Idiomas:</span>
                  <div className="flex flex-wrap gap-2 mt-1">
                    {service.languages.map((lang) => (
                      <span key={lang} className="bg-primary-darkest px-3 py-1 rounded text-sm text-primary-light border border-primary-medium">
                        {lang}
                      </span>
                    ))}
                  </div>
                </div>
              )}
              {service.category && (
                <div>
                  <span className="inline-block bg-primary-darkest px-4 py-2 rounded-full text-primary-accent border border-primary-medium text-sm">
                    {service.category}
                  </span>
                </div>
              )}
            </div>

            {/* Action Buttons */}
            <div className="space-y-3 border-t border-primary-medium pt-4">
              <button
                onClick={handleRequest}
                disabled={!service.available || submitting}
                className={`w-full py-3 rounded-md font-semibold transition-all ${
                  service.available ? 'bg-primary-accent text-white hover:bg-opacity-80' : 'bg-primary-medium text-primary-light cursor-not-allowed'
                }`}>
                {submitting ? 'Procesando...' : service.available ? 'Solicitar Servicio' : 'No disponible'}
              </button>

              <button
                onClick={handleAddToCart}
                disabled={!service.available}
                className="w-full py-3 rounded-md font-semibold bg-primary-darkest border-2 border-primary-accent text-primary-accent hover:bg-primary-dark transition-all">
                Añadir al Carrito
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Description & Details */}
      <div className="bg-primary-dark rounded-lg p-6 space-y-6">
        <div>
          <h2 className="text-2xl font-semibold text-primary-lightest mb-3">Descripción</h2>
          <p className="text-primary-light leading-relaxed">{service.description}</p>
        </div>

        <div>
          <h3 className="text-lg font-semibold text-primary-lightest mb-2">Detalles</h3>
          <div className="grid grid-cols-2 gap-4 text-primary-light text-sm">
            <div>
              <span className="block text-primary-light opacity-75">Disponibilidad</span>
              <span className={`font-semibold ${service.available ? 'text-green-400' : 'text-orange-400'}`}>
                {service.available ? 'Disponible' : 'Ocupado'}
              </span>
            </div>
            <div>
              <span className="block text-primary-light opacity-75">Creado</span>
              <span className="font-semibold">{new Date(service.createdAt).toLocaleDateString()}</span>
            </div>
            <div>
              <span className="block text-primary-light opacity-75">Actualizado</span>
              <span className="font-semibold">{new Date(service.updatedAt).toLocaleDateString()}</span>
            </div>
          </div>
        </div>
      </div>

      {/* Order Modal */}
      <Modal
        open={orderModalOpen}
        title="Solicitud Enviada"
        onClose={() => setOrderModalOpen(false)}
        footer={
          <div className="flex justify-between gap-3">
            <button onClick={() => setOrderModalOpen(false)} className="px-4 py-2 bg-primary-accent text-white rounded hover:bg-opacity-80">
              Cerrar
            </button>
          </div>
        }>
        {createdOrder ? (
          <div className="space-y-4">
            <div className="bg-primary-darkest p-4 rounded">
              <p className="text-sm text-primary-light">ID de Orden</p>
              <p className="text-2xl font-bold text-primary-accent">#{createdOrder.id}</p>
            </div>
            <div className="grid grid-cols-2 gap-4 text-sm">
              <div>
                <p className="text-primary-light">Fecha</p>
                <p className="text-primary-lightest font-semibold">{new Date(createdOrder.orderDate).toLocaleString()}</p>
              </div>
              <div>
                <p className="text-primary-light">Total</p>
                <p className="text-primary-accent font-bold text-lg">${Number(createdOrder.totalAmount).toFixed(2)}</p>
              </div>
            </div>
            <div className="border-t border-primary-medium pt-4">
              <p className="font-semibold text-primary-lightest mb-2">Servicios Solicitados</p>
              <ul className="space-y-2">
                {createdOrder.orderItems.map((it) => (
                  <li key={it.id} className="text-primary-light text-sm bg-primary-darkest p-2 rounded">
                    {it.serviceName} — ${Number(it.price).toFixed(2)}
                  </li>
                ))}
              </ul>
            </div>
          </div>
        ) : (
          <div className="text-primary-light">No hay detalles disponibles.</div>
        )}
      </Modal>
    </div>
  );
}
