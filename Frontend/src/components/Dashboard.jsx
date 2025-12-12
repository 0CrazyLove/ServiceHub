import { useState, useEffect } from 'react';
import { useAuth } from '../hooks/useAuth.js';
import { getDashboardStats, getOrders, getServices } from '../../Services/api.js';


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
  const [extendedStats, setExtendedStats] = useState({
    topServices: [],
    categoryStats: [],
    allOrders: []
  });

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

        // Fetch basic stats, orders, and services in parallel
        const [basicStats, ordersData, servicesData] = await Promise.all([
          getDashboardStats(token),
          getOrders(token),
          getServices({ pageSize: 1000 }) // Fetch all services to map categories
        ]);

        setStats(basicStats);

        // Process data for extended statistics
        processExtendedStats(ordersData, servicesData.items || []);

      } catch (err) {
        console.error(err);
        setError('No se pudieron cargar las estadísticas. Comprueba que tu sesión tenga permisos de administrador.');
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [token, isAuthenticated, mounted]);

  const processExtendedStats = (orders, services) => {
    // Create a map of ServiceId -> Service Details for quick lookup
    const serviceMap = new Map();
    services.forEach(s => serviceMap.set(s.id, s));

    const serviceSales = new Map(); // ServiceId -> { name, totalSales, count }
    const categorySales = new Map(); // CategoryName -> { totalSales, count }

    // Enrich orders with service details (image, category)
    const enrichedOrders = orders.map(order => {
      const enrichedItems = order.orderItems.map(item => {
        const service = serviceMap.get(item.serviceId);
        return {
          ...item,
          serviceImage: service ? service.imageUrl : null,
          category: service ? service.category : 'Otros',
          serviceName: item.serviceName || (service ? service.name : 'Servicio Desconocido')
        };
      });
      return { ...order, orderItems: enrichedItems };
    });

    enrichedOrders.forEach(order => {
      order.orderItems.forEach(item => {
        const itemTotal = item.quantity * item.price;

        // Aggregate Service Stats
        if (!serviceSales.has(item.serviceId)) {
          serviceSales.set(item.serviceId, { name: item.serviceName, totalSales: 0, count: 0 });
        }
        const sStat = serviceSales.get(item.serviceId);
        sStat.totalSales += itemTotal;
        sStat.count += item.quantity;

        // Aggregate Category Stats
        if (!categorySales.has(item.category)) {
          categorySales.set(item.category, { name: item.category, totalSales: 0, count: 0 });
        }
        const cStat = categorySales.get(item.category);
        cStat.totalSales += itemTotal;
        cStat.count += item.quantity;
      });
    });

    // Calculate Top 5 Services
    const topServices = Array.from(serviceSales.values())
      .sort((a, b) => b.totalSales - a.totalSales)
      .slice(0, 5);

    // Calculate Category Distribution
    const categoryStats = Array.from(categorySales.values())
      .sort((a, b) => b.totalSales - a.totalSales);

    // Sort all orders by date descending
    const allOrders = [...enrichedOrders]
      .sort((a, b) => new Date(b.orderDate) - new Date(a.orderDate));

    setExtendedStats({
      topServices,
      categoryStats,
      allOrders
    });
  };

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
        Panel de Control
      </h2>

      {/* KPI Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-6 mb-8">
        <div className="bg-primary-dark rounded-lg p-6 shadow-md border border-primary-medium">
          <p className="text-sm text-primary-light">Ventas totales</p>
          <p className="text-2xl font-semibold text-primary-accent mt-2">
            ${Number(totalSales).toFixed(2)}
          </p>
        </div>

        <div className="bg-primary-dark rounded-lg p-6 shadow-md border border-primary-medium">
          <p className="text-sm text-primary-light">Servicios Activos</p>
          <p className="text-2xl font-semibold text-primary-accent mt-2">
            {serviceCount}
          </p>
        </div>

        <div className="bg-primary-dark rounded-lg p-6 shadow-md border border-primary-medium">
          <p className="text-sm text-primary-light">Órdenes Totales</p>
          <p className="text-2xl font-semibold text-primary-accent mt-2">
            {orderCount}
          </p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
        {/* Top Services */}
        <div className="bg-primary-dark rounded-lg p-6 shadow-md border border-primary-medium">
          <h3 className="text-xl font-semibold text-primary-lightest mb-4">Servicios Más Vendidos</h3>
          {extendedStats.topServices.length > 0 ? (
            <div className="space-y-4">
              {extendedStats.topServices.map((service, index) => (
                <div key={index} className="flex justify-between items-center border-b border-primary-medium pb-2 last:border-0">
                  <div className="flex items-center">
                    <span className="text-primary-accent font-bold mr-3">#{index + 1}</span>
                    <span className="text-primary-light">{service.name}</span>
                  </div>
                  <div className="text-right">
                    <p className="text-primary-lightest font-medium">${service.totalSales.toFixed(2)}</p>
                    <p className="text-xs text-primary-light">{service.count} ventas</p>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-primary-light text-sm">No hay datos de ventas disponibles.</p>
          )}
        </div>

        {/* Category Distribution */}
        <div className="bg-primary-dark rounded-lg p-6 shadow-md border border-primary-medium">
          <h3 className="text-xl font-semibold text-primary-lightest mb-4">Ventas por Categoría</h3>
          {extendedStats.categoryStats.length > 0 ? (
            <div className="space-y-4">
              {extendedStats.categoryStats.map((cat, index) => (
                <div key={index}>
                  <div className="flex justify-between text-sm mb-1">
                    <span className="text-primary-light">{cat.name}</span>
                    <span className="text-primary-lightest">${cat.totalSales.toFixed(2)}</span>
                  </div>
                  <div className="w-full bg-primary-medium rounded-full h-2.5">
                    <div
                      className="bg-primary-accent h-2.5 rounded-full"
                      style={{ width: `${(cat.totalSales / totalSales) * 100}%` }}
                    ></div>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-primary-light text-sm">No hay datos de categorías disponibles.</p>
          )}
        </div>
      </div>

      {/* Order History */}
      <div className="bg-primary-dark rounded-lg p-6 shadow-md border border-primary-medium mb-8">
        <h3 className="text-xl font-semibold text-primary-lightest mb-4">Historial de Órdenes</h3>
        <div className="overflow-x-auto">
          <table className="w-full text-left text-primary-light">
            <thead>
              <tr className="border-b border-primary-medium">
                <th className="pb-3 font-medium">ID</th>
                <th className="pb-3 font-medium">Usuario</th>
                <th className="pb-3 font-medium">Fecha</th>
                <th className="pb-3 font-medium">Servicios</th>
                <th className="pb-3 font-medium text-right">Total</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-primary-medium">
              {extendedStats.allOrders.length > 0 ? (
                extendedStats.allOrders.map((order) => (
                  <tr key={order.id} className="group hover:bg-primary-medium/20 transition-colors">
                    <td className="py-3 text-primary-accent font-medium">#{order.id}</td>
                    <td className="py-3 text-primary-light text-sm">{order.userId || 'N/A'}</td>
                    <td className="py-3">{new Date(order.orderDate).toLocaleDateString()} {new Date(order.orderDate).toLocaleTimeString()}</td>
                    <td className="py-3">
                      <div className="flex flex-col space-y-2">
                        {order.orderItems.map((item, idx) => {
                          // We need to find the service image. 
                          // The serviceMap is local to processExtendedStats. 
                          // I should probably enrich the order items with service details in processExtendedStats.
                          return (
                            <div key={idx} className="flex items-center">
                              {item.serviceImage && (
                                <img
                                  src={item.serviceImage}
                                  alt={item.serviceName}
                                  className="w-16 h-16 rounded object-cover mr-3 border border-primary-medium"
                                />
                              )}
                              <span className="text-sm">{item.serviceName} <span className="text-primary-light/70">x{item.quantity}</span></span>
                            </div>
                          )
                        })}
                      </div>
                    </td>
                    <td className="py-3 text-right font-medium text-primary-lightest">
                      ${order.totalAmount.toFixed(2)}
                    </td>
                  </tr>
                ))) : (
                <tr>
                  <td colSpan="4" className="py-4 text-center text-primary-light">No hay órdenes registradas.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

