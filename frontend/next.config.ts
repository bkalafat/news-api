import type { NextConfig } from "next";
import createNextIntlPlugin from 'next-intl/plugin';

const withNextIntl = createNextIntlPlugin('./i18n/request.ts');

const nextConfig: NextConfig = {
  /* Image optimization for external sources */
  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: '**',
      },
      {
        protocol: 'http',
        hostname: '**',
      },
    ],
    unoptimized: false,
    dangerouslyAllowSVG: true,
    contentDispositionType: 'attachment',
    contentSecurityPolicy: "default-src 'self'; script-src 'none'; sandbox;",
    loader: 'default',
  },
  
  /* Enable React strict mode for better development experience */
  reactStrictMode: true,

  /* Optimize production builds */
  compiler: {
    removeConsole: process.env.NODE_ENV === 'production',
  },

  /* Enable React Compiler for automatic performance optimizations */
  reactCompiler: true,

  /* Performance optimizations */
  experimental: {
    optimizePackageImports: ['lucide-react', 'date-fns'],
    // Enable Turbopack filesystem caching for faster dev restarts
    turbopackFileSystemCacheForDev: true,
  },
};

export default withNextIntl(nextConfig);
