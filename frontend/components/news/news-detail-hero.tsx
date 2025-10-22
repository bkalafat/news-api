import Image from "next/image";
import { Badge } from "@/components/ui/badge";
import { Calendar, Eye } from "lucide-react";

interface News {
  id: string;
  category: string;
  caption: string;
  summary: string;
  imgAlt: string;
  imageUrl?: string;
  thumbnailUrl?: string;
  imgPath: string;
  expressDate: string;
  viewCount: number;
  imageMetadata?: {
    width: number;
    height: number;
    altText: string;
  };
}

interface NewsDetailHeroProps {
  news: News;
}

export function NewsDetailHero({ news }: NewsDetailHeroProps) {
  const imageUrl = news.imageUrl || news.imgPath || "/placeholder-news.jpg";
  const altText = news.imageMetadata?.altText || news.imgAlt || news.caption;

  const categoryColors: Record<string, string> = {
    technology: "bg-blue-500",
    world: "bg-green-500",
    business: "bg-purple-500",
    science: "bg-cyan-500",
    health: "bg-red-500",
    entertainment: "bg-pink-500",
    sports: "bg-orange-500",
  };

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
    <div className="relative w-full bg-gradient-to-b from-background to-muted/20">
      {/* Hero Image */}
      <div className="container mx-auto px-4 py-8">
        <div className="max-w-5xl mx-auto">
          {/* Category Badge */}
          <div className="mb-4">
            <Badge 
              className={`${categoryColors[news.category] || "bg-gray-500"} text-white px-4 py-1 text-sm font-semibold`}
            >
              {categoryNames[news.category] || news.category}
            </Badge>
          </div>

          {/* Title */}
          <h1 className="text-4xl md:text-5xl font-bold mb-6 leading-tight text-foreground">
            {news.caption}
          </h1>

          {/* Summary */}
          <p className="text-xl text-muted-foreground mb-6 leading-relaxed">
            {news.summary}
          </p>

          {/* Meta Information */}
          <div className="flex flex-wrap items-center gap-4 text-sm text-muted-foreground mb-8">
            <div className="flex items-center gap-2">
              <Calendar className="w-4 h-4" />
              <time dateTime={news.expressDate}>
                {new Date(news.expressDate).toLocaleDateString("tr-TR", {
                  year: "numeric",
                  month: "long",
                  day: "numeric",
                  hour: "2-digit",
                  minute: "2-digit",
                })}
              </time>
            </div>
            <div className="flex items-center gap-2">
              <Eye className="w-4 h-4" />
              <span>{news.viewCount.toLocaleString("tr-TR")} görüntülenme</span>
            </div>
          </div>

          {/* Main Image */}
          <div className="relative w-full aspect-video rounded-lg overflow-hidden shadow-2xl border border-border">
            <Image
              src={imageUrl}
              alt={altText}
              fill
              priority
              className="object-cover"
              sizes="(max-width: 768px) 100vw, (max-width: 1200px) 80vw, 1200px"
            />
          </div>
        </div>
      </div>
    </div>
  );
}
