import React from 'react';

/**
 * Hero banner section component.
 * 
 * Displays a prominent banner at the top of the page with call-to-action
 * and search functionality for finding services.
 * Uses dark primary color scheme with centered layout.
 * 
 * @returns {JSX.Element} Hero section
 */
export default function Hero() {
  return (
    <section className="bg-primary-darkest text-primary-lightest">
      <div className="container mx-auto px-4 py-20 text-center">
        <h2 className="text-4xl font-bold mb-4">
          Find the best services
        </h2>
        <p className="text-lg mb-8">
          Connecting professionals with clients worldwide
        </p>
        <div className="flex justify-center">
          <input
            type="text"
            placeholder="What service are you looking for?"
            className="w-1/2 px-4 py-2 rounded-l-md bg-primary-dark text-primary-lightest border border-primary-medium focus:outline-none focus:border-primary-accent"
            aria-label="Search services"
          />
          <button className="bg-primary-accent hover:bg-opacity-80 text-white px-6 py-2 rounded-r-md font-semibold">
            Search
          </button>
        </div>
      </div>
    </section>
  );
}
