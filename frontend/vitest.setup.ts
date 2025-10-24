import '@testing-library/jest-dom/vitest';
import { cleanup } from '@testing-library/react';
import { afterEach, vi } from 'vitest';
import { createElement } from 'react';

// Cleanup after each test
afterEach(() => {
  cleanup();
});

// Mock Next.js router
vi.mock('next/navigation', () => ({
  useRouter: () => ({
    push: vi.fn(),
    replace: vi.fn(),
    prefetch: vi.fn(),
    back: vi.fn(),
    pathname: '/',
    query: {},
    asPath: '/',
  }),
  usePathname: () => '/',
  useSearchParams: () => new URLSearchParams(),
  useParams: () => ({}),
  notFound: vi.fn(),
}));

// Mock Next.js Image - return a simple img element
vi.mock('next/image', () => ({
  __esModule: true,
  default: (props: any) => {
    return createElement('img', props);
  },
}));

// Mock Next.js Link
vi.mock('next/link', () => ({
  __esModule: true,
  default: ({ children, href, ...props }: any) => {
    return createElement('a', { href, ...props }, children);
  },
}));

// Mock next-intl
vi.mock('next-intl', () => ({
  useTranslations: () => (key: string) => key,
  NextIntlClientProvider: ({ children }: any) => children,
}));

// Mock environment variables
process.env.NEXT_PUBLIC_API_URL = 'http://localhost:5000';
process.env.NEXT_PUBLIC_API_BASE_PATH = '/api/NewsArticle';
process.env.NEXT_PUBLIC_SITE_URL = 'http://localhost:3000';
process.env.NEXT_PUBLIC_SITE_NAME = 'Teknoloji Haberleri';

// Mock IntersectionObserver
global.IntersectionObserver = class IntersectionObserver {
  constructor() {}
  disconnect() {}
  observe() {}
  takeRecords() {
    return [];
  }
  unobserve() {}
} as any;

// Mock ResizeObserver
global.ResizeObserver = class ResizeObserver {
  constructor() {}
  disconnect() {}
  observe() {}
  unobserve() {}
} as any;

// Mock matchMedia
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: vi.fn().mockImplementation((query) => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: vi.fn(),
    removeListener: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  })),
});

// Mock fetch globally
global.fetch = vi.fn();
