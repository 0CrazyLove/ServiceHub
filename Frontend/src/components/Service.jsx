import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Search, Filter, ChevronDown } from 'lucide-react';
import { getServices } from '../../Services/api.js';
import ServiceCard from './ServiceCard';

/**
 * Services catalog component with filtering and pagination.
 * 
 * Displays a grid of available services with filtering options:
 * - Search by name/description
 * - Category filtering
 * - Sorting
 * - Pagination
 * 
 * @returns {JSX.Element} Services grid with filters and pagination
 */
export default function Services() {
  const [services, setServices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  // Filter states
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('Todas');
  const [sortBy, setSortBy] = useState('rating');

  const categories = [
    'Todas',
    'Desarrollo web',
    'Diseño gráfico',
    'Marketing digital',
    'Redacción y traducción',
    'Video y gráficos en movimiento',
    'Música y audio',
    'Negocios',
    'Finanzas'
  ];

  /**
   * Initialize filters from URL query params
   */
  useEffect(() => {
    const params = new URLSearchParams(window.location.search);
    const categoryParam = params.get('category');
    const searchParam = params.get('search');

    if (categoryParam) setSelectedCategory(categoryParam);
    if (searchParam) setSearchQuery(searchParam);
  }, []);

  /**
   * Load services when filters or page change
   */
  useEffect(() => {
    loadServices();

    // Update URL without reloading
    const params = new URLSearchParams();
    if (selectedCategory !== 'Todas') params.set('category', selectedCategory);
    if (searchQuery) params.set('search', searchQuery);
    if (currentPage > 1) params.set('page', currentPage.toString());

    const newUrl = `${window.location.pathname}${params.toString() ? '?' + params.toString() : ''}`;
    window.history.replaceState({}, '', newUrl);

  }, [currentPage, selectedCategory, sortBy]); // Note: searchQuery is handled on submit/debounce ideally, but here we'll load on effect for simplicity or add a specific search trigger

  /**
   * Fetch services from API
   */
  const loadServices = async () => {
    try {
      setLoading(true);
      setError(null);

      // Note: The API might need updates to support 'search' and 'sortBy'
      // For now, we pass what we can. If API doesn't support search, we might need client-side filtering
      const data = await getServices({
        category: selectedCategory === 'Todas' ? '' : selectedCategory,
        page: currentPage,
        pageSize: 12,
        search: searchQuery, // Assuming API supports this or we filter client-side
        sortBy: sortBy // Assuming API supports this
      });

      let items = data.items || data;

      // Client-side filtering/sorting if API doesn't support it fully yet
      // This is a fallback to ensure the UI works as expected
      if (searchQuery) {
        const lowerQuery = searchQuery.toLowerCase();
        items = items.filter(s =>
          s.name.toLowerCase().includes(lowerQuery) ||
          s.description?.toLowerCase().includes(lowerQuery)
        );
      }

      if (sortBy) {
        items.sort((a, b) => {
          switch (sortBy) {
            case 'rating': return b.rating - a.rating;
            case 'price-low': return a.price - b.price;
            case 'price-high': return b.price - a.price;
            case 'reviews': return (b.reviewCount || 0) - (a.reviewCount || 0);
            default: return 0;
          }
        });
      }

      setServices(items);

      if (data.totalPages) {
        setTotalPages(data.totalPages);
      }
    } catch (err) {
      setError('Error al cargar los servicios. Por favor intenta de nuevo.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearchSubmit = (e) => {
    e.preventDefault();
    setCurrentPage(1);
    loadServices();
  };

  return (
    <div className="min-h-screen bg-primary-darkest">
      {/* Header */}
      <section className="border-b border-primary-light/10 bg-primary-darkest pt-8 pb-2">
        <div className="container mx-auto px-4">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
          >
            <h1 className="mb-3 text-3xl font-bold text-primary-lightest md:text-4xl">Explorar servicios</h1>
            <p className="text-primary-light max-w-2xl">
              Encuentra el profesional perfecto para tu proyecto entre miles de expertos verificados.
            </p>
          </motion.div>
        </div>
      </section>

      {/* Filters Bar */}
      <section className="border-b border-primary-light/10 bg-primary-darkest">
        <div className="container mx-auto px-4 py-4">
          <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
            {/* Search */}
            <form onSubmit={handleSearchSubmit} className="relative max-w-md flex-1">
              <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-primary-light/50" />
              <input
                type="text"
                placeholder="Buscar servicios..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full h-10 pl-10 pr-4 rounded-md bg-primary-medium/20 border border-primary-light/10 text-primary-lightest placeholder-primary-light/50 focus:outline-none focus:ring-2 focus:ring-primary-accent transition-all"
              />
            </form>

            {/* Category & Sort (Mobile/Desktop) */}
            <div className="flex items-center gap-3">
              {/* Sort Dropdown */}
              <div className="relative group">
                <button className="flex items-center gap-2 px-4 py-2 rounded-md bg-primary-medium/20 border border-primary-light/10 text-primary-lightest hover:bg-primary-medium/30 transition-all text-sm font-medium w-48 justify-between">
                  <span>
                    {sortBy === 'rating' && 'Mejor calificados'}
                    {sortBy === 'reviews' && 'Más reseñas'}
                    {sortBy === 'price-low' && 'Precio: Menor a mayor'}
                    {sortBy === 'price-high' && 'Precio: Mayor a menor'}
                  </span>
                  <ChevronDown className="h-4 w-4 opacity-50" />
                </button>
                {/* Dropdown Content */}
                <div className="absolute right-0 mt-2 w-48 bg-primary-dark border border-primary-light/10 rounded-md shadow-xl opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all z-50 overflow-hidden">
                  <button onClick={() => setSortBy('rating')} className="w-full text-left px-4 py-2 text-sm text-primary-light hover:bg-primary-medium/20 hover:text-white transition-colors">Mejor calificados</button>
                  <button onClick={() => setSortBy('reviews')} className="w-full text-left px-4 py-2 text-sm text-primary-light hover:bg-primary-medium/20 hover:text-white transition-colors">Más reseñas</button>
                  <button onClick={() => setSortBy('price-low')} className="w-full text-left px-4 py-2 text-sm text-primary-light hover:bg-primary-medium/20 hover:text-white transition-colors">Precio: Menor a mayor</button>
                  <button onClick={() => setSortBy('price-high')} className="w-full text-left px-4 py-2 text-sm text-primary-light hover:bg-primary-medium/20 hover:text-white transition-colors">Precio: Mayor a menor</button>
                </div>
              </div>
            </div>
          </div>

          {/* Category Pills */}
          <div className="mt-4 flex flex-wrap gap-2 pb-2 overflow-x-auto no-scrollbar">
            {categories.map((cat) => (
              <button
                key={cat}
                onClick={() => {
                  setSelectedCategory(cat);
                  setCurrentPage(1);
                }}
                className={`whitespace-nowrap rounded-full px-4 py-1.5 text-sm font-medium transition-colors border ${selectedCategory === cat
                  ? 'bg-primary-accent border-primary-accent text-white'
                  : 'bg-primary-medium/10 border-primary-light/10 text-primary-light hover:bg-primary-medium/20 hover:text-primary-lightest'
                  }`}
              >
                {cat}
              </button>
            ))}
          </div>
        </div>
      </section>

      {/* Results */}
      <section className="py-8 min-h-[50vh]">
        <div className="container mx-auto px-4">
          <div className="mb-6 flex items-center justify-between">
            <p className="text-sm text-primary-light">
              {services.length} servicios encontrados
            </p>
          </div>

          {loading ? (
            <div className="flex justify-center items-center py-20">
              <div className="animate-spin rounded-full h-12 w-12 border-4 border-primary-medium border-t-primary-accent"></div>
            </div>
          ) : services.length > 0 ? (
            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 justify-items-center">
              {services.map((service, index) => (
                <ServiceCard key={service.id} service={service} index={index} />
              ))}
            </div>
          ) : (
            <div className="py-20 text-center">
              <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-primary-medium/20 mb-4">
                <Search className="h-8 w-8 text-primary-light/50" />
              </div>
              <h3 className="text-xl font-semibold text-primary-lightest mb-2">No se encontraron resultados</h3>
              <p className="text-primary-light max-w-md mx-auto mb-6">
                No pudimos encontrar servicios que coincidan con tus filtros. Intenta ajustar tu búsqueda o categoría.
              </p>
              <button
                className="px-6 py-2 bg-primary-dark border border-primary-light/20 rounded-md text-primary-lightest hover:bg-primary-medium/20 transition-all"
                onClick={() => {
                  setSearchQuery('');
                  setSelectedCategory('Todas');
                  setCurrentPage(1);
                }}
              >
                Limpiar filtros
              </button>
            </div>
          )}

          {/* Pagination */}
          {!loading && totalPages > 1 && (
            <div className="flex justify-center items-center gap-4 mt-12 mb-8">
              <button
                onClick={() => setCurrentPage((prev) => Math.max(1, prev - 1))}
                disabled={currentPage === 1}
                className="px-4 py-2 bg-primary-dark border border-primary-light/20 text-primary-lightest rounded-md disabled:opacity-50 disabled:cursor-not-allowed hover:bg-primary-medium/20 transition-all"
              >
                Anterior
              </button>

              <span className="text-primary-light text-sm">
                Página {currentPage} de {totalPages}
              </span>

              <button
                onClick={() => setCurrentPage((prev) => Math.min(totalPages, prev + 1))}
                disabled={currentPage === totalPages}
                className="px-4 py-2 bg-primary-dark border border-primary-light/20 text-primary-lightest rounded-md disabled:opacity-50 disabled:cursor-not-allowed hover:bg-primary-medium/20 transition-all"
              >
                Siguiente
              </button>
            </div>
          )}
        </div>
      </section>
    </div>
  );
}