import { notFound } from "next/navigation";
import { NewsDetailHero } from "@/components/news/news-detail-hero";
import { NewsDetailContent } from "@/components/news/news-detail-content";
import { RelatedNews } from "@/components/news/related-news";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import { Metadata } from "next";
import { Header } from "@/components/layout/header";
import { Footer } from "@/components/layout/footer";

// ISR: Revalidate every 6 hours for news detail pages
// Static generation with periodic revalidation
export const revalidate = 21600; // 6 hours

// Generate static params for top 200 news articles at build time (maximize free Azure static hosting)
export async function generateStaticParams() {
  try {
    const apiUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
    const response = await fetch(`${apiUrl}/api/NewsArticle?pageSize=200`, {
      next: { revalidate: 86400 }, // Cache for 24 hours
    });

    if (!response.ok) return [];

    const news: News[] = await response.json();
    // Generate static pages for all 200 articles at build time
    return news.map((item) => ({
      slug: item.slug,
    }));
  } catch (error) {
    console.error("Error generating static params:", error);
    return [];
  }
}

interface News {
  id: string;
  category: string;
  type: string;
  caption: string;
  slug: string;
  keywords: string;
  socialTags: string;
  summary: string;
  imgPath: string;
  imgAlt: string;
  imageUrl?: string;
  thumbnailUrl?: string;
  imageMetadata?: {
    width: number;
    height: number;
    altText: string;
  };
  content: string;
  subjects: string[];
  authors: string[];
  expressDate: string;
  createDate: string;
  updateDate: string;
  priority: number;
  isActive: boolean;
  url: string;
  viewCount: number;
  isSecondPageNews: boolean;
}

async function getNewsBySlug(slug: string): Promise<News | null> {
  try {
    // Use the by-slug endpoint for better performance
    const apiUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
    const response = await fetch(
      `${apiUrl}/api/NewsArticle/by-slug?slug=${encodeURIComponent(slug)}`,
      {
        next: { revalidate: 60 },
      }
    );

    if (!response.ok) {
      console.error(`Failed to fetch news by slug: ${response.status}`);
      return null;
    }

    const news: News = await response.json();
    return news;
  } catch (error) {
    console.error("Error fetching news by slug:", error);
    return null;
  }
}

export async function generateMetadata({
  params,
}: {
  params: Promise<{ slug: string }>;
}): Promise<Metadata> {
  const { slug } = await params;
  const news = await getNewsBySlug(slug);

  if (!news) {
    return {
      title: "Haber Bulunamadı",
    };
  }

  const imageUrl = news.imageUrl || news.imgPath || "/placeholder-news.jpg";
  const siteUrl = process.env.NEXT_PUBLIC_SITE_URL || "http://localhost:3000";
  const canonicalUrl = `${siteUrl}/news/${slug}`;

  return {
    title: `${news.caption}`,
    description: news.summary,
    keywords: news.keywords.split(",").map((k) => k.trim()),
    authors: news.authors.map((author) => ({ name: author })),
    category: news.category,
    alternates: {
      canonical: canonicalUrl,
    },
    openGraph: {
      title: news.caption,
      description: news.summary,
      type: "article",
      url: canonicalUrl,
      siteName: "Teknoloji Haberleri",
      locale: "tr_TR",
      publishedTime: news.expressDate,
      modifiedTime: news.updateDate,
      authors: news.authors,
      tags: news.subjects,
      section: news.category,
      images: [
        {
          url: imageUrl,
          width: news.imageMetadata?.width || 1200,
          height: news.imageMetadata?.height || 630,
          alt: news.imgAlt || news.caption,
        },
      ],
    },
    twitter: {
      card: "summary_large_image",
      site: "@teknoloji_haber",
      creator: "@teknoloji_haber",
      title: news.caption,
      description: news.summary,
      images: [
        {
          url: imageUrl,
          alt: news.imgAlt || news.caption,
        },
      ],
    },
    robots: {
      index: true,
      follow: true,
      nocache: false,
      googleBot: {
        index: true,
        follow: true,
        "max-video-preview": -1,
        "max-image-preview": "large",
        "max-snippet": -1,
      },
    },
  };
}

export default async function NewsDetailPage({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = await params;
  const news = await getNewsBySlug(slug);

  if (!news) {
    notFound();
  }

  const categoryNames: Record<string, string> = {
    popular: "Popüler",
    artificialintelligence: "Yapay Zeka",
    githubcopilot: "GitHub Copilot",
    mcp: "MCP",
    openai: "OpenAI",
    robotics: "Robotik",
    deepseek: "DeepSeek",
    dotnet: ".NET",
    claudeai: "Claude AI",
  };

  const siteUrl = process.env.NEXT_PUBLIC_SITE_URL || "http://localhost:3000";
  const imageUrl = news.imageUrl || news.imgPath || "/placeholder-news.jpg";

  // JSON-LD structured data for NewsArticle
  const articleSchema = {
    "@context": "https://schema.org",
    "@type": "NewsArticle",
    headline: news.caption,
    description: news.summary,
    image: {
      "@type": "ImageObject",
      url: imageUrl,
      width: news.imageMetadata?.width || 1200,
      height: news.imageMetadata?.height || 630,
      caption: news.imgAlt || news.caption,
    },
    datePublished: news.expressDate,
    dateModified: news.updateDate || news.expressDate,
    author: news.authors.map((author) => ({
      "@type": "Person",
      name: author,
    })),
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
    mainEntityOfPage: {
      "@type": "WebPage",
      "@id": `${siteUrl}/news/${slug}`,
    },
    articleSection: categoryNames[news.category] || news.category,
    keywords: news.keywords,
    inLanguage: "tr-TR",
    articleBody: news.content,
  };

  // JSON-LD structured data for BreadcrumbList
  const breadcrumbSchema = {
    "@context": "https://schema.org",
    "@type": "BreadcrumbList",
    itemListElement: [
      {
        "@type": "ListItem",
        position: 1,
        name: "Ana Sayfa",
        item: siteUrl,
      },
      {
        "@type": "ListItem",
        position: 2,
        name: categoryNames[news.category] || news.category,
        item: `${siteUrl}/category/${news.category}`,
      },
      {
        "@type": "ListItem",
        position: 3,
        name: news.caption,
        item: `${siteUrl}/news/${slug}`,
      },
    ],
  };

  return (
    <div className="flex min-h-screen flex-col">
      <script
        type="application/ld+json"
        dangerouslySetInnerHTML={{ __html: JSON.stringify(articleSchema) }}
      />
      <script
        type="application/ld+json"
        dangerouslySetInnerHTML={{ __html: JSON.stringify(breadcrumbSchema) }}
      />
      <Header />
      <main className="bg-background flex-1">
        {/* Breadcrumb Navigation */}
        <div className="container mx-auto px-4 py-4">
          <Breadcrumb>
            <BreadcrumbList>
              <BreadcrumbItem>
                <BreadcrumbLink href="/">Ana Sayfa</BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbLink href={`/category/${news.category}`}>
                  {categoryNames[news.category] || news.category}
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbPage>{news.caption}</BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
        </div>

        {/* Hero Section */}
        <NewsDetailHero news={news} />

        {/* Main Content */}
        <div className="container mx-auto px-4 py-8">
          <div className="mx-auto grid max-w-7xl grid-cols-1 gap-8 lg:grid-cols-3">
            {/* Article Content - 2/3 width on large screens */}
            <div className="lg:col-span-2">
              <NewsDetailContent news={news} />
            </div>

            {/* Sidebar - 1/3 width on large screens */}
            <aside className="lg:col-span-1">
              <RelatedNews category={news.category} currentNewsId={news.id} />
            </aside>
          </div>
        </div>
      </main>
      <Footer />
    </div>
  );
}
