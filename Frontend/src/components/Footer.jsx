import React from 'react';

/**
 * Application footer component.
 * 
 * Displays footer navigation with company information, links, and legal notices.
 * Uses a three-column layout with centered text alignment.
 * Renders consistently across all pages using the primary dark color scheme.
 * 
 * @returns {JSX.Element} Footer element
 */
export default function Footer() {
  return (
    <footer className="bg-primary-darkest text-primary-lightest py-6 border-t border-primary-medium/30">
      <div className="container mx-auto px-4">
        <div className="flex flex-col md:flex-row justify-between items-center gap-4">
          {/* Company information section */}
          <div className="text-center md:text-left">
            <h4 className="font-bold text-lg tracking-tight">ServiceHub</h4>
            <p className="text-xs text-primary-light/60 mt-1">
              &copy; 2025 ServiceHub. Todos los derechos reservados.
            </p>
          </div>

          {/* Navigation links section */}
          <nav>
            <ul className="flex flex-wrap justify-center md:justify-end gap-6 text-sm font-medium">
              <li>
                <a href="#" className="hover:text-primary-accent transition-colors duration-200">
                  Servicios
                </a>
              </li>
              <li>
                <a href="#" className="hover:text-primary-accent transition-colors duration-200">
                  Acerca de
                </a>
              </li>
              <li>
                <a href="#" className="hover:text-primary-accent transition-colors duration-200">
                  Contacto
                </a>
              </li>
              <li>
                <a href="#" className="hover:text-primary-accent transition-colors duration-200">
                  Términos
                </a>
              </li>
              <li>
                <a href="#" className="hover:text-primary-accent transition-colors duration-200">
                  Privacidad
                </a>
              </li>
            </ul>
          </nav>
        </div>
      </div>
    </footer>
  );
}
