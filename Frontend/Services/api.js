/**
 * ServiceHub API Client
 * 
 * Central service module for all HTTP communication with the ServiceHub backend API.
 * Handles services, authentication, orders, and dashboard operations with consistent
 * error handling and JWT token management.
 * 
 * API Base URL: http://localhost:5097/api
 */

const API_URL = import.meta.env.PUBLIC_API_URL;

/**
 * Handle API responses and throw errors if the response is not successful.
 * 
 * @param {Response} response - The fetch Response object
 * @returns {Promise<any>} Parsed JSON response body
 * @throws {Error} If the response is not OK
 * @private
 */
async function handleResponse(response) {
  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'API request failed');
  }
  return response.json();
}

/**
 * Wrapper around fetch to handle authentication and token refreshing.
 * 
 * @param {string} url - The URL to fetch
 * @param {Object} options - Fetch options
 * @param {string} [token] - Optional override token
 * @returns {Promise<any>} Parsed JSON response
 */
async function fetchWithAuth(url, options = {}, token = null) {
  const headers = { ...options.headers };

  // Use passed token or get from localStorage
  let authToken = token || localStorage.getItem('authToken');

  if (authToken) {
    headers['Authorization'] = `Bearer ${authToken}`;
  }

  // Ensure Content-Type is set for JSON bodies if not already
  if (options.body && !headers['Content-Type'] && typeof options.body === 'string') {
    headers['Content-Type'] = 'application/json';
  }

  const response = await fetch(url, { ...options, headers });

  if (response.status === 401) {
    // Try to refresh
    try {
      const refreshToken = localStorage.getItem('refreshToken');
      if (!refreshToken) throw new Error('No refresh token available');

      const refreshResponse = await fetch(`${API_URL}/auth/refresh-token`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken })
      });

      if (!refreshResponse.ok) throw new Error('Refresh failed');

      const data = await refreshResponse.json();

      // Update tokens
      localStorage.setItem('authToken', data.token);
      localStorage.setItem('refreshToken', data.refreshToken);

      // Dispatch event for useAuth
      window.dispatchEvent(new Event('auth:token-updated'));

      // Retry original request with new token
      headers['Authorization'] = `Bearer ${data.token}`;
      const retryResponse = await fetch(url, { ...options, headers });
      return await handleResponse(retryResponse);

    } catch (error) {
      console.error('Token refresh failed:', error);
      // Clear auth state
      localStorage.removeItem('authToken');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('authUser');
      throw error;
    }
  }

  return await handleResponse(response);
}

/**
 * SERVICES OPERATIONS
 */

/**
 * Retrieve a paginated list of services with optional filtering.
 * 
 * @param {Object} options - Query parameters
 * @param {string} [options.category=''] - Filter by service category
 * @param {number} [options.page=1] - Page number for pagination
 * @param {number} [options.pageSize=10] - Number of items per page
 * @param {number} [options.minPrice=null] - Minimum price filter
 * @param {number} [options.maxPrice=null] - Maximum price filter
 * @returns {Promise<Object>} Paginated services response containing items, pagination metadata
 */
export async function getServices({ category = '', page = 1, pageSize = 10, minPrice = null, maxPrice = null } = {}) {
  try {
    const params = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
    });

    if (category) params.append('category', category);
    if (minPrice !== null) params.append('minPrice', minPrice.toString());
    if (maxPrice !== null) params.append('maxPrice', maxPrice.toString());

    const response = await fetch(`${API_URL}/services?${params}`);
    return await handleResponse(response);
  } catch (error) {
    console.error('Error fetching services:', error);
    throw error;
  }
}

/**
 * Retrieve details for a specific service by ID.
 * 
 * @param {number} id - The service ID
 * @returns {Promise<Object>} Service details including pricing, ratings, and availability
 */
export async function getServiceById(id) {
  try {
    const response = await fetch(`${API_URL}/services/${id}`);
    return await handleResponse(response);
  } catch (error) {
    console.error(`Error fetching service ${id}:`, error);
    throw error;
  }
}

/**
 * Retrieve all available service categories.
 * 
 * @returns {Promise<Array<string>>} Array of category names
 */
export async function getServiceCategories() {
  try {
    const response = await fetch(`${API_URL}/services/categories`);
    return await handleResponse(response);
  } catch (error) {
    console.error('Error fetching service categories:', error);
    throw error;
  }
}

/**
 * Create a new service (requires admin authentication).
 * 
 * @param {Object} serviceData - Service details
 * @param {string} token - JWT authentication token
 * @returns {Promise<Object>} Created service object with assigned ID
 */
export async function createService(serviceData, token) {
  try {
    return await fetchWithAuth(`${API_URL}/services`, {
      method: 'POST',
      body: JSON.stringify(serviceData)
    }, token);
  } catch (error) {
    console.error('Error creating service:', error);
    throw error;
  }
}

/**
 * Update an existing service (requires admin authentication).
 * 
 * @param {number} id - The service ID to update
 * @param {Object} serviceData - Updated service details
 * @param {string} token - JWT authentication token
 * @returns {Promise<Object>} Updated service object
 */
export async function updateService(id, serviceData, token) {
  try {
    const response = await fetchWithAuth(`${API_URL}/services/${id}`, {
      method: 'PUT',
      body: JSON.stringify(serviceData)
    }, token);

    if (response && (response.success || response.id)) {
      return { success: true };
    }
    return response;
  } catch (error) {
    console.error(`Error updating service ${id}:`, error);
    throw error;
  }
}

/**
 * Delete a service (requires admin authentication).
 * 
 * @param {number} id - The service ID to delete
 * @param {string} token - JWT authentication token
 * @returns {Promise<Object>} Success response
 */
export async function deleteService(id, token) {
  try {
    const response = await fetchWithAuth(`${API_URL}/services/${id}`, {
      method: 'DELETE'
    }, token);

    return { success: true };
  } catch (error) {
    console.error(`Error deleting service ${id}:`, error);
    throw error;
  }
}

/**
 * AUTHENTICATION OPERATIONS
 */

/**
 * Register a new user account with email and password.
 * 
 * @param {Object} data - Registration data
 * @param {string} data.userName - Desired username
 * @param {string} data.email - User email address
 * @param {string} data.password - User password
 * @param {string} data.confirmPassword - Password confirmation
 * @returns {Promise<Object>} Authentication response with JWT token and user info
 */
export async function registerUser(data) {
  try {
    const response = await fetch(`${API_URL}/auth/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        userName: data.userName,
        email: data.email,
        password: data.password,
        confirmPassword: data.confirmPassword
      })
    });
    return await handleResponse(response);
  } catch (error) {
    console.error('Error registering user:', error);
    throw error;
  }
}

/**
 * Authenticate a user with email and password credentials.
 * 
 * @param {Object} data - Login credentials
 * @param {string} data.email - User email
 * @param {string} data.password - User password
 * @returns {Promise<Object>} Authentication response with JWT token and user info
 */
export async function loginUser(data) {
  try {
    const response = await fetch(`${API_URL}/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        email: data.email,
        password: data.password
      })
    });
    return await handleResponse(response);
  } catch (error) {
    console.error('Error logging in user:', error);
    throw error;
  }
}

/**
 * GOOGLE OAUTH OPERATIONS
 */

/**
 * Handle Google OAuth callback by exchanging authorization code for JWT token.
 * 
 * Called after user completes Google Sign-In. Exchanges the authorization code
 * for backend JWT token and creates/updates user account as needed.
 * 
 * @param {string} code - Authorization code from Google OAuth flow
 * @returns {Promise<Object>} Authentication response with JWT token and user info
 */
export async function googleCallback(code) {
  try {
    const response = await fetch(`${API_URL}/auth/google/callback`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ code })
    });
    return await handleResponse(response);
  } catch (error) {
    console.error('Error in Google OAuth callback:', error);
    throw error;
  }
}

/**
 * DASHBOARD OPERATIONS
 */

/**
 * Retrieve dashboard statistics (admin only).
 * 
 * Fetches aggregated platform metrics including total sales, service count, and order count.
 * 
 * @param {string} token - JWT authentication token
 * @returns {Promise<Object>} Dashboard stats including totalSales, serviceCount, orderCount
 */
export async function getDashboardStats(token) {
  try {
    return await fetchWithAuth(`${API_URL}/dashboard/stats`, {
      method: 'GET'
    }, token);
  } catch (error) {
    console.error('Error fetching dashboard statistics:', error);
    throw error;
  }
}

/**
 * ORDERS OPERATIONS
 */

/**
 * Retrieve all orders for the authenticated user.
 * 
 * Requires authentication. Returns list of orders with associated items and services.
 * 
 * @param {string} token - JWT authentication token
 * @returns {Promise<Array<Object>>} Array of order objects
 */
export async function getOrders(token) {
  try {
    return await fetchWithAuth(`${API_URL}/orders`, {
      method: 'GET'
    }, token);
  } catch (error) {
    console.error('Error fetching orders:', error);
    throw error;
  }
}

/**
 * Create a new order with the specified services.
 * 
 * Requires authentication with Customer role. Services must be available and exist.
 * 
 * @param {Object} orderDto - Order data containing array of items
 * @param {Array<Object>} orderDto.orderItems - Items to order (serviceId and quantity)
 * @param {string} token - JWT authentication token
 * @returns {Promise<Object>} Created order with assigned ID and total amount
 */
export async function createOrder(orderDto, token) {
  try {
    return await fetchWithAuth(`${API_URL}/orders`, {
      method: 'POST',
      body: JSON.stringify(orderDto)
    }, token);
  } catch (error) {
    console.error('Error creating order:', error);
    throw error;
  }
}