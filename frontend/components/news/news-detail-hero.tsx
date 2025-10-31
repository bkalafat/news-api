import Image from "next/image";
import { Badge } from "@/components/ui/badge";
import { Calendar, Eye, Clock } from "lucide-react";

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

  const categoryColors: Record<string, { bg: string; from: string; to: string }> = {
    reddit: { bg: "bg-orange-600", from: "from-orange-600", to: "to-orange-500" },
    github: { bg: "bg-gray-900", from: "from-gray-900", to: "to-gray-700" },
    twitter: { bg: "bg-blue-400", from: "from-blue-400", to: "to-blue-500" },
    linkedin: { bg: "bg-blue-700", from: "from-blue-700", to: "to-blue-600" },
    facebook: { bg: "bg-blue-600", from: "from-blue-600", to: "to-blue-500" },
    instagram: { bg: "bg-pink-600", from: "from-pink-600", to: "to-purple-600" },
    tiktok: { bg: "bg-black", from: "from-black", to: "to-gray-800" },
    youtube: { bg: "bg-red-600", from: "from-red-600", to: "to-red-500" },
    technology: { bg: "bg-purple-600", from: "from-purple-600", to: "to-indigo-600" },
  };

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

  const colors = categoryColors[news.category] || { bg: "bg-gray-500", from: "from-gray-500", to: "to-gray-600" };
  const publishDate = new Date(news.expressDate);
  const now = new Date();
  const diffTime = Math.abs(now.getTime() - publishDate.getTime());
  const diffHours = Math.floor(diffTime / (1000 * 60 * 60));
  const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24));
  
  let timeAgo = "";
  if (diffHours < 1) {
    timeAgo = "Az önce";
  } else if (diffHours < 24) {
    timeAgo = `${diffHours} saat önce`;
  } else if (diffDays < 7) {
    timeAgo = `${diffDays} gün önce`;
  } else {
    timeAgo = publishDate.toLocaleDateString("tr-TR", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  }

  return (
    <div className="relative w-full overflow-hidden">
      {/* Animated gradient background */}
      <div className="absolute inset-0 bg-gradient-to-br from-primary/5 via-purple-500/5 to-background" />
      <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-primary/10 via-transparent to-transparent" />
      
      <div className="container relative mx-auto px-4 py-12 md:py-16">
        <div className="max-w-5xl mx-auto">
          {/* Category Badge - Modern with gradient */}
          <div className="mb-6 flex items-center gap-3">
            <Badge 
              className={`bg-gradient-to-r ${colors.from} ${colors.to} text-white px-5 py-2 text-sm font-bold shadow-lg hover:shadow-xl transition-all hover:scale-105 rounded-full border-0`}
            >
              {categoryNames[news.category] || news.category}
            </Badge>
            <div className="h-1 w-16 bg-gradient-to-r from-primary/50 to-transparent rounded-full" />
          </div>

          {/* Title - Premium Typography */}
          <h1 className="text-4xl md:text-6xl lg:text-7xl font-black mb-8 leading-[1.1] tracking-tighter">
            <span className="bg-gradient-to-br from-foreground via-foreground/90 to-foreground/70 bg-clip-text text-transparent">
              {news.caption}
            </span>
          </h1>

          {/* Summary - Enhanced */}
          <p className="text-xl md:text-2xl text-muted-foreground/90 mb-8 leading-relaxed font-light max-w-4xl">
            {news.summary}
          </p>

          {/* Meta Information - Modern Card Design */}
          <div className="flex flex-wrap items-center gap-6 mb-10">
            <div className="flex items-center gap-2 px-4 py-2 bg-background/80 backdrop-blur-sm rounded-full border border-border/50 shadow-sm">
              <Clock className="w-4 h-4 text-primary" />
              <span className="text-sm font-medium">{timeAgo}</span>
            </div>
            <div className="flex items-center gap-2 px-4 py-2 bg-background/80 backdrop-blur-sm rounded-full border border-border/50 shadow-sm">
              <Calendar className="w-4 h-4 text-primary" />
              <time dateTime={news.expressDate} className="text-sm font-medium">
                {publishDate.toLocaleDateString("tr-TR", {
                  year: "numeric",
                  month: "short",
                  day: "numeric",
                })}
              </time>
            </div>
            <div className="flex items-center gap-2 px-4 py-2 bg-background/80 backdrop-blur-sm rounded-full border border-border/50 shadow-sm">
              <Eye className="w-4 h-4 text-primary" />
              <span className="text-sm font-medium">{news.viewCount.toLocaleString("tr-TR")} görüntülenme</span>
            </div>
          </div>

          {/* Main Image - Premium with overlay effects */}
          <div className="relative group">
            {/* Glow effect */}
            <div className="absolute -inset-1 bg-gradient-to-r from-primary/20 via-purple-500/20 to-primary/20 rounded-3xl blur-2xl opacity-0 group-hover:opacity-100 transition-opacity duration-700" />
            
            {/* Image container */}
            <div className="relative w-full aspect-video rounded-2xl overflow-hidden shadow-2xl border border-border/50 bg-muted">
              <Image
                src={imageUrl}
                alt={altText}
                fill
                priority
                className="object-cover transition-transform duration-700 group-hover:scale-105"
                sizes="(max-width: 768px) 100vw, (max-width: 1200px) 80vw, 1200px"
              />
              {/* Subtle overlay on hover */}
              <div className="absolute inset-0 bg-gradient-to-t from-black/20 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500" />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
