const API_URL = 'http://localhost:5097/api';

// Helper para manejar respuestas del API
async function handleResponse(response) {
  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'Error en la petición');
  }
  return response.json();
}

// ===== SERVICIOS =====

/**
 * Obtener todos los servicios con paginación y filtros
 * @param {Object} params - Parámetros de consulta
 * @param {string} params.category - Categoría opcional
 * @param {number} params.page - Número de página (default: 1)
 * @param {number} params.pageSize - Tamaño de página (default: 10)
 * @param {number} params.minPrice - Precio mínimo opcional
 * @param {number} params.maxPrice - Precio máximo opcional
 */
export async function getServices({ category = '', page = 1, pageSize = 10, minPrice = null, maxPrice = null } = {}) {
  try {
    const params = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
    });
    
    if (category) {
      params.append('category', category);
    }
    
    if (minPrice !== null) {
      params.append('minPrice', minPrice.toString());
    }
    
    if (maxPrice !== null) {
      params.append('maxPrice', maxPrice.toString());
    }
    
    const response = await fetch(`${API_URL}/services?${params}`);
    return await handleResponse(response);
  } catch (error) {
    console.error('Error al obtener servicios:', error);
    throw error;
  }
}

/**
 * Obtener un servicio por ID
 * @param {number} id - ID del servicio
 */
export async function getServiceById(id) {
  try {
    const response = await fetch(`${API_URL}/services/${id}`);
    return await handleResponse(response);
  } catch (error) {
    console.error(`Error al obtener servicio ${id}:`, error);
    throw error;
  }
}

/**
 * Obtener todas las categorías de servicios
 */
export async function getServiceCategories() {
  try {
    const response = await fetch(`${API_URL}/services/categories`);
    return await handleResponse(response);
  } catch (error) {
    console.error('Error al obtener categorías:', error);
    throw error;
  }
}

/**
 * Crear un nuevo servicio (requiere autenticación de Admin)
 * @param {Object} serviceData - Datos del servicio
 * @param {string} token - Token JWT de autenticación
 */
export async function createService(serviceData, token) {
  try {
    const response = await fetch(`${API_URL}/services`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(serviceData)
    });
    return await handleResponse(response);
  } catch (error) {
    console.error('Error al crear servicio:', error);
    throw error;
  }
}

/**
 * Actualizar un servicio (requiere autenticación de Admin)
 * @param {number} id - ID del servicio
 * @param {Object} serviceData - Datos actualizados
 * @param {string} token - Token JWT de autenticación
 */
export async function updateService(id, serviceData, token) {
  try {
    const response = await fetch(`${API_URL}/services/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(serviceData)
    });
    
    if (response.status === 204 || response.status === 200) {
      return { success: true };
    }
    return await handleResponse(response);
  } catch (error) {
    console.error(`Error al actualizar servicio ${id}:`, error);
    throw error;
  }
}

/**
 * Eliminar un servicio (requiere autenticación de Admin)
 * @param {number} id - ID del servicio
 * @param {string} token - Token JWT de autenticación
 */
export async function deleteService(id, token) {
  try {
    const response = await fetch(`${API_URL}/services/${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });
    
    if (response.status === 204) {
      return { success: true };
    }
    return await handleResponse(response);
  } catch (error) {
    console.error(`Error al eliminar servicio ${id}:`, error);
    throw error;
  }
}

// ===== AUTENTICACIÓN =====

/**
 * Registrar un nuevo usuario
 * @param {Object} data - Datos del registro
 * @param {string} data.userName - Nombre de usuario
 * @param {string} data.email - Email del usuario
 * @param {string} data.password - Contraseña
 * @param {string} data.confirmPassword - Confirmación de contraseña
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
    console.error('Error al registrar:', error);
    throw error;
  }
}

/**
 * Login de usuario
 * @param {Object} data - Datos del login
 * @param {string} data.email - Email del usuario
 * @param {string} data.password - Contraseña
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
    console.error('Error al iniciar sesión:', error);
    throw error;
  }
}

// ===== DASHBOARD =====

/**
 * Obtener estadísticas del dashboard (requiere token Admin)
 * @param {string} token - Token JWT del usuario con permisos Admin
 */
export async function getDashboardStats(token) {
  try {
    const headers = {};
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(`${API_URL}/dashboard/stats`, {
      method: 'GET',
      headers
    });

    return await handleResponse(response);
  } catch (error) {
    console.error('Error al obtener estadísticas del dashboard:', error);
    throw error;
  }
}