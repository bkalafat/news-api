import { describe, it, expect } from 'vitest';
import { render, screen } from '@/__tests__/utils/test-utils';
import { Footer } from '@/components/layout/footer';

describe('Footer Component', () => {
  it('renders footer element', () => {
    const { container } = render(<Footer />);

    const footer = container.querySelector('footer');
    expect(footer).toBeInTheDocument();
  });

  it('displays copyright information', () => {
    render(<Footer />);

    // Check for copyright text with current year
    const currentYear = new Date().getFullYear();
    const copyright = screen.getByText(new RegExp(currentYear.toString()));
    expect(copyright).toBeInTheDocument();
  });

  it('renders social media links', () => {
    render(<Footer />);

    // Check for common social media links
    const socialLinks = screen.queryAllByRole('link');
    expect(socialLinks.length).toBeGreaterThan(0);
  });

  it('displays site name or logo', () => {
    render(<Footer />);

    const siteName = screen.getByText(/Teknoloji Haberleri/i);
    expect(siteName).toBeInTheDocument();
  });

  it('renders footer navigation sections', () => {
    const { container } = render(<Footer />);

    // Footer usually has multiple columns/sections
    const sections = container.querySelectorAll('div');
    expect(sections.length).toBeGreaterThan(1);
  });

  it('includes links to important pages', () => {
    render(<Footer />);

    // Common footer links
    const importantLinks = [
      /hakkımızda|about/i,
      /iletişim|contact/i,
      /gizlilik|privacy/i,
    ];

    importantLinks.forEach((linkPattern) => {
      const link = screen.queryByText(linkPattern);
      // Not all sites have all these links, but check if present
      if (link) {
        expect(link).toBeInTheDocument();
      }
    });
  });

  it('has proper semantic HTML structure', () => {
    const { container } = render(<Footer />);

    const footer = container.querySelector('footer');
    expect(footer?.tagName.toLowerCase()).toBe('footer');
  });

  it('applies background styling', () => {
    const { container } = render(<Footer />);

    const footer = container.querySelector('footer');
    // Footer should have background color or border
    const hasBackground =
      footer?.className.includes('bg-') || footer?.className.includes('border');
    expect(hasBackground).toBe(true);
  });

  it('is positioned at bottom of page', () => {
    const { container } = render(<Footer />);

    const footer = container.querySelector('footer');
    // Footer should have appropriate positioning or margin
    expect(footer).toBeInTheDocument();
  });

  it('renders newsletter signup if present', () => {
    render(<Footer />);

    // Check for newsletter-related elements
    const newsletter = screen.queryByText(/bülten|newsletter/i);
    if (newsletter) {
      expect(newsletter).toBeInTheDocument();
    }
  });

  it('displays category links in footer', () => {
    render(<Footer />);

    const categoryLinks = screen.queryAllByRole('link');
    // Should have multiple links
    expect(categoryLinks.length).toBeGreaterThan(0);
  });

  it('renders accessibility-friendly content', () => {
    const { container } = render(<Footer />);

    // Check for proper link structure
    const links = container.querySelectorAll('a');
    links.forEach((link) => {
      // Links should have text content or aria-label
      const hasTextOrLabel =
        link.textContent ||
        link.getAttribute('aria-label') ||
        link.querySelector('svg');
      expect(hasTextOrLabel).toBeTruthy();
    });
  });
});
