/**
 * Tailwind CSS Configuration
 * 
 * Defines the design system for the ServiceHub application including:
 * - Extended color palette with primary color variations from darkest to lightest
 * - Scans all Astro and React component files for class detection
 * - Provides semantic color names for consistent UI theming
 * 
 * Color Scheme:
 * - Primary Darkest (#06141B): Deep navy for primary backgrounds
 * - Primary Dark (#11212D): Slightly lighter navy for secondary elements
 * - Primary Medium (#253745): Medium shade for accents and borders
 * - Primary Accent (#4A5C6A): Accent color for highlights
 * - Primary Light (#9BA8AB): Light shade for text and outlines
 * - Primary Lightest (#CCD0CF): Very light for subtle backgrounds
 */

/** @type {import('tailwindcss').Config} */
export default {
	content: ['./src/**/*.{astro,html,js,jsx,md,mdx,svelte,ts,tsx,vue}'],
	theme: {
		extend: {
			colors: {
				'primary-darkest': '#313131',
				'primary-dark': '#414141',
				'primary-medium': '#525252',
				'primary-accent': '#EC625F',
				'primary-light': '#D4D4D4',
				'primary-lightest': '#FAFAFA',
			},
			fontFamily: {
				sans: ['Poppins', 'sans-serif'],
			},
		},
	},
	plugins: [],
}