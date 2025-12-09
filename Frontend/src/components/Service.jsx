import { useState, useEffect } from 'react';
import { getServices } from '../../Services/api.js';

/**
 * Services catalog component with filtering and pagination.
 * 
 * Displays a grid of available services with filtering options:
 * - Category filtering
 * - Price range filtering
 * - Pagination for browsing through results
 * 
 * Shows service details including ratings, reviews, pricing, and availability.
 * Handles loading, error states, and empty results gracefully.
 * 
 * @returns {JSX.Element} Services grid with filters and pagination
 */
export default function Services() {
  const [services, setServices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [selectedCategory, setSelectedCategory] = useState('');
  const [priceRange, setPriceRange] = useState({ min: null, max: null });

  /**
   * Load services on filter or page change.
   */
  useEffect(() => {
    loadServices();
  }, [currentPage, selectedCategory, priceRange]);

  /**
   * Fetch services from API with current filters and pagination.
   * 
   * @private
   */
  const loadServices = async () => {
    try {
      setLoading(true);
      setError(null);

      const data = await getServices({
        category: selectedCategory,
        page: currentPage,
        pageSize: 12,
        minPrice: priceRange.min,
        maxPrice: priceRange.max
      });

      setServices(data.items || data);

      if (data.totalPages) {
        setTotalPages(data.totalPages);
      }
    } catch (err) {
      setError(
        'Error al cargar los servicios. Por favor intenta de nuevo.'
      );
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const categories = [
    'Todas',
    'Desarrollo web',
    'Diseño gráfico',
    'Marketing digital',
    'Redacción y traducción',
    'Video y gráficos en movimiento',
    'Música y audio'
  ];

  /**
   * Handle category filter change.
   * Resets to page 1 when filter changes.
   * 
   * @param {string} category - Selected category name
   * @private
   */
  const handleCategoryChange = (category) => {
    setSelectedCategory(category === 'Todas' ? '' : category);
    setCurrentPage(1);
  };

  /**
   * Handle price range filter change.
   * Resets to page 1 when filter changes.
   * 
   * @param {Object} range - Price range object with min and max
   * @private
   */
  const handlePriceFilter = (range) => {
    setPriceRange(range);
    setCurrentPage(1);
  };

  /**
   * Render star rating display.
   * 
   * @param {number} rating - Rating value (0-5)
   * @returns {JSX.Element} Star rating component
   * @private
   */
  const renderStars = (rating) => {
    return (
      <div className="flex items-center gap-1">
        {[...Array(5)].map((_, i) => (
          <span
            key={i}
            className={
              i < Math.floor(rating) ? 'text-yellow-400' : 'text-gray-600'
            }
          >
            ★
          </span>
        ))}
        <span className="text-primary-light text-sm ml-1">
          ({rating.toFixed(1)})
        </span>
      </div>
    );
  };

  // Loading state
  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-screen bg-primary-darkest">
        <div className="animate-spin rounded-full h-12 w-12 border-4 border-primary-medium border-t-primary-accent"></div>
      </div>
    );
  }

  // Error state
  if (error) {
    return (
      <div className="container mx-auto px-4 py-8 bg-primary-darkest min-h-screen">
        <div className="bg-red-200 text-red-700 px-4 py-3 rounded-md shadow-md flex justify-between items-center">
          <span>{error}</span>
          <button onClick={loadServices} className="text-red-700 font-semibold underline">
            Retry
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-primary-darkest min-h-screen">
      <div className="container mx-auto px-4 py-16">
        {/* Header */}
        <div className="text-center mb-12">
          <h2 className="text-4xl font-bold text-primary-lightest mb-4">
            Servicios profesionales
          </h2>
          <p className="text-primary-light max-w-xl mx-auto">
            Encuentra expertos que lleven tu proyecto al siguiente nivel
          </p>
        </div>

        {/* Category filters */}
        <div className="flex flex-wrap justify-center gap-3 mb-6">
          {categories.map((category) => (
            <button
              key={category}
              onClick={() => handleCategoryChange(category)}
              className={`px-5 py-2 rounded-full text-sm font-medium transition-all duration-200 shadow-sm ${(category === 'Todas' && !selectedCategory) ||
                selectedCategory === category
                ? 'bg-primary-accent text-white shadow-md'
                : 'bg-primary-dark text-primary-light hover:bg-primary-medium'
                }`}
            >
              {category}
            </button>
          ))}
        </div>

        {/* Price filters */}
        <div className="flex flex-wrap justify-center gap-3 mb-10">
          <button
            onClick={() => handlePriceFilter({ min: null, max: null })}
            className={`px-4 py-2 rounded-md text-sm transition-all duration-200 ${priceRange.min === null && priceRange.max === null
              ? 'bg-primary-accent text-white'
              : 'bg-primary-dark text-primary-light hover:bg-primary-medium'
              }`}
          >
            Todos los precios
          </button>
          <button
            onClick={() => handlePriceFilter({ min: null, max: 100 })}
            className={`px-4 py-2 rounded-md text-sm transition-all duration-200 ${priceRange.max === 100
              ? 'bg-primary-accent text-white'
              : 'bg-primary-dark text-primary-light hover:bg-primary-medium'
              }`}
          >
            Menos de $100
          </button>
          <button
            onClick={() => handlePriceFilter({ min: 100, max: 300 })}
            className={`px-4 py-2 rounded-md text-sm transition-all duration-200 ${priceRange.min === 100 && priceRange.max === 300
              ? 'bg-primary-accent text-white'
              : 'bg-primary-dark text-primary-light hover:bg-primary-medium'
              }`}
          >
            $100 - $300
          </button>
          <button
            onClick={() => handlePriceFilter({ min: 300, max: null })}
            className={`px-4 py-2 rounded-md text-sm transition-all duration-200 ${priceRange.min === 300
              ? 'bg-primary-accent text-white'
              : 'bg-primary-dark text-primary-light hover:bg-primary-medium'
              }`}
          >
            Más de $300
          </button>
        </div>

        {/* Empty state */}
        {services.length === 0 ? (
          <div className="text-center py-16 text-primary-light text-lg">
            No hay servicios disponibles con los filtros seleccionados
          </div>
        ) : (
          /* Services grid */
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8">
            {services.map((service) => (
              <div
                key={service.id}
                className="bg-primary-dark rounded-lg shadow-lg overflow-hidden hover:shadow-xl transition-all duration-300 border border-primary-medium flex flex-col"
              >
                {/* Service image */}
                <div className="h-48 bg-primary-darkest overflow-hidden relative">
                  {service.imageUrl ? (
                    <img
                      src={service.imageUrl}
                      alt={service.name}
                      className="w-full h-full object-cover hover:scale-105 transition-transform duration-300"
                    />
                  ) : (
                    <div className="w-full h-full flex items-center justify-center text-primary-accent">
                      Sin imagen
                    </div>
                  )}
                  {/* Verified badge */}
                  {service.verified && (
                    <span className="absolute top-3 right-3 bg-blue-500 text-white text-xs px-2 py-1 rounded-full flex items-center gap-1">
                      ✓ Verificado
                    </span>
                  )}
                  {/* Availability badge */}
                  {!service.available && (
                    <span className="absolute top-3 left-3 bg-orange-500 text-white text-xs px-2 py-1 rounded-full">
                      Busy
                    </span>
                  )}
                </div>

                {/* Service info */}
                <div className="p-5 flex flex-col flex-grow">
                  <div className="mb-3">
                    <h3 className="text-lg font-semibold text-primary-lightest mb-1">
                      {service.name}
                    </h3>
                    <p className="text-primary-accent text-sm font-medium">
                      por {service.provider}
                    </p>
                  </div>

                  <p className="text-primary-light text-sm mb-4 line-clamp-2 flex-grow">
                    {service.description}
                  </p>

                  {/* Rating */}
                  <div className="mb-3">
                    {renderStars(service.rating)}
                    <p className="text-primary-light text-xs mt-1">
                      {service.reviewCount} reseñas • {service.completedJobs} trabajos
                    </p>
                  </div>

                  {/* Price and delivery */}
                  <div className="flex items-center justify-between mb-3 pb-3 border-b border-primary-medium">
                    <div>
                      <span className="text-2xl font-bold text-primary-accent">
                        ${service.price.toFixed(2)}
                      </span>
                      <span className="text-primary-light text-sm ml-1">
                        / {service.priceType}
                      </span>
                    </div>
                    <div className="text-right">
                      <p className="text-primary-light text-xs">Entrega:</p>
                      <p className="text-primary-lightest text-sm font-medium">
                        {service.deliveryTime}
                      </p>
                    </div>
                  </div>

                  {/* Languages */}
                  {service.languages && service.languages.length > 0 && (
                    <div className="flex flex-wrap gap-1 mb-4">
                      {service.languages.map((lang) => (
                        <span
                          key={lang}
                          className="bg-primary-darkest text-primary-light text-xs px-2 py-1 rounded"
                        >
                          {lang}
                        </span>
                      ))}
                    </div>
                  )}

                  {/* Category badge */}
                  {service.category && (
                    <span className="inline-block bg-primary-darkest text-primary-lightest text-xs px-3 py-1 rounded-full mb-4 border border-primary-medium self-start">
                      {service.category}
                    </span>
                  )}

                  {/* CTA button */}
                  <a
                    href={`/service/${service.id}`}
                    className={`w-full inline-block text-center py-2.5 rounded-md font-medium transition-all duration-200 ${service.available
                      ? 'bg-primary-accent text-white hover:bg-opacity-80'
                      : 'bg-primary-medium text-primary-light cursor-not-allowed'
                      }`}
                    aria-disabled={!service.available}
                  >
                    {service.available ? 'Ver detalles' : 'No disponible'}
                  </a>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="flex justify-center items-center gap-4 mt-12">
            <button
              onClick={() => setCurrentPage((prev) => Math.max(1, prev - 1))}
              disabled={currentPage === 1}
              className="px-4 py-2 bg-primary-accent text-white rounded-md disabled:bg-primary-medium disabled:cursor-not-allowed hover:bg-opacity-80 transition-all"
            >
              Anterior
            </button>

            <span className="text-primary-light">
              Página {currentPage} de {totalPages}
            </span>

            <button
              onClick={() =>
                setCurrentPage((prev) => Math.min(totalPages, prev + 1))
              }
              disabled={currentPage === totalPages}
              className="px-4 py-2 bg-primary-accent text-white rounded-md disabled:bg-primary-medium disabled:cursor-not-allowed hover:bg-opacity-80 transition-all"
            >
              Siguiente
            </button>
          </div>
        )}
      </div>
    </div>
  );
}