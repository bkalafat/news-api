import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import { QueryProvider } from "@/lib/providers/query-provider";
import { ThemeProvider } from "@/lib/providers/theme-provider";
import { NextIntlClientProvider } from "next-intl";
import { getMessages } from "next-intl/server";

const inter = Inter({
  subsets: ["latin"],
  display: "swap",
  variable: "--font-inter",
});

export const metadata: Metadata = {
  title: {
    default: "Teknoloji Haberleri - Türkiye'nin Teknoloji Gazetesi",
    template: "%s | Teknoloji Haberleri",
  },
  description:
    "Son dakika teknoloji haberleri, güncel gelişmeler, yazılım, donanım, yapay zeka, siber güvenlik ve teknoloji dünyasından tüm haberler. Türkiye'nin öncü teknoloji haber platformu.",
  generator: "Next.js",
  keywords: [
    "teknoloji haberleri",
    "güncel haberler",
    "yazılım haberleri",
    "teknoloji",
    "haberler",
    "Türkiye",
    "yapay zeka",
    "siber güvenlik",
    "donanım",
    "mobil",
    "bilim",
    "inovasyon",
  ],
  authors: [{ name: "Teknoloji Haberleri Editörleri", url: "https://teknoloji-haberleri.com" }],
  creator: "Teknoloji Haberleri",
  publisher: "Teknoloji Haberleri",
  category: "technology",
  classification: "Technology News",
  formatDetection: {
    email: false,
    address: false,
    telephone: false,
  },
  metadataBase: new URL(process.env.NEXT_PUBLIC_SITE_URL || "http://localhost:3000"),
  alternates: {
    canonical: "/",
    languages: {
      "tr-TR": "/",
    },
  },
  openGraph: {
    type: "website",
    locale: "tr_TR",
    url: "/",
    siteName: "Teknoloji Haberleri",
    title: "Teknoloji Haberleri - Türkiye'nin Teknoloji Gazetesi",
    description:
      "Son dakika teknoloji haberleri, güncel gelişmeler, yazılım, donanım, yapay zeka ve teknoloji dünyasından tüm haberler.",
    images: [
      {
        url: "/og-image.png",
        width: 1200,
        height: 630,
        alt: "Teknoloji Haberleri - Türkiye'nin Teknoloji Gazetesi",
      },
    ],
  },
  twitter: {
    card: "summary_large_image",
    site: "@teknoloji_haber",
    creator: "@teknoloji_haber",
    title: "Teknoloji Haberleri - Türkiye'nin Teknoloji Gazetesi",
    description:
      "Son dakika teknoloji haberleri, güncel gelişmeler, yazılım, donanım, yapay zeka ve teknoloji dünyasından tüm haberler.",
    images: ["/og-image.png"],
  },
  robots: {
    index: true,
    follow: true,
    nocache: false,
    googleBot: {
      index: true,
      follow: true,
      noimageindex: false,
      "max-video-preview": -1,
      "max-image-preview": "large",
      "max-snippet": -1,
    },
  },
  verification: {
    google: "google-site-verification-code",
    yandex: "yandex-verification-code",
    other: {
      "msvalidate.01": "bing-verification-code",
    },
  },
};

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const messages = await getMessages();
  const siteUrl = process.env.NEXT_PUBLIC_SITE_URL || "http://localhost:3000";

  // JSON-LD structured data for the website
  const websiteSchema = {
    "@context": "https://schema.org",
    "@type": "WebSite",
    name: "Teknoloji Haberleri",
    alternateName: "Türkiye'nin Teknoloji Gazetesi",
    url: siteUrl,
    description:
      "Son dakika teknoloji haberleri, güncel gelişmeler, yazılım, donanım, yapay zeka ve teknoloji dünyasından tüm haberler.",
    inLanguage: "tr-TR",
    potentialAction: {
      "@type": "SearchAction",
      target: {
        "@type": "EntryPoint",
        urlTemplate: `${siteUrl}/search?q={search_term_string}`,
      },
      "query-input": "required name=search_term_string",
    },
    publisher: {
      "@type": "Organization",
      name: "Teknoloji Haberleri",
      url: siteUrl,
      logo: {
        "@type": "ImageObject",
        url: `${siteUrl}/og-image.png`,
        width: 1200,
        height: 630,
      },
    },
  };

  const organizationSchema = {
    "@context": "https://schema.org",
    "@type": "Organization",
    name: "Teknoloji Haberleri",
    url: siteUrl,
    logo: `${siteUrl}/og-image.png`,
    description:
      "Türkiye'nin öncü teknoloji haber platformu. Yazılım, donanım, yapay zeka, siber güvenlik ve teknoloji dünyasından güncel haberler.",
    sameAs: [
      "https://twitter.com/teknoloji_haber",
      "https://www.facebook.com/teknolojihaber",
      "https://www.linkedin.com/company/teknoloji-haberleri",
    ],
    contactPoint: {
      "@type": "ContactPoint",
      contactType: "Customer Service",
      availableLanguage: ["Turkish"],
    },
  };

  return (
    <html lang="tr" className={inter.variable} suppressHydrationWarning>
      <head>
        <script
          type="application/ld+json"
          dangerouslySetInnerHTML={{ __html: JSON.stringify(websiteSchema) }}
        />
        <script
          type="application/ld+json"
          dangerouslySetInnerHTML={{ __html: JSON.stringify(organizationSchema) }}
        />
      </head>
      <body className="bg-background min-h-screen antialiased">
        <ThemeProvider
          attribute="class"
          defaultTheme="system"
          enableSystem
          disableTransitionOnChange
        >
          <QueryProvider>
            <NextIntlClientProvider messages={messages}>{children}</NextIntlClientProvider>
          </QueryProvider>
        </ThemeProvider>
      </body>
    </html>
  );
}
