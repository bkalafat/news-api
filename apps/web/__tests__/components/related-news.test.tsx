import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@/__tests__/utils/test-utils';
import { RelatedNews } from '@/components/news/related-news';
import { mockNewsArticles } from '@/__tests__/__mocks__/mockData';

// Mock fetch globally
global.fetch = vi.fn();

describe('RelatedNews Component', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders loading skeleton initially', () => {
    (global.fetch as any).mockResolvedValueOnce({
      ok: true,
      json: async () => mockNewsArticles,
    });

    render(
      <RelatedNews category="technology" currentNewsId="999" limit={3} />
    );

    // Check for loading skeletons
    const skeletons = screen.getAllByRole('generic');
    expect(skeletons.length).toBeGreaterThan(0);
  });

  it('fetches and displays related news', async () => {
    (global.fetch as any).mockResolvedValueOnce({
      ok: true,
      json: async () => mockNewsArticles,
    });

    render(
      <RelatedNews category="technology" currentNewsId="2" limit={3} />
    );

    // Wait for data to load
    await waitFor(() => {
      expect(screen.getByText('İlgili Haberler')).toBeInTheDocument();
    });

    // Should display news excluding the current one
    await waitFor(() => {
      expect(
        screen.getByText(mockNewsArticles[0].caption)
      ).toBeInTheDocument();
    });
  });

  it('filters out current news from results', async () => {
    (global.fetch as any).mockResolvedValueOnce({
      ok: true,
      json: async () => mockNewsArticles,
    });

    const currentNewsId = mockNewsArticles[0].id;
    render(
      <RelatedNews
        category="technology"
        currentNewsId={currentNewsId}
        limit={5}
      />
    );

    await waitFor(() => {
      // Current news should not be displayed
      const currentNewsCaption = mockNewsArticles[0].caption;
      expect(screen.queryByText(currentNewsCaption)).not.toBeInTheDocument();
    });
  });

  it('limits results to specified limit', async () => {
    const manyNews = Array(10)
      .fill(null)
      .map((_, i) => ({
        ...mockNewsArticles[0],
        id: `${i}`,
        caption: `News ${i}`,
      }));

    (global.fetch as any).mockResolvedValueOnce({
      ok: true,
      json: async () => manyNews,
    });

    render(
      <RelatedNews category="technology" currentNewsId="999" limit={3} />
    );

    await waitFor(() => {
      const newsLinks = screen.getAllByRole('link');
      // Should have limit + 1 (for "View All" link)
      expect(newsLinks.length).toBeLessThanOrEqual(4);
    });
  });

  it('calls API with correct category parameter', async () => {
    (global.fetch as any).mockResolvedValueOnce({
      ok: true,
      json: async () => mockNewsArticles,
    });

    const category = 'sports';
    render(
      <RelatedNews category={category} currentNewsId="1" limit={3} />
    );

    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining(`category=${category}`)
      );
    });
  });

  it('renders "View All" link with correct category', async () => {
    (global.fetch as any).mockResolvedValueOnce({
      ok: true,
      json: async () => mockNewsArticles,
    });

    render(
      <RelatedNews category="technology" currentNewsId="1" limit={3} />
    );

    await waitFor(() => {
      const viewAllLink = screen.getByText(/Tüm.*Haberlerini Gör/i);
      expect(viewAllLink).toHaveAttribute('href', '/category/technology');
    });
  });

  it('displays news thumbnails', async () => {
    (global.fetch as any).mockResolvedValueOnce({
      ok: true,
      json: async () => mockNewsArticles,
    });

    render(
      <RelatedNews category="technology" currentNewsId="999" limit={3} />
    );

    await waitFor(() => {
      const images = screen.getAllByRole('img');
      expect(images.length).toBeGreaterThan(0);
    });
  });

  it('displays news dates in Turkish format', async () => {
    (global.fetch as any).mockResolvedValueOnce({
      ok: true,
      json: async () => mockNewsArticles,
    });

    render(
      <RelatedNews category="technology" currentNewsId="999" limit={3} />
    );

    await waitFor(() => {
      // Check for date elements (formatted as "23 Eki" etc.)
      const timeElements = screen.getAllByRole('time');
      expect(timeElements.length).toBeGreaterThan(0);
    });
  });

  it('handles API errors gracefully', async () => {
    (global.fetch as any).mockRejectedValueOnce(
      new Error('API Error')
    );

    render(
      <RelatedNews category="technology" currentNewsId="1" limit={3} />
    );

    // Should not crash and should not display error to user
    await waitFor(() => {
      expect(screen.queryByText('İlgili Haberler')).not.toBeInTheDocument();
    });
  });

  it('returns null when no related news available', async () => {
    (global.fetch as any).mockResolvedValueOnce({
      ok: true,
      json: async () => [],
    });

    const { container } = render(
      <RelatedNews category="technology" currentNewsId="1" limit={3} />
    );

    await waitFor(() => {
      expect(container.firstChild).toBeNull();
    });
  });

  it('renders sticky card for better UX', async () => {
    (global.fetch as any).mockResolvedValueOnce({
      ok: true,
      json: async () => mockNewsArticles,
    });

    const { container } = render(
      <RelatedNews category="technology" currentNewsId="999" limit={3} />
    );

    await waitFor(() => {
      const card = container.querySelector('.sticky');
      expect(card).toBeInTheDocument();
      expect(card).toHaveClass('top-4');
    });
  });
});
