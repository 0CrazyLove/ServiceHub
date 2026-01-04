import React, { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import { registerUser } from '../../Services/api';

/**
 * User registration form component.
 * 
 * Provides account creation with email, username, password validation.
 * Includes password visibility toggle and strength requirements display.
 * Validates password confirmation and minimum requirements.
 * Handles registration errors and redirects on success.
 * 
 * @returns {JSX.Element} Registration form
 */
export default function RegisterForm() {
  const [formData, setFormData] = useState({
    userName: '',
    email: '',
    password: '',
    confirmPassword: ''
  });
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();

  /**
   * Handle form input changes.
   * 
   * @param {React.ChangeEvent<HTMLInputElement>} e - Input change event
   */
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value
    }));
  };

  /**
   * Handle form submission and user registration.
   * 
   * Validates password strength and confirmation before submission.
   * 
   * @param {React.FormEvent} e - Form submission event
   */
  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    // Validate passwords match
    if (formData.password !== formData.confirmPassword) {
      setError('Las contraseñas no coinciden');
      return;
    }

    // Validate password length
    if (formData.password.length < 6) {
      setError('La contraseña debe tener al menos 6 caracteres');
      return;
    }

    setLoading(true);

    try {
      const response = await registerUser(formData);

      // Store token and user data
      login(response, response.token, response.refreshToken);

      // Redirect to home
      setTimeout(() => {
        window.location.href = '/';
      }, 500);
    } catch (err) {
      // Custom error handling
      let errorMessage = 'Error durante el registro';

      if (err.message.includes('400') || err.message.includes('Bad Request')) {
        errorMessage =
          'Verifica que el correo no esté registrado y que los datos sean válidos.';
      } else if (err.message.includes('409') || err.message.includes('Conflict')) {
        errorMessage = 'Este correo ya está registrado. Prueba con otro.';
      } else if (err.message.includes('500')) {
        errorMessage = 'Error del servidor. Intenta nuevamente más tarde.';
      } else if (err.message) {
        errorMessage = err.message;
      }

      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  /**
   * Eye icon component for password visibility toggle.
   * @private
   */
  const EyeIcon = () => (
    <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
      <path d="M10 12a2 2 0 100-4 2 2 0 000 4z" />
      <path
        fillRule="evenodd"
        d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z"
        clipRule="evenodd"
      />
    </svg>
  );

  /**
   * Eye off icon component for password visibility toggle.
   * @private
   */
  const EyeOffIcon = () => (
    <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
      <path
        fillRule="evenodd"
        d="M3.707 2.293a1 1 0 00-1.414 1.414l14 14a1 1 0 001.414-1.414l-1.473-1.473A10.014 10.014 0 0019.542 10C18.268 5.943 14.478 3 10 3a9.958 9.958 0 00-4.512 1.074l-1.78-1.781zm4.261 4.26l1.514 1.515a2.003 2.003 0 012.45 2.45l1.514 1.514a4 4 0 00-5.478-5.478z"
        clipRule="evenodd"
      />
      <path d="M15.171 13.576l1.414 1.414A10.025 10.025 0 0020 10c-1.274-4.057-5.064-7-9.542-7a9.971 9.971 0 00-3.516.635l1.414 1.414a6 6 0 018.644 8.527z" />
    </svg>
  );

  return (
    <div className="w-full max-w-md mx-auto">
      <div className="bg-primary-dark rounded-lg shadow-lg p-8">
        <h2 className="text-2xl font-bold text-primary-lightest mb-6 text-center">
          Crear cuenta
        </h2>

        {/* Error message display */}
        {error && (
          <div className="bg-red-500 bg-opacity-20 border border-red-500 text-red-200 px-4 py-3 rounded-md mb-4">
            {error}
          </div>
        )}

        {/* Registration form */}
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label
              htmlFor="userName"
              className="block text-primary-light text-sm font-medium mb-2"
            >
              Usuario
            </label>
            <input
              type="text"
              id="userName"
              name="userName"
              value={formData.userName}
              onChange={handleChange}
              required
              className="w-full px-4 py-2 bg-primary-medium border border-primary-accent rounded-md text-primary-lightest placeholder-primary-light focus:outline-none focus:border-primary-lightest transition"
            />
          </div>

          <div>
            <label
              htmlFor="email"
              className="block text-primary-light text-sm font-medium mb-2"
            >
              Correo electrónico
            </label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
              className="w-full px-4 py-2 bg-primary-medium border border-primary-accent rounded-md text-primary-lightest placeholder-primary-light focus:outline-none focus:border-primary-lightest transition"
            />
          </div>

          <div>
            <label
              htmlFor="password"
              className="block text-primary-light text-sm font-medium mb-2"
            >
              Contraseña
            </label>
            <div className="relative">
              <input
                type={showPassword ? 'text' : 'password'}
                id="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                required
                className="w-full px-4 py-2 bg-primary-medium border border-primary-accent rounded-md text-primary-lightest placeholder-primary-light focus:outline-none focus:border-primary-lightest transition pr-10"
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute right-3 top-1/2 transform -translate-y-1/2 text-primary-light hover:text-primary-lightest transition"
                title={showPassword ? 'Hide password' : 'Show password'}
              >
                {showPassword ? <EyeIcon /> : <EyeOffIcon />}
              </button>
            </div>
            <p className="text-primary-light text-xs mt-1">
              Mínimo 6 caracteres, 1 mayúscula, 1 minúscula y 1 número
            </p>
          </div>

          <div>
            <label
              htmlFor="confirmPassword"
              className="block text-primary-light text-sm font-medium mb-2"
            >
              Confirmar contraseña
            </label>
            <div className="relative">
              <input
                type={showConfirmPassword ? 'text' : 'password'}
                id="confirmPassword"
                name="confirmPassword"
                value={formData.confirmPassword}
                onChange={handleChange}
                required
                className="w-full px-4 py-2 bg-primary-medium border border-primary-accent rounded-md text-primary-lightest placeholder-primary-light focus:outline-none focus:border-primary-lightest transition pr-10"
              />
              <button
                type="button"
                onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                className="absolute right-3 top-1/2 transform -translate-y-1/2 text-primary-light hover:text-primary-lightest transition"
                title={
                  showConfirmPassword ? 'Hide password' : 'Show password'
                }
              >
                {showConfirmPassword ? <EyeIcon /> : <EyeOffIcon />}
              </button>
            </div>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-primary-accent hover:bg-opacity-80 disabled:opacity-50 text-primary-lightest font-bold py-2 px-4 rounded-md transition"
          >
            {loading ? 'Registrando...' : 'Crear cuenta'}
          </button>
        </form>

        {/* Login link */}
        <p className="text-primary-light text-center mt-6">
          ¿Ya tienes una cuenta?{' '}
          <a href="/login" className="text-primary-accent hover:text-primary-lightest font-bold">
            Inicia sesión aquí
          </a>
        </p>
      </div>
    </div>
  );
}
