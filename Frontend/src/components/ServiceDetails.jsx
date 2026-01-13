import React, { useEffect, useState } from 'react';
import { getServiceById, createOrder } from '../../Services/api.js';
import { useAuth } from '../hooks/useAuth.js';
import { useCart } from '../contexts/CartContext.jsx';
import { Trash2 } from 'lucide-react';
import Modal from './Modal.jsx';
import Providers from './Providers.jsx';

/**
 * Service details page component.
 * 
 * Displays comprehensive information about a single service including:
 * - Service description, pricing, and delivery details
 * - Provider information and ratings
 * - Availability status and languages supported
 * - Options to request service or add to cart
 * - Order confirmation modal
 * 
 * Requires authentication for service requests.
 * 
 * @param {Object} props - Component props
 * @param {number} props.serviceId - ID of the service to display
 * @returns {JSX.Element} Service details page
 */
export default function ServiceDetails(props) {
    return (
        <Providers>
            <ServiceDetailsContent {...props} />
        </Providers>
    );
}

function ServiceDetailsContent({ serviceId }) {
    const [service, setService] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [submitting, setSubmitting] = useState(false);
    const [orderModalOpen, setOrderModalOpen] = useState(false);
    const [createdOrder, setCreatedOrder] = useState(null);
    const [mounted, setMounted] = useState(false);

    const { token, isAuthenticated, mounted: authMounted } = useAuth();
    const { items, add, remove } = useCart();

    // Check if service is already in cart
    const isInCart = items.some(item => item.id === serviceId);

    /**
     * Set mounted flag for SSR compatibility.
     */
    useEffect(() => {
        setMounted(true);
    }, []);

    /**
     * Fetch service details from API.
     */
    useEffect(() => {
        const load = async () => {
            try {
                setLoading(true);
                const data = await getServiceById(serviceId);
                setService(data);
            } catch (err) {
                console.error(err);
                setError('No se pudo cargar el servicio. Por favor intenta de nuevo.');
            } finally {
                setLoading(false);
            }
        };
        load();
    }, [serviceId]);

    /**
     * Handle service request (immediate order creation).
     * Redirects to login if not authenticated.
     */
    const handleRequest = async () => {
        if (!isAuthenticated) {
            window.location.href = '/login';
            return;
        }

        try {
            setSubmitting(true);
            // Create order immediately with single service
            const orderDto = {
                orderItems: [{ serviceId: service.id, quantity: 1 }]
            };
            const created = await createOrder(orderDto, token);

            setCreatedOrder(created);
            setOrderModalOpen(true);
        } catch (err) {
            console.error(err);
        } finally {
            setSubmitting(false);
        }
    };

    /**
     * Toggle service in shopping cart.
     */
    const handleToggleCart = () => {
        if (isInCart) {
            remove(service.id);
        } else {
            add({
                id: service.id,
                name: service.name,
                price: service.price,
                provider: service.provider,
                imageUrl: service.imageUrl,
                deliveryTime: service.deliveryTime
            });
        }
    };

    if (loading)
        return <div className="p-6 text-center">Cargando servicio...</div>;
    if (error) return <div className="p-6 text-red-400">{error}</div>;
    if (!service) return null;

    return (
        <div className="py-8">
            {/* Image and Quick Actions */}
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

                {/* Service Info and Actions */}
                <div className="flex flex-col justify-between">
                    <div className="bg-primary-dark rounded-lg p-6 space-y-6">
                        {/* Header */}
                        <div>
                            <h1 className="text-3xl font-bold text-primary-lightest mb-2">
                                {service.name}
                            </h1>
                            <p className="text-primary-accent text-lg font-semibold">
                                por {service.provider}
                            </p>
                            {service.verified && (
                                <p className="text-blue-400 text-sm mt-2">✓ Verificado</p>
                            )}
                        </div>

                        {/* Rating & Stats */}
                        <div className="space-y-2">
                            <div className="flex items-center gap-2">
                                <span className="text-yellow-400">
                                    {'★'.repeat(Math.floor(service.rating))}
                                    {'☆'.repeat(5 - Math.floor(service.rating))}
                                </span>
                                <span className="text-primary-light text-sm">
                                    ({service.rating.toFixed(1)})
                                </span>
                            </div>
                            <p className="text-primary-light text-sm">
                                {service.reviewCount} reviews • {service.completedJobs} jobs
                                completed
                            </p>
                        </div>

                        {/* Pricing */}
                        <div className="border-t border-primary-medium pt-4">
                            <div className="text-sm text-primary-light mb-1">Precio</div>
                            <div className="text-3xl font-bold text-primary-accent">
                                ${Number(service.price).toFixed(2)}
                                <span className="text-lg text-primary-light">
                                    /{service.priceType}
                                </span>
                            </div>
                        </div>

                        {/* Details */}
                        <div className="border-t border-primary-medium pt-4 space-y-2">
                            <div>
                                <span className="text-primary-light text-sm">
                                    Tiempo de entrega:
                                </span>
                                <p className="text-primary-lightest font-semibold">
                                    {service.deliveryTime}
                                </p>
                            </div>
                            {service.languages?.length > 0 && (
                                <div>
                                    <span className="text-primary-light text-sm">Idiomas:</span>
                                    <div className="flex flex-wrap gap-2 mt-1">
                                        {service.languages.map((lang) => (
                                            <span
                                                key={lang}
                                                className="bg-primary-darkest px-3 py-1 rounded text-sm text-primary-light border border-primary-medium"
                                            >
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
                                className={`w-full py-3 rounded-md font-semibold transition-all ${service.available
                                    ? 'bg-primary-accent text-white hover:bg-opacity-80'
                                    : 'bg-primary-medium text-primary-light cursor-not-allowed'
                                    }`}
                            >
                                {submitting
                                    ? 'Procesando...'
                                    : service.available
                                        ? 'Solicitar servicio'
                                        : 'No disponible'}
                            </button>

                            <button
                                onClick={handleToggleCart}
                                disabled={!service.available}
                                className={`py-3 rounded-md font-semibold transition-all flex items-center justify-center gap-2
                  ${isInCart
                                        ? 'w-16 bg-red-500/10 border-2 border-red-500 text-red-500 hover:bg-red-500/20'
                                        : 'w-full bg-primary-darkest border-2 border-primary-accent text-primary-accent hover:bg-primary-dark'
                                    }`}
                                title={isInCart ? "Eliminar del carrito" : "Añadir al carrito"}
                            >
                                {isInCart ? <Trash2 size={20} /> : 'Añadir al carrito'}
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            {/* Description & Details */}
            <div className="bg-primary-dark rounded-lg p-6 space-y-6">
                <div>
                    <h2 className="text-2xl font-semibold text-primary-lightest mb-3">
                        Description
                    </h2>
                    <p className="text-primary-light leading-relaxed">
                        {service.description}
                    </p>
                </div>

                <div>
                    <h3 className="text-lg font-semibold text-primary-lightest mb-2">
                        Details
                    </h3>
                    <div className="grid grid-cols-2 gap-4 text-primary-light text-sm">
                        <div>
                            <span className="block text-primary-light opacity-75">
                                Availability
                            </span>
                            <span
                                className={`font-semibold ${service.available ? 'text-green-400' : 'text-orange-400'
                                    }`}
                            >
                                {service.available ? 'Available' : 'Busy'}
                            </span>
                        </div>
                        <div>
                            <span className="block text-primary-light opacity-75">
                                Created
                            </span>
                            <span className="font-semibold">
                                {new Date(service.createdAt).toLocaleDateString()}
                            </span>
                        </div>
                        <div>
                            <span className="block text-primary-light opacity-75">
                                Updated
                            </span>
                            <span className="font-semibold">
                                {new Date(service.updatedAt).toLocaleDateString()}
                            </span>
                        </div>
                    </div>
                </div>
            </div>

            {/* Order Confirmation Modal */}
            <Modal
                open={orderModalOpen}
                title="Solicitud enviada"
                onClose={() => setOrderModalOpen(false)}
                footer={
                    <div className="flex justify-between gap-3">
                        <button
                            onClick={() => setOrderModalOpen(false)}
                            className="px-4 py-2 bg-primary-accent text-white rounded hover:bg-opacity-80"
                        >
                            Cerrar
                        </button>
                    </div>
                }
            >
                {createdOrder ? (
                    <div className="space-y-4">
                        <div className="bg-primary-darkest p-4 rounded">
                            <p className="text-sm text-primary-light">ID de orden</p>
                            <p className="text-2xl font-bold text-primary-accent">
                                #{createdOrder.id}
                            </p>
                        </div>
                        <div className="grid grid-cols-2 gap-4 text-sm">
                            <div>
                                <p className="text-primary-light">Fecha</p>
                                <p className="text-primary-lightest font-semibold">
                                    {new Date(createdOrder.orderDate).toLocaleString()}
                                </p>
                            </div>
                            <div>
                                <p className="text-primary-light">Total</p>
                                <p className="text-primary-accent font-bold text-lg">
                                    ${Number(createdOrder.totalAmount).toFixed(2)}
                                </p>
                            </div>
                        </div>
                        <div className="border-t border-primary-medium pt-4">
                            <p className="font-semibold text-primary-lightest mb-2">
                                Servicios solicitados
                            </p>
                            <ul className="space-y-2">
                                {createdOrder.orderItems.map((it) => (
                                    <li
                                        key={it.id}
                                        className="text-primary-light text-sm bg-primary-darkest p-2 rounded"
                                    >
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
