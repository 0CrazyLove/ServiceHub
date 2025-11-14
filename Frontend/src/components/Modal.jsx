import React from 'react';

export default function Modal({ open, title, children, onClose, footer }) {
  if (!open) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div className="bg-primary-dark rounded-lg shadow-xl max-w-lg w-full mx-4">
        <div className="flex items-center justify-between px-6 py-4 border-b border-primary-medium">
          <h3 className="text-lg font-semibold text-primary-lightest">{title}</h3>
          <button onClick={onClose} className="text-primary-light hover:text-white">✕</button>
        </div>
        <div className="px-6 py-4 text-primary-light">{children}</div>
        {footer && <div className="px-6 py-4 border-t border-primary-medium">{footer}</div>}
      </div>
    </div>
  );
}
