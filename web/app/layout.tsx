import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import { QueryProvider } from "@/lib/providers/query-provider";
import { ThemeProvider } from "@/lib/providers/theme-provider";
import { NextIntlClientProvider } from 'next-intl';
import { getMessages } from 'next-intl/server';

const inter = Inter({
  subsets: ["latin"],
  display: "swap",
  variable: "--font-inter",
});

export const metadata: Metadata = {
  title: {
    default: "Teknoloji Haberleri - Türkiye'nin Teknoloji Gazetesi",
    template: "%s | Teknoloji Haberleri"
  },
  description: "BBC ve güvenilir kaynaklardan son dakika teknoloji haberleri, güncel gelişmeler ve analizler.",
  keywords: ["teknoloji", "haberler", "Türkiye", "BBC", "teknoloji haberleri", "güncel haberler"],
  authors: [{ name: "Teknoloji Haberleri" }],
  creator: "Teknoloji Haberleri",
  publisher: "Teknoloji Haberleri",
  formatDetection: {
    email: false,
    address: false,
    telephone: false,
  },
  metadataBase: new URL(process.env.NEXT_PUBLIC_SITE_URL || 'http://localhost:3000'),
  openGraph: {
    type: "website",
    locale: "tr_TR",
    url: "/",
    siteName: "Teknoloji Haberleri",
    title: "Teknoloji Haberleri - Türkiye'nin Teknoloji Gazetesi",
    description: "BBC ve güvenilir kaynaklardan son dakika teknoloji haberleri",
    images: [
      {
        url: "/og-image.png",
        width: 1200,
        height: 630,
        alt: "Teknoloji Haberleri",
      },
    ],
  },
  twitter: {
    card: "summary_large_image",
    title: "Teknoloji Haberleri",
    description: "BBC ve güvenilir kaynaklardan son dakika teknoloji haberleri",
    images: ["/og-image.png"],
  },
  robots: {
    index: true,
    follow: true,
    googleBot: {
      index: true,
      follow: true,
      'max-video-preview': -1,
      'max-image-preview': 'large',
      'max-snippet': -1,
    },
  },
};

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const messages = await getMessages();

  return (
    <html lang="tr" className={inter.variable} suppressHydrationWarning>
      <body className="antialiased min-h-screen bg-background">
        <ThemeProvider
          attribute="class"
          defaultTheme="system"
          enableSystem
          disableTransitionOnChange
        >
          <QueryProvider>
            <NextIntlClientProvider messages={messages}>
              {children}
            </NextIntlClientProvider>
          </QueryProvider>
        </ThemeProvider>
      </body>
    </html>
  );
}
