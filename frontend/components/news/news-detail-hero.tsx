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

  const colors = categoryColors[news.category] || {
    bg: "bg-gray-500",
    from: "from-gray-500",
    to: "to-gray-600",
  };
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
      <div className="from-primary/5 to-background absolute inset-0 bg-gradient-to-br via-purple-500/5" />
      <div className="from-primary/10 absolute inset-0 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] via-transparent to-transparent" />

      <div className="relative container mx-auto px-4 py-12 md:py-16">
        <div className="mx-auto max-w-5xl">
          {/* Category Badge - Modern with gradient */}
          <div className="mb-6 flex items-center gap-3">
            <Badge
              className={`bg-gradient-to-r ${colors.from} ${colors.to} rounded-full border-0 px-5 py-2 text-sm font-bold text-white shadow-lg transition-all hover:scale-105 hover:shadow-xl`}
            >
              {categoryNames[news.category] || news.category}
            </Badge>
            <div className="from-primary/50 h-1 w-16 rounded-full bg-gradient-to-r to-transparent" />
          </div>

          {/* Title - Premium Typography */}
          <h1 className="mb-8 text-4xl leading-[1.1] font-black tracking-tighter md:text-6xl lg:text-7xl">
            <span className="from-foreground via-foreground/90 to-foreground/70 bg-gradient-to-br bg-clip-text text-transparent">
              {news.caption}
            </span>
          </h1>

          {/* Summary - Enhanced */}
          <p className="text-muted-foreground/90 mb-8 max-w-4xl text-xl leading-relaxed font-light md:text-2xl">
            {news.summary}
          </p>

          {/* Meta Information - Modern Card Design */}
          <div className="mb-10 flex flex-wrap items-center gap-6">
            <div className="bg-background/80 border-border/50 flex items-center gap-2 rounded-full border px-4 py-2 shadow-sm backdrop-blur-sm">
              <Clock className="text-primary h-4 w-4" />
              <span className="text-sm font-medium">{timeAgo}</span>
            </div>
            <div className="bg-background/80 border-border/50 flex items-center gap-2 rounded-full border px-4 py-2 shadow-sm backdrop-blur-sm">
              <Calendar className="text-primary h-4 w-4" />
              <time dateTime={news.expressDate} className="text-sm font-medium">
                {publishDate.toLocaleDateString("tr-TR", {
                  year: "numeric",
                  month: "short",
                  day: "numeric",
                })}
              </time>
            </div>
            <div className="bg-background/80 border-border/50 flex items-center gap-2 rounded-full border px-4 py-2 shadow-sm backdrop-blur-sm">
              <Eye className="text-primary h-4 w-4" />
              <span className="text-sm font-medium">
                {news.viewCount.toLocaleString("tr-TR")} görüntülenme
              </span>
            </div>
          </div>

          {/* Main Image - Premium with overlay effects */}
          <div className="group relative">
            {/* Glow effect */}
            <div className="from-primary/20 to-primary/20 absolute -inset-1 rounded-3xl bg-gradient-to-r via-purple-500/20 opacity-0 blur-2xl transition-opacity duration-700 group-hover:opacity-100" />

            {/* Image container */}
            <div className="border-border/50 bg-muted relative aspect-video w-full overflow-hidden rounded-2xl border shadow-2xl">
              <Image
                src={imageUrl}
                alt={altText}
                fill
                priority
                className="object-cover transition-transform duration-700 group-hover:scale-105"
                sizes="(max-width: 768px) 100vw, (max-width: 1200px) 80vw, 1200px"
              />
              {/* Subtle overlay on hover */}
              <div className="absolute inset-0 bg-gradient-to-t from-black/20 to-transparent opacity-0 transition-opacity duration-500 group-hover:opacity-100" />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
