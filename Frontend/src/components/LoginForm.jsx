import React, { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import { loginUser, googleCallback } from '../../Services/api';
import GoogleLoginButton from './GoogleLoginButton';

/**
 * Login form component with email/password and Google OAuth support.
 * 
 * Provides two authentication methods:
 * - Email and password login with validation
 * - Google OAuth authentication using authorization code flow
 * 
 * Handles loading states, error messages, and password visibility toggle.
 * Redirects to home page after successful authentication.
 * Includes demo credentials for testing.
 * 
 * @returns {JSX.Element} Login form
 */
export default function LoginForm({ enableGoogle = false }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();

  /**
   * Handle email/password form submission.
   * 
   * @param {React.FormEvent} e - Form submission event
   */
  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const response = await loginUser({ email, password });
      login(response, response.token, response.refreshToken);

      setTimeout(() => {
        window.location.href = '/';
      }, 500);
    } catch (err) {
      let errorMessage = 'Error al iniciar sesión';

      if (err.message.includes('401') || err.message.includes('Unauthorized')) {
        errorMessage = 'Correo o contraseña inválidos. Intenta de nuevo.';
      } else if (err.message.includes('400') || err.message.includes('Bad Request')) {
        errorMessage = 'Verifica tu información.';
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
   * Handle Google OAuth success.
   */
  const handleGoogleSuccess = async (codeResponse) => {
    setError('');
    setLoading(true);
    try {
      const response = await googleCallback(codeResponse.code);

      // Store token and user data
      login(response, response.token, response.refreshToken);

      // Redirect to home
      setTimeout(() => {
        window.location.href = '/';
      }, 500);
    } catch (err) {
      console.error('Google login error:', err);
      setError('Error al iniciar sesión con Google. Por favor intenta nuevamente.');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Handle Google OAuth error.
   */
  const handleGoogleError = (error) => {
    console.error('Google OAuth error:', error);
    setError('Error al conectar con Google. Por favor intenta nuevamente.');
  };

  return (
    <div className="w-full max-w-md mx-auto">
      <div className="bg-primary-dark rounded-lg shadow-lg p-8">
        <h2 className="text-2xl font-bold text-primary-lightest mb-6 text-center">
          Iniciar sesión
        </h2>

        {/* Error message display */}
        {error && (
          <div className="bg-red-500 bg-opacity-20 border border-red-500 text-red-200 px-4 py-3 rounded-md mb-4">
            {error}
          </div>
        )}

        {/* Google OAuth button */}
        {enableGoogle && (
          <GoogleLoginButton
            onSuccess={handleGoogleSuccess}
            onError={handleGoogleError}
            loading={loading}
          />
        )}

        {/* Divider */}
        <div className="relative mb-6">
          <div className="absolute inset-0 flex items-center">
            <div className="w-full border-t border-primary-accent"></div>
          </div>
          <div className="relative flex justify-center text-sm">
            <span className="px-4 bg-primary-dark text-primary-light">
              O continuar con correo
            </span>
          </div>
        </div>

        {/* Email and password form */}
        <form onSubmit={handleSubmit} className="space-y-4">
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
              value={email}
              onChange={(e) => setEmail(e.target.value)}
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
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                className="w-full px-4 py-2 bg-primary-medium border border-primary-accent rounded-md text-primary-lightest placeholder-primary-light focus:outline-none focus:border-primary-lightest transition pr-10"
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute right-3 top-1/2 transform -translate-y-1/2 text-primary-light hover:text-primary-lightest transition"
                title={showPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'}
              >
                {showPassword ? (
                  <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M10 12a2 2 0 100-4 2 2 0 000 4z" />
                    <path
                      fillRule="evenodd"
                      d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z"
                      clipRule="evenodd"
                    />
                  </svg>
                ) : (
                  <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                    <path
                      fillRule="evenodd"
                      d="M3.707 2.293a1 1 0 00-1.414 1.414l14 14a1 1 0 001.414-1.414l-1.473-1.473A10.014 10.014 0 0019.542 10C18.268 5.943 14.478 3 10 3a9.958 9.958 0 00-4.512 1.074l-1.78-1.781zm4.261 4.26l1.514 1.515a2.003 2.003 0 012.45 2.45l1.514 1.514a4 4 0 00-5.478-5.478z"
                      clipRule="evenodd"
                    />
                    <path d="M15.171 13.576l1.414 1.414A10.025 10.025 0 0020 10c-1.274-4.057-5.064-7-9.542-7a9.971 9.971 0 00-3.516.635l1.414 1.414a6 6 0 018.644 8.527z" />
                  </svg>
                )}
              </button>
            </div>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-primary-accent hover:bg-opacity-80 disabled:opacity-50 text-primary-lightest font-bold py-2 px-4 rounded-md transition"
          >
            {loading ? 'Iniciando sesión...' : 'Iniciar sesión'}
          </button>
        </form>

        {/* Sign up link */}
        <p className="text-primary-light text-center mt-6">
          ¿No tienes una cuenta?{' '}
          <a href="/registro" className="text-primary-accent hover:text-primary-lightest font-bold">
            Regístrate aquí
          </a>
        </p>

        {/* Demo credentials */}
        <div className="mt-6 pt-6 border-t border-primary-accent">
          <p className="text-primary-light text-sm text-center mb-2">
            Credenciales de prueba:
          </p>
          <p className="text-primary-light text-xs text-center">
            Email: <code className="bg-primary-medium px-2 py-1 rounded">admin@example.com</code>
          </p>
          <p className="text-primary-light text-xs text-center">
            Contraseña: <code className="bg-primary-medium px-2 py-1 rounded">Admin123!</code>
          </p>
        </div>
      </div>
    </div>
  );
}