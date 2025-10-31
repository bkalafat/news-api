"use client";

import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Twitter, Facebook, Linkedin, Link2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useState } from "react";

interface News {
  id: string;
  content: string;
  authors: string[];
  subjects: string[];
  keywords: string;
  socialTags: string;
}

interface NewsDetailContentProps {
  news: News;
}

export function NewsDetailContent({ news }: NewsDetailContentProps) {
  const [copied, setCopied] = useState(false);

  // Check if content is HTML
  const isHtmlContent = news.content.trim().startsWith("<");

  // Share functionality - removed unused function
  // Uncomment and use when needed:
  // const handleShare = async () => {
  //   if (navigator.share) {
  //     try {
  //       await navigator.share({
  //         title: document.title,
  //         url: window.location.href,
  //       });
  //     } catch (err) {
  //       console.log("Share failed:", err);
  //     }
  //   }
  // };

  const copyLink = () => {
    navigator.clipboard.writeText(window.location.href);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <div className="space-y-8">
      {/* Authors Section - Modern Design */}
      {news.authors && news.authors.length > 0 && (
        <div className="border-primary/20 relative border-l-4 py-2 pl-6">
          <div className="flex flex-wrap items-center gap-6">
            {news.authors.map((author, index) => (
              <div key={index} className="group flex items-center gap-3">
                <div className="relative">
                  <div className="from-primary/20 absolute inset-0 rounded-full bg-gradient-to-br to-purple-500/20 blur-md transition-all group-hover:blur-lg" />
                  <Avatar className="border-background relative border-2 shadow-lg">
                    <AvatarFallback className="from-primary text-primary-foreground bg-gradient-to-br to-purple-600 text-lg font-bold">
                      {author.charAt(0).toUpperCase()}
                    </AvatarFallback>
                  </Avatar>
                </div>
                <div>
                  <p className="text-base font-bold tracking-tight">{author}</p>
                  <p className="text-muted-foreground text-xs tracking-wider uppercase">Yazar</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Article Content - Premium Typography */}
      <div className="relative">
        {/* Decorative gradient line */}
        <div className="from-primary/20 absolute top-0 -left-4 h-full w-1 rounded-full bg-gradient-to-b via-purple-500/10 to-transparent" />

        {isHtmlContent ? (
          <article
            className="prose prose-lg dark:prose-invert /* Base Typography */ prose-headings:scroll-mt-20 prose-headings:font-bold prose-headings:tracking-tight /* Headings with gradients */ prose-h1:text-4xl prose-h1:mb-6 prose-h1:mt-10 prose-h1:bg-gradient-to-r prose-h1:from-foreground prose-h1:to-foreground/70 prose-h1:bg-clip-text prose-h1:text-transparent prose-h2:text-3xl prose-h2:mb-5 prose-h2:mt-12 prose-h2:bg-gradient-to-r prose-h2:from-foreground prose-h2:to-foreground/70 prose-h2:bg-clip-text prose-h2:text-transparent prose-h2:border-b prose-h2:border-border/50 prose-h2:pb-3 prose-h3:text-2xl prose-h3:mb-4 prose-h3:mt-8 prose-h3:text-foreground/90 /* Paragraphs - Optimized Reading */ prose-p:text-lg prose-p:leading-[1.8] prose-p:mb-6 prose-p:text-foreground/90 prose-p:tracking-wide /* Links */ prose-a:text-primary prose-a:no-underline prose-a:font-semibold prose-a:border-b-2 prose-a:border-primary/20 hover:prose-a:border-primary/60 prose-a:transition-colors /* Lists */ prose-ul:my-6 prose-ul:space-y-2 prose-ol:my-6 prose-ol:space-y-2 prose-li:text-lg prose-li:leading-relaxed prose-li:pl-2 prose-li:marker:text-primary/70 /* Images */ prose-img:rounded-2xl prose-img:my-8 prose-img:shadow-2xl prose-img:border prose-img:border-border/50 /* Blockquotes - Tech Blog Style */ prose-blockquote:border-l-4 prose-blockquote:border-primary prose-blockquote:bg-muted/30 prose-blockquote:rounded-r-xl prose-blockquote:pl-6 prose-blockquote:pr-6 prose-blockquote:py-4 prose-blockquote:my-8 prose-blockquote:not-italic prose-blockquote:text-foreground/80 prose-blockquote:font-medium prose-blockquote:shadow-sm /* Code */ prose-code:text-primary prose-code:bg-muted/50 prose-code:px-2 prose-code:py-1 prose-code:rounded-md prose-code:text-sm prose-code:font-mono prose-code:font-semibold prose-code:before:content-[''] prose-code:after:content-[''] /* Pre/Code Blocks */ prose-pre:bg-muted prose-pre:border prose-pre:border-border prose-pre:rounded-xl prose-pre:shadow-lg prose-pre:my-6 /* Tables */ prose-table:w-full prose-table:my-8 prose-table:border-collapse prose-th:bg-muted prose-th:p-4 prose-th:text-left prose-th:font-bold prose-th:border prose-th:border-border prose-td:p-4 prose-td:border prose-td:border-border prose-tr:transition-colors hover:prose-tr:bg-muted/30 /* Strong/Bold */ prose-strong:text-foreground prose-strong:font-bold max-w-none"
            dangerouslySetInnerHTML={{ __html: news.content }}
          />
        ) : (
          <article className="space-y-6">
            {news.content
              .split("\n")
              .filter((p) => p.trim().length > 0)
              .map((paragraph, index) => (
                <p
                  key={index}
                  className="text-foreground/90 text-lg leading-[1.8] tracking-wide first:text-xl first:leading-[1.9] first:font-medium"
                >
                  {paragraph}
                </p>
              ))}
          </article>
        )}
      </div>

      <Separator className="my-12" />

      {/* Tags Section - Modern Card Design */}
      {news.subjects && news.subjects.length > 0 && (
        <div className="group">
          <div className="mb-4 flex items-center gap-2">
            <div className="from-primary h-1 w-8 rounded-full bg-gradient-to-r to-purple-600" />
            <h3 className="text-xl font-bold tracking-tight">İlgili Konular</h3>
          </div>
          <div className="flex flex-wrap gap-2.5">
            {news.subjects.map((subject, index) => (
              <Badge
                key={index}
                variant="secondary"
                className="hover:bg-primary/10 hover:text-primary border-border/50 hover:border-primary/30 cursor-pointer rounded-full border px-4 py-2 text-sm font-semibold transition-all hover:scale-105"
              >
                {subject}
              </Badge>
            ))}
          </div>
        </div>
      )}

      {/* Keywords */}
      {news.keywords && (
        <div className="group">
          <div className="mb-4 flex items-center gap-2">
            <div className="h-1 w-8 rounded-full bg-gradient-to-r from-purple-600 to-blue-600" />
            <h3 className="text-xl font-bold tracking-tight">Etiketler</h3>
          </div>
          <div className="flex flex-wrap gap-2">
            {news.keywords.split(",").map((keyword, index) => (
              <Badge
                key={index}
                variant="outline"
                className="hover:bg-muted cursor-pointer rounded-full px-3 py-1.5 text-xs font-medium transition-all"
              >
                #{keyword.trim()}
              </Badge>
            ))}
          </div>
        </div>
      )}

      {/* Share Section - Enhanced */}
      <div className="from-muted/50 to-muted/20 border-border/50 rounded-2xl border bg-gradient-to-br p-6">
        <div className="flex flex-col items-start justify-between gap-4 sm:flex-row sm:items-center">
          <div>
            <h3 className="mb-1 text-lg font-bold">Bu haberi paylaş</h3>
            <p className="text-muted-foreground text-sm">Arkadaşlarınla ve sosyal medyada paylaş</p>
          </div>
          <div className="flex flex-wrap gap-2">
            <Button
              variant="outline"
              size="sm"
              className="gap-2 transition-all hover:border-[#1DA1F2] hover:bg-[#1DA1F2] hover:text-white"
              onClick={() =>
                window.open(
                  `https://twitter.com/intent/tweet?url=${encodeURIComponent(window.location.href)}`,
                  "_blank"
                )
              }
            >
              <Twitter className="h-4 w-4" />
              Twitter
            </Button>
            <Button
              variant="outline"
              size="sm"
              className="gap-2 transition-all hover:border-[#1877F2] hover:bg-[#1877F2] hover:text-white"
              onClick={() =>
                window.open(
                  `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(window.location.href)}`,
                  "_blank"
                )
              }
            >
              <Facebook className="h-4 w-4" />
              Facebook
            </Button>
            <Button
              variant="outline"
              size="sm"
              className="gap-2 transition-all hover:border-[#0A66C2] hover:bg-[#0A66C2] hover:text-white"
              onClick={() =>
                window.open(
                  `https://www.linkedin.com/sharing/share-offsite/?url=${encodeURIComponent(window.location.href)}`,
                  "_blank"
                )
              }
            >
              <Linkedin className="h-4 w-4" />
              LinkedIn
            </Button>
            <Button
              variant="outline"
              size="sm"
              className="hover:bg-primary hover:text-primary-foreground gap-2 transition-all"
              onClick={copyLink}
            >
              <Link2 className="h-4 w-4" />
              {copied ? "Kopyalandı!" : "Linki Kopyala"}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
