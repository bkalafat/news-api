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
    // First, try to get all news and find by slug
    const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000'}/api/news`, {
      next: { revalidate: 60 },
    });

    if (!response.ok) {
      return null;
    }

    const allNews: News[] = await response.json();
    const news = allNews.find(n => n.slug === slug);
    
    return news || null;
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
    <div className="min-h-screen bg-background">
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
    </div>
  );
}
