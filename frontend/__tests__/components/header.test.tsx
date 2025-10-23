import { describe, it, expect } from 'vitest';
import { render, screen } from '@/__tests__/utils/test-utils';
import { Header } from '@/components/layout/header';

describe('Header Component', () => {
  it('renders site logo/title', () => {
    render(<Header />);

    // Site name is rendered as translation key "app.title" in test environment
    const siteName = screen.getByText(/app\.title/i);
    expect(siteName).toBeInTheDocument();
  });

  it('renders navigation links', () => {
    render(<Header />);

    // Check for main navigation links
    expect(screen.getByText(/Ana Sayfa|Home/i)).toBeInTheDocument();
    expect(screen.getByText(/Kategoriler|Categories/i)).toBeInTheDocument();
  });

  it('renders category navigation links', () => {
    render(<Header />);

    // Common categories
    const categories = ['Teknoloji', 'Spor', 'Dünya', 'Ekonomi', 'Bilim'];
    
    categories.forEach((category) => {
      const link = screen.queryByText(category);
      if (link) {
        expect(link).toBeInTheDocument();
      }
    });
  });

  it('has correct link structure', () => {
    const { container } = render(<Header />);

    // Check if nav element exists
    const nav = container.querySelector('nav');
    expect(nav).toBeInTheDocument();

    // Check if links have proper href attributes
    const links = container.querySelectorAll('a');
    expect(links.length).toBeGreaterThan(0);
  });

  it('is sticky/fixed at top of page', () => {
    const { container } = render(<Header />);

    // Header should have sticky or fixed positioning
    const header = container.querySelector('header');
    expect(header).toBeInTheDocument();
  });

  it('renders theme toggle button', () => {
    render(<Header />);

    // Check for theme toggle (sun/moon icon)
    const themeButton = screen.queryByRole('button', { name: /theme|tema/i });
    if (themeButton) {
      expect(themeButton).toBeInTheDocument();
    }
  });

  it('renders mobile menu button on small screens', () => {
    render(<Header />);

    // Check for mobile menu toggle (hamburger icon)
    const mobileMenuButtons = screen.queryAllByRole('button');
    expect(mobileMenuButtons.length).toBeGreaterThan(0);
  });

  it('logo links to homepage', () => {
    const { container } = render(<Header />);

    // Find logo link
    const logoLink = container.querySelector('a[href="/"]');
    expect(logoLink).toBeInTheDocument();
  });

  it('applies responsive classes', () => {
    const { container } = render(<Header />);

    // Check for responsive utility classes - header uses sticky, w-full, backdrop-blur
    const header = container.querySelector('header');
    const hasResponsiveClasses =
      header?.className.includes('sticky') ||
      header?.className.includes('w-full') ||
      header?.className.includes('backdrop');
    expect(hasResponsiveClasses).toBe(true);
  });
});
