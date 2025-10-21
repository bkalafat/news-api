'use client';

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
  const slug = news.url?.split('/').pop() || '';
  const localNewsUrl = `/news/${slug}`;
  
  return (
    <Card className="overflow-hidden hover:shadow-lg transition-shadow duration-300 group">
      {/* Image */}
      {news.imageUrl && (
        <div className="relative h-48 w-full overflow-hidden bg-muted">
          <Image
            src={news.imageUrl}
            alt={news.title}
            fill
            className="object-cover group-hover:scale-105 transition-transform duration-300"
            sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
          />
        </div>
      )}

      <CardHeader className="space-y-2">
        {/* Category Badge & Share Button */}
        <div className="flex items-center justify-between">
          {news.category && (
            <Badge variant="secondary" className="w-fit">
              {news.category}
            </Badge>
          )}
          <ShareButtons 
            title={news.title} 
            url={`${typeof window !== 'undefined' ? window.location.origin : ''}${localNewsUrl}`}
          />
        </div>

        {/* Title */}
        <CardTitle className="line-clamp-2 leading-tight">
          <Link
            href={localNewsUrl}
            className="hover:text-primary transition-colors"
          >
            {news.title}
          </Link>
        </CardTitle>

        {/* Date & Source */}
        <div className="flex items-center gap-4 text-xs text-muted-foreground">
          <div className="flex items-center gap-1">
            <Calendar className="h-3 w-3" />
            {news.publishedAt && !isNaN(news.publishedAt.getTime()) ? (
              <time dateTime={news.publishedAt.toISOString()}>
                {format(news.publishedAt, 'dd MMMM yyyy', { locale: tr })}
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
        <CardDescription className="line-clamp-3">
          {news.description}
        </CardDescription>

        {/* Read More Link */}
        <Link
          href={localNewsUrl}
          className="inline-flex items-center gap-1 mt-4 text-sm font-medium text-primary hover:underline"
        >
          Haberi Oku
          <ExternalLink className="h-3 w-3" />
        </Link>
      </CardContent>
    </Card>
  );
}
