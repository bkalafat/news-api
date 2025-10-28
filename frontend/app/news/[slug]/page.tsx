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

// Generate static params for top news articles at build time
export async function generateStaticParams() {
  try {
    const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';
    const response = await fetch(`${apiUrl}/api/NewsArticle?pageSize=100`, {
      next: { revalidate: 86400 } // Cache for 24 hours
    });
    
    if (!response.ok) return [];
    
    const news: News[] = await response.json();
    return news.slice(0, 50).map((item) => ({
      slug: item.slug,
    }));
  } catch (error) {
    console.error('Error generating static params:', error);
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
    const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';
    const response = await fetch(`${apiUrl}/api/NewsArticle/by-slug?slug=${encodeURIComponent(slug)}`, {
      next: { revalidate: 60 },
    });

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

  return {
    title: `${news.caption} | Teknoloji Haberleri`,
    description: news.summary,
    keywords: news.keywords.split(",").map((k) => k.trim()),
    openGraph: {
      title: news.caption,
      description: news.summary,
      type: "article",
      publishedTime: news.expressDate,
      modifiedTime: news.updateDate,
      authors: news.authors,
      tags: news.subjects,
      images: [
        {
          url: imageUrl,
          width: 1200,
          height: 630,
          alt: news.imgAlt || news.caption,
        },
      ],
    },
    twitter: {
      card: "summary_large_image",
      title: news.caption,
      description: news.summary,
      images: [imageUrl],
    },
  };
}

export default async function NewsDetailPage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;
  const news = await getNewsBySlug(slug);

  if (!news) {
    notFound();
  }

  const categoryNames: Record<string, string> = {
    technology: "Teknoloji",
    world: "Dünya",
    business: "İş Dünyası",
    science: "Bilim",
    health: "Sağlık",
    entertainment: "Eğlence",
    sports: "Spor",
  };

  return (
    <div className="min-h-screen flex flex-col">
      <Header />
      <main className="flex-1 bg-background">
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
        <div className="max-w-7xl mx-auto grid grid-cols-1 lg:grid-cols-3 gap-8">
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
