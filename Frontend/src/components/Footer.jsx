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
    <footer className="bg-primary-dark text-primary-lightest py-8">
      <div className="container mx-auto px-4">
        <div className="flex justify-around">
          {/* Company information section */}
          <div className="text-center">
            <h4 className="font-bold mb-2">ServiceHub</h4>
            <p className="text-sm">
              &copy; 2025 ServiceHub. All rights reserved.
            </p>
          </div>
          {/* Navigation links section */}
          <div className="text-center">
            <h4 className="font-bold mb-2">Navigation</h4>
            <ul>
              <li>
                <a href="#" className="hover:text-primary-accent">
                  Services
                </a>
              </li>
              <li>
                <a href="#" className="hover:text-primary-accent">
                  About
                </a>
              </li>
              <li>
                <a href="#" className="hover:text-primary-accent">
                  Contact
                </a>
              </li>
            </ul>
          </div>
          {/* Legal links section */}
          <div className="text-center">
            <h4 className="font-bold mb-2">Legal</h4>
            <ul>
              <li>
                <a href="#" className="hover:text-primary-accent">
                  Terms and Conditions
                </a>
              </li>
              <li>
                <a href="#" className="hover:text-primary-accent">
                  Privacy Policy
                </a>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </footer>
  );
}
