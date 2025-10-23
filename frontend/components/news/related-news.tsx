"use client";

import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import Link from "next/link";
import Image from "next/image";
import { Badge } from "@/components/ui/badge";
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
        const filtered = data
          .filter((item) => item.id !== currentNewsId)
          .slice(0, limit);
        
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
    technology: "Teknoloji",
    world: "Dünya",
    business: "Ekonomi",
    science: "Bilim",
    health: "Sağlık",
    entertainment: "Eğlence",
    sports: "Spor",
  };

  return (
    <Card className="sticky top-4">
      <CardHeader>
        <CardTitle>İlgili Haberler</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {news.map((item) => {
          const imageUrl = item.thumbnailUrl || item.imageUrl || item.imgPath || "/placeholder-news.jpg";
          
          return (
            <Link
              key={item.id}
              href={`/news/${item.slug || item.id}`}
              className="group block hover:bg-muted/50 p-2 rounded-lg transition-colors"
            >
              <div className="flex gap-3">
                {/* Thumbnail */}
                <div className="relative w-24 h-20 flex-shrink-0 rounded-md overflow-hidden bg-muted">
                  <Image
                    src={imageUrl}
                    alt={item.caption}
                    fill
                    className="object-cover group-hover:scale-105 transition-transform duration-300"
                    sizes="96px"
                  />
                </div>

                {/* Content */}
                <div className="flex-1 min-w-0">
                  <h4 className="text-sm font-semibold line-clamp-2 group-hover:text-primary transition-colors mb-1">
                    {item.caption}
                  </h4>
                  <div className="flex items-center gap-2 text-xs text-muted-foreground">
                    <Calendar className="w-3 h-3" />
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
          className="block text-center text-sm font-medium text-primary hover:underline pt-2"
        >
          Tüm {categoryNames[category] || category} Haberlerini Gör →
        </Link>
      </CardContent>
    </Card>
  );
}
