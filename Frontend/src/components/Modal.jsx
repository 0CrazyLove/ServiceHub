import React from 'react';

/**
 * Modal dialog component.
 * 
 * Displays a centered modal overlay with customizable title, content, and footer.
 * Features a semi-transparent dark overlay behind the modal dialog.
 * Only renders when the open prop is true.
 * 
 * @param {Object} props - Component props
 * @param {boolean} props.open - Whether the modal is visible
 * @param {string} props.title - Modal header title
 * @param {React.ReactNode} props.children - Modal body content
 * @param {Function} props.onClose - Callback when close button is clicked
 * @param {React.ReactNode} [props.footer] - Optional modal footer content
 * @returns {JSX.Element|null} Modal dialog or null if not open
 */
export default function Modal({ open, title, children, onClose, footer }) {
  if (!open) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div className="bg-primary-dark rounded-lg shadow-xl max-w-lg w-full mx-4">
        {/* Modal header with title and close button */}
        <div className="flex items-center justify-between px-6 py-4 border-b border-primary-medium">
          <h3 className="text-lg font-semibold text-primary-lightest">{title}</h3>
          <button
            onClick={onClose}
            className="text-primary-light hover:text-white"
            aria-label="Close modal"
          >
            ✕
          </button>
        </div>
        {/* Modal body content */}
        <div className="px-6 py-4 text-primary-light">{children}</div>
        {/* Optional modal footer */}
        {footer && (
          <div className="px-6 py-4 border-t border-primary-medium">{footer}</div>
        )}
      </div>
    </div>
  );
}
