"use client";

import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import Link from "next/link";
import Image from "next/image";
import { Calendar } from "lucide-react";

interface RelatedNewsItem {
  id: string;
  caption: string;
  slug: string;
  summary: string;
  category: string;
  thumbnailUrl?: string;
  imageUrl?: string;
  imgPath: string;
  expressDate: string;
}

interface RelatedNewsProps {
  category: string;
  currentNewsId: string;
  limit?: number;
}

export function RelatedNews({ category, currentNewsId, limit = 5 }: RelatedNewsProps) {
  const [news, setNews] = useState<RelatedNewsItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    async function fetchRelatedNews() {
      try {
        const apiUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
        const response = await fetch(`${apiUrl}/api/NewsArticle?category=${category}`);

        if (!response.ok) {
          throw new Error("Failed to fetch related news");
        }

        const data: RelatedNewsItem[] = await response.json();

        // Filter out current news and limit results
        const filtered = data.filter((item) => item.id !== currentNewsId).slice(0, limit);

        setNews(filtered);
      } catch (error) {
        console.error("Error fetching related news:", error);
      } finally {
        setIsLoading(false);
      }
    }

    fetchRelatedNews();
  }, [category, currentNewsId, limit]);

  if (isLoading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>İlgili Haberler</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          {Array.from({ length: 3 }).map((_, index) => (
            <div key={index} className="space-y-2">
              <Skeleton className="h-24 w-full rounded-md" />
              <Skeleton className="h-4 w-3/4" />
              <Skeleton className="h-4 w-1/2" />
            </div>
          ))}
        </CardContent>
      </Card>
    );
  }

  if (news.length === 0) {
    return null;
  }

  const categoryNames: Record<string, string> = {
    reddit: "Reddit",
    github: "GitHub",
    twitter: "X/Twitter",
    linkedin: "LinkedIn",
    facebook: "Facebook",
    instagram: "Instagram",
    tiktok: "TikTok",
    youtube: "YouTube",
    technology: "Teknoloji",
  };

  return (
    <Card className="sticky top-4">
      <CardHeader>
        <CardTitle>İlgili Haberler</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {news.map((item) => {
          const imageUrl =
            item.thumbnailUrl || item.imageUrl || item.imgPath || "/placeholder-news.jpg";

          return (
            <Link
              key={item.id}
              href={`/news/${item.slug || item.id}`}
              className="group hover:bg-muted/50 block rounded-lg p-2 transition-colors"
            >
              <div className="flex gap-3">
                {/* Thumbnail */}
                <div className="bg-muted relative h-20 w-24 flex-shrink-0 overflow-hidden rounded-md">
                  <Image
                    src={imageUrl}
                    alt={item.caption}
                    fill
                    className="object-cover transition-transform duration-300 group-hover:scale-105"
                    sizes="96px"
                  />
                </div>

                {/* Content */}
                <div className="min-w-0 flex-1">
                  <h4 className="group-hover:text-primary mb-1 line-clamp-2 text-sm font-semibold transition-colors">
                    {item.caption}
                  </h4>
                  <div className="text-muted-foreground flex items-center gap-2 text-xs">
                    <Calendar className="h-3 w-3" />
                    <time dateTime={item.expressDate}>
                      {new Date(item.expressDate).toLocaleDateString("tr-TR", {
                        day: "numeric",
                        month: "short",
                      })}
                    </time>
                  </div>
                </div>
              </div>
            </Link>
          );
        })}

        {/* View All Link */}
        <Link
          href={`/category/${category}`}
          className="text-primary block pt-2 text-center text-sm font-medium hover:underline"
        >
          Tüm {categoryNames[category] || category} Haberlerini Gör →
        </Link>
      </CardContent>
    </Card>
  );
}
