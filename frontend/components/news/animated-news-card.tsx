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
import { motion } from "framer-motion";

interface AnimatedNewsCardProps {
  news: News;
  index?: number;
}

export function AnimatedNewsCard({ news, index = 0 }: AnimatedNewsCardProps) {
  const imageUrl = news.imageUrl || news.thumbnailUrl || news.imgPath || "/placeholder-news.jpg";
  const newsUrl = `/news/${news.slug || news.id}`;

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{
        duration: 0.4,
        delay: index * 0.1,
        ease: "easeOut",
      }}
      whileHover={{ y: -4 }}
    >
      <Card className="group h-full overflow-hidden transition-shadow duration-300 hover:shadow-lg">
        {/* Image */}
        {imageUrl && (
          <div className="bg-muted relative h-48 w-full overflow-hidden">
            <Image
              src={imageUrl}
              alt={news.imgAlt || news.caption}
              fill
              className="object-cover transition-transform duration-300 group-hover:scale-105"
              sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
            />
          </div>
        )}

        <CardHeader className="space-y-2">
          {/* Category Badge & Share Button */}
          <motion.div
            className="flex items-center justify-between"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ delay: 0.2 }}
          >
            {news.category && (
              <Badge variant="secondary" className="w-fit">
                {news.category}
              </Badge>
            )}
            <ShareButtons title={news.caption || news.title || "Haber"} url={newsUrl} />
          </motion.div>

          {/* Title */}
          <CardTitle className="line-clamp-2 leading-tight">
            <Link href={newsUrl} className="hover:text-primary transition-colors">
              {news.caption || news.title}
            </Link>
          </CardTitle>

          {/* Date & Source */}
          <div className="text-muted-foreground flex items-center gap-4 text-xs">
            <div className="flex items-center gap-1">
              <Calendar className="h-3 w-3" />
              {(news.expressDate || news.publishedAt) && (
                <time dateTime={news.expressDate || news.publishedAt?.toISOString()}>
                  {news.expressDate
                    ? format(new Date(news.expressDate), "dd MMMM yyyy", { locale: tr })
                    : news.publishedAt
                      ? format(news.publishedAt, "dd MMMM yyyy", { locale: tr })
                      : "Tarih belirtilmemiş"}
                </time>
              )}
            </div>
            {news.authors && news.authors.length > 0 && (
              <Badge variant="outline" className="text-xs">
                {news.authors[0]}
              </Badge>
            )}
          </div>
        </CardHeader>

        <CardContent>
          {/* Description */}
          <CardDescription className="line-clamp-3">
            {news.summary || news.description || news.content}
          </CardDescription>

          {/* Read More Link */}
          <Link
            href={newsUrl}
            className="text-primary group/link mt-4 inline-flex items-center gap-1 text-sm font-medium hover:underline"
          >
            Haberi Oku
            <ExternalLink className="h-3 w-3 transition-transform group-hover/link:translate-x-0.5" />
          </Link>
        </CardContent>
      </Card>
    </motion.div>
  );
}
