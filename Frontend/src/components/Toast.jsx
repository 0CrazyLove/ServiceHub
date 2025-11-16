import { useEffect } from 'react';

/**
 * Individual toast notification component.
 * 
 * Displays a notification message with automatic dismissal after a configured duration.
 * Supports three visual types: info (blue), success (green), error (red).
 * Can be manually dismissed via the close button.
 * 
 * Note: This component is primarily used internally by ToastContext.
 * For displaying toasts throughout the app, use the useToast hook instead.
 * 
 * @param {Object} props - Component props
 * @param {string} props.message - Notification message to display
 * @param {string} [props.type='info'] - Notification type ('info', 'success', or 'error')
 * @param {Function} [props.onClose] - Callback when notification should close
 * @param {number} [props.duration=4000] - Milliseconds before auto-dismiss
 * @returns {JSX.Element|null} Toast notification or null if no message
 */
export default function Toast({ message, type = 'info', onClose, duration = 4000 }) {
  /**
   * Set up automatic dismissal timer.
   * Clears the timer on unmount or if message changes.
   */
  useEffect(() => {
    if (!message) return;
    const t = setTimeout(() => onClose && onClose(), duration);
    return () => clearTimeout(t);
  }, [message, duration, onClose]);

  if (!message) return null;

  // Determine background color based on notification type
  const color =
    type === 'error'
      ? 'bg-red-600'
      : type === 'success'
        ? 'bg-green-600'
        : 'bg-blue-600';

  return (
    <div
      className={`fixed bottom-6 right-6 z-50 ${color} text-white px-4 py-3 rounded shadow-md`}
    >
      <div className="flex items-center justify-between gap-4">
        <div>{message}</div>
        <button
          onClick={onClose}
          className="opacity-80 hover:opacity-100"
          aria-label="Dismiss notification"
        >
          ✕
        </button>
      </div>
    </div>
  );
}
