"use client";

import { useSearchParams } from "next/navigation";
import { useAllNews } from "@/lib/api/hooks";
import { AnimatedNewsCard } from "@/components/news/animated-news-card";
import { NewsCardSkeleton } from "@/components/news/news-card-skeleton";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { AlertCircle } from "lucide-react";
import { useTranslations } from "next-intl";
import { useMemo } from "react";

export function SearchResults() {
  const searchParams = useSearchParams();
  const query = searchParams.get("q") || "";
  const t = useTranslations("common");

  const { data: news, isLoading, error } = useAllNews();

  // Filter news by query
  const filteredNews = useMemo(() => {
    if (!news || !query) return [];

    const lowerQuery = query.toLowerCase();
    return news.filter(
      (item) =>
        (item.caption || item.title || "").toLowerCase().includes(lowerQuery) ||
        (item.summary || item.description || "").toLowerCase().includes(lowerQuery) ||
        item.category?.toLowerCase().includes(lowerQuery) ||
        item.content?.toLowerCase().includes(lowerQuery) ||
        item.authors?.some((author) => author.toLowerCase().includes(lowerQuery)) ||
        item.author?.toLowerCase().includes(lowerQuery)
    );
  }, [news, query]);

  if (isLoading) {
    return (
      <div className="space-y-8">
        <div className="h-10 w-64 animate-pulse rounded bg-gray-200" />
        <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
          {[...Array(6)].map((_, i) => (
            <NewsCardSkeleton key={i} />
          ))}
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <Alert variant="destructive">
        <AlertCircle className="h-4 w-4" />
        <AlertTitle>{t("error")}</AlertTitle>
        <AlertDescription>
          Haberler yüklenirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.
        </AlertDescription>
      </Alert>
    );
  }

  return (
    <div className="space-y-8">
      <div>
        <h1 className="mb-2 text-3xl font-bold">Arama Sonuçları</h1>
        <p className="text-muted-foreground">
          &quot;{query}&quot; için {filteredNews.length} sonuç bulundu
        </p>
      </div>

      {filteredNews.length === 0 ? (
        <Alert>
          <AlertCircle className="h-4 w-4" />
          <AlertTitle>Sonuç Bulunamadı</AlertTitle>
          <AlertDescription>
            Aramanız için hiçbir haber bulunamadı. Lütfen farklı anahtar kelimeler deneyin.
          </AlertDescription>
        </Alert>
      ) : (
        <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
          {filteredNews.map((item, index) => (
            <AnimatedNewsCard key={item.id} news={item} index={index} />
          ))}
        </div>
      )}
    </div>
  );
}
