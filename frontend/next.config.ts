import type { NextConfig } from "next";
import createNextIntlPlugin from "next-intl/plugin";

const withNextIntl = createNextIntlPlugin("./i18n/request.ts");

const nextConfig: NextConfig = {
  /* Static export for GitHub Pages */
  output: process.env.NEXT_PUBLIC_BUILD_MODE === "github-pages" ? "export" : undefined,
  basePath: process.env.NEXT_PUBLIC_BASE_PATH || "",
  trailingSlash: true,

  /* Image optimization for external sources */
  images: {
    remotePatterns: [
      {
        protocol: "https",
        hostname: "**",
      },
      {
        protocol: "http",
        hostname: "**",
      },
    ],
    unoptimized: process.env.NEXT_PUBLIC_BUILD_MODE === "github-pages" ? true : false,
    dangerouslyAllowSVG: true,
    contentDispositionType: "attachment",
    contentSecurityPolicy: "default-src 'self'; script-src 'none'; sandbox;",
    loader: "default",
  },

  /* Enable React strict mode for better development experience */
  reactStrictMode: true,

  /* Optimize production builds */
  compiler: {
    removeConsole: process.env.NODE_ENV === "production",
  },

  /* Enable React Compiler for automatic performance optimizations */
  reactCompiler: true,

  /* Performance optimizations */
  experimental: {
    optimizePackageImports: ["lucide-react", "date-fns"],
    // Enable Turbopack filesystem caching for faster dev restarts
    turbopackFileSystemCacheForDev: true,
  },

  /* Aggressive caching for minimum backend load */
  // Cache static pages for 1 week
  // Revalidate every 12 hours to get new content
  staticPageGenerationTimeout: 120,

  /* Headers for CDN and browser caching */
  async headers() {
    return [
      {
        source: "/:path*",
        headers: [
          {
            key: "Cache-Control",
            value: "public, max-age=3600, stale-while-revalidate=86400",
          },
        ],
      },
      {
        source: "/api/:path*",
        headers: [
          {
            key: "Cache-Control",
            value: "public, max-age=300, stale-while-revalidate=600",
          },
        ],
      },
      {
        source: "/_next/image",
        headers: [
          {
            key: "Cache-Control",
            value: "public, max-age=31536000, immutable",
          },
        ],
      },
    ];
  },
};

export default withNextIntl(nextConfig);
