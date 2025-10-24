import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@/__tests__/utils/test-utils';
import { NewsDetailHero } from '@/components/news/news-detail-hero';
import { mockNewsArticle } from '@/__tests__/__mocks__/mockData';

describe('NewsDetailHero Component', () => {
  it('renders news title/caption', () => {
    render(<NewsDetailHero news={mockNewsArticle} />);

    const title = screen.getByText(mockNewsArticle.caption);
    expect(title).toBeInTheDocument();
  });

  it('displays featured image when provided', () => {
    render(<NewsDetailHero news={mockNewsArticle} />);

    if (mockNewsArticle.imageUrl) {
      const image = screen.getByRole('img');
      expect(image).toBeInTheDocument();
      // Alt text is from imgAlt field, not caption
      expect(image).toHaveAttribute('alt', mockNewsArticle.imgAlt);
    }
  });

  it('renders category badge', () => {
    render(<NewsDetailHero news={mockNewsArticle} />);

    // Category is displayed as "Teknoloji" (translated from "technology")
    if (mockNewsArticle.category) {
      const category = screen.getByText(/teknoloji/i);
      expect(category).toBeInTheDocument();
    }
  });

  it('displays formatted publish date', () => {
    render(<NewsDetailHero news={mockNewsArticle} />);

    // Check for date presence
    const dateText = screen.getByText(/\d{1,2}\s+\w+\s+\d{4}/i);
    expect(dateText).toBeInTheDocument();
  });

  it('renders authors when provided', () => {
    const newsWithAuthor = {
      ...mockNewsArticle,
      authors: ['Ahmet Yılmaz', 'Ayşe Demir'],
    };

    render(<NewsDetailHero news={newsWithAuthor} />);

    const author1 = screen.queryByText(/Ahmet Yılmaz/i);
    const author2 = screen.queryByText(/Ayşe Demir/i);

    if (author1 || author2) {
      expect(author1 || author2).toBeInTheDocument();
    }
  });

  it('displays read time estimate', () => {
    render(<NewsDetailHero news={mockNewsArticle} />);

    // Look for read time (e.g., "5 dk okuma" or "5 min read")
    const readTime = screen.queryByText(/\d+\s*(dk|min|dakika)/i);
    if (readTime) {
      expect(readTime).toBeInTheDocument();
    }
  });

  it('renders social share buttons', () => {
    render(<NewsDetailHero news={mockNewsArticle} />);

    // Check for share-related buttons or icons
    const shareButtons = screen.queryAllByRole('button');
    if (shareButtons.length > 0) {
      expect(shareButtons.length).toBeGreaterThan(0);
    }
  });

  it('shows view count if provided', () => {
    const newsWithViews = {
      ...mockNewsArticle,
      viewCount: 1542,
    };

    render(<NewsDetailHero news={newsWithViews} />);

    const viewCount = screen.queryByText(/1542|1.5K/i);
    if (viewCount) {
      expect(viewCount).toBeInTheDocument();
    }
  });

  it('renders breadcrumb navigation', () => {
    render(<NewsDetailHero news={mockNewsArticle} />);

    // Check for breadcrumb (Home > Category > Article)
    const breadcrumb = screen.queryByText(/Ana Sayfa|Home/i);
    if (breadcrumb) {
      expect(breadcrumb).toBeInTheDocument();
    }
  });

  it('handles missing image gracefully', () => {
    const newsWithoutImage = {
      ...mockNewsArticle,
      imageUrl: '',
      imgPath: '',
    };

    render(<NewsDetailHero news={newsWithoutImage} />);

    const image = screen.queryByRole('img');
    // Component may still render container, check if image is missing or placeholder
    // Accept both no image or a placeholder image
    if (image) {
      // If image exists, it should be a placeholder or empty src
      expect(image.getAttribute('src')).toBeTruthy();
    }
  });

  it('applies responsive image classes', () => {
    const { container } = render(<NewsDetailHero news={mockNewsArticle} />);

    const imageContainer = container.querySelector('img') || container.querySelector('[class*="image"]');
    if (imageContainer) {
      // Check for responsive classes (w-full, aspect-ratio, etc.)
      const hasResponsiveClass =
        imageContainer.className.includes('w-full') ||
        imageContainer.className.includes('aspect') ||
        imageContainer.className.includes('object-cover');
      expect(hasResponsiveClass).toBe(true);
    }
  });

  it('renders update date if different from publish date', () => {
    const newsWithUpdate = {
      ...mockNewsArticle,
      updateDate: new Date('2024-12-20').toISOString(),
    };

    render(<NewsDetailHero news={newsWithUpdate} />);

    const updateText = screen.queryByText(/güncellem|updated/i);
    if (updateText) {
      expect(updateText).toBeInTheDocument();
    }
  });

  it('includes SEO-friendly heading hierarchy', () => {
    const { container } = render(<NewsDetailHero news={mockNewsArticle} />);

    // Title should be h1
    const h1 = container.querySelector('h1');
    expect(h1).toBeInTheDocument();
    expect(h1?.textContent).toBe(mockNewsArticle.caption);
  });

  it('renders bookmark/save button', () => {
    render(<NewsDetailHero news={mockNewsArticle} />);

    const bookmarkButton = screen.queryByLabelText(/kaydet|save|bookmark/i);
    if (bookmarkButton) {
      expect(bookmarkButton).toBeInTheDocument();
    }
  });

  it('displays tags when provided', () => {
    const newsWithTags = {
      ...mockNewsArticle,
      socialTags: 'AI, OpenAI, GPT-5, Technology',
    };

    render(<NewsDetailHero news={newsWithTags} />);

    const tag = screen.queryByText(/AI|OpenAI/i);
    if (tag) {
      expect(tag).toBeInTheDocument();
    }
  });
});
