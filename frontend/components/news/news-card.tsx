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
    <Card className="group overflow-hidden transition-shadow duration-300 hover:shadow-lg">
      {/* Image */}
      {news.imageUrl && (
        <div className="bg-muted relative h-48 w-full overflow-hidden">
          <Image
            src={news.imageUrl}
            alt={news.title || "News image"}
            fill
            className="object-cover transition-transform duration-300 group-hover:scale-105"
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
            title={news.title || "News article"}
            url={`${typeof window !== "undefined" ? window.location.origin : ""}${localNewsUrl}`}
          />
        </div>

        {/* Title */}
        <CardTitle className="line-clamp-2 leading-tight">
          <Link href={localNewsUrl} className="hover:text-primary transition-colors">
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
          className="text-primary mt-4 inline-flex items-center gap-1 text-sm font-medium hover:underline"
        >
          Haberi Oku
          <ExternalLink className="h-3 w-3" />
        </Link>
      </CardContent>
    </Card>
  );
}
