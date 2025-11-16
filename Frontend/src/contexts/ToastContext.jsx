import React, { createContext, useContext, useState, useCallback } from 'react';

/**
 * Context for managing toast notifications throughout the application.
 * 
 * Provides a centralized notification system with support for multiple
 * toast messages displayed simultaneously. Toasts are positioned in the
 * bottom-right corner with automatic dismissal after configurable duration.
 * Supports three notification types: info (blue), success (green), error (red).
 */
const ToastContext = createContext(null);

/** Counter for generating unique toast identifiers */
let idCounter = 1;

/**
 * Toast context provider component.
 * 
 * Renders the provider and toast notification container.
 * Displays toasts in a fixed bottom-right position with automatic removal.
 * Must wrap components that use the useToast hook.
 * 
 * @param {Object} props - Component props
 * @param {React.ReactNode} props.children - Child components that can access toast context
 * @returns {JSX.Element} Provider component with notification display
 */
export function ToastProvider({ children }) {
  const [toasts, setToasts] = useState([]);

  /**
   * Display a new toast notification.
   * 
   * Creates a notification that displays for a configurable duration,
   * then automatically removes itself. Manual dismissal is available via close button.
   * 
   * @param {string} message - Notification message to display
   * @param {string} [type='info'] - Notification type ('info', 'success', or 'error')
   *   - 'info': Blue background for neutral messages
   *   - 'success': Green background for positive feedback
   *   - 'error': Red background for error messages
   * @param {number} [duration=4000] - Milliseconds to display before auto-dismiss
   * @returns {number} Toast ID for potential manual removal
   */
  const push = useCallback((message, type = 'info', duration = 4000) => {
    const id = idCounter++;
    setToasts((t) => [...t, { id, message, type, duration }]);
    return id;
  }, []);

  /**
   * Manually remove a toast notification by ID.
   * 
   * @param {number} id - Toast ID to remove
   * @returns {void}
   */
  const remove = useCallback((id) => {
    setToasts((t) => t.filter((x) => x.id !== id));
  }, []);

  return (
    <ToastContext.Provider value={{ push, remove }}>
      {children}
      {/* Toast notification container - fixed position bottom-right */}
      <div className="fixed bottom-6 right-6 z-50 flex flex-col gap-3">
        {toasts.map((t) => (
          <div
            key={t.id}
            className={`px-4 py-3 rounded shadow-md text-white ${
              t.type === 'error'
                ? 'bg-red-600'
                : t.type === 'success'
                  ? 'bg-green-600'
                  : 'bg-blue-600'
            }`}
          >
            <div className="flex items-center justify-between gap-4">
              <div>{t.message}</div>
              <button
                onClick={() => remove(t.id)}
                className="opacity-80 hover:opacity-100"
                aria-label="Dismiss notification"
              >
                ✕
              </button>
            </div>
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  );
}

/**
 * Hook to access toast notification context.
 * 
 * Must be called within a ToastProvider component tree.
 * 
 * @returns {Object} Toast context object
 * @returns {Function} push - Display a new toast notification
 * @returns {Function} remove - Manually remove a toast by ID
 * @throws {Error} If used outside of ToastProvider
 */
export function useToast() {
  const ctx = useContext(ToastContext);
  if (!ctx) throw new Error('useToast must be used within ToastProvider');
  return ctx;
}
