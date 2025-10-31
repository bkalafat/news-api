"use client";

import { News } from "@/lib/api/types";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { format } from "date-fns";
import { tr } from "date-fns/locale";
import { Calendar, ExternalLink } from "lucide-react";
import Image from "next/image";
import Link from "next/link";
import { ShareButtons } from "@/components/social/share-buttons";

interface NewsCardProps {
  news: News;
}

export function NewsCard({ news }: NewsCardProps) {
  // Extract slug from URL
  const slug = news.url?.split("/").pop() || "";
  const localNewsUrl = `/news/${slug}`;

  return (
    <Card className="group overflow-hidden transition-all duration-500 hover:shadow-2xl hover:shadow-primary/10 hover:-translate-y-1 border-border/50 bg-card/80 backdrop-blur">
      {/* Image */}
      {news.imageUrl && (
        <div className="bg-muted relative h-48 w-full overflow-hidden">
          <Image
            src={news.imageUrl}
            alt={news.title || "News image"}
            fill
            className="object-cover transition-transform duration-700 group-hover:scale-110"
            sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
          />
          {/* Gradient overlay on hover */}
          <div className="absolute inset-0 bg-gradient-to-t from-background/80 via-background/20 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500" />
        </div>
      )}

      <CardHeader className="space-y-2">
        {/* Category Badge & Share Button */}
        <div className="flex items-center justify-between">
          {news.category && (
            <Badge 
              variant="secondary" 
              className="w-fit bg-primary/10 text-primary hover:bg-primary/20 border-primary/20 font-semibold"
            >
              {news.category}
            </Badge>
          )}
          <ShareButtons
            title={news.title || "News article"}
            url={`${typeof window !== "undefined" ? window.location.origin : ""}${localNewsUrl}`}
          />
        </div>

        {/* Title */}
        <CardTitle className="line-clamp-2 leading-tight text-lg">
          <Link 
            href={localNewsUrl} 
            className="hover:text-primary transition-colors duration-300 bg-gradient-to-r from-foreground to-foreground hover:from-primary hover:to-primary/70 bg-clip-text hover:text-transparent"
          >
            {news.title}
          </Link>
        </CardTitle>

        {/* Date & Source */}
        <div className="text-muted-foreground flex items-center gap-4 text-xs">
          <div className="flex items-center gap-1">
            <Calendar className="h-3 w-3" />
            {news.publishedAt && !isNaN(news.publishedAt.getTime()) ? (
              <time dateTime={news.publishedAt.toISOString()}>
                {format(news.publishedAt, "dd MMMM yyyy", { locale: tr })}
              </time>
            ) : (
              <span>Tarih belirtilmemiş</span>
            )}
          </div>
          {news.source && (
            <Badge variant="outline" className="text-xs">
              {news.source}
            </Badge>
          )}
        </div>
      </CardHeader>

      <CardContent>
        {/* Description */}
        <CardDescription className="line-clamp-3">{news.description}</CardDescription>

        {/* Read More Link */}
        <Link
          href={localNewsUrl}
          className="text-primary mt-4 inline-flex items-center gap-2 text-sm font-semibold hover:gap-3 transition-all duration-300 group/link"
        >
          Haberi Oku
          <ExternalLink className="h-4 w-4 group-hover/link:translate-x-0.5 group-hover/link:-translate-y-0.5 transition-transform" />
        </Link>
      </CardContent>
    </Card>
  );
}
