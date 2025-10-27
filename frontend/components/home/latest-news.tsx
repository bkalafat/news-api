'use client';

import { useState, useMemo } from "react";
import { useTranslations } from "next-intl";
import { useAllNews } from "@/lib/api/hooks";
import { AnimatedNewsCard } from "@/components/news/animated-news-card";
import { NewsCardSkeleton } from "@/components/news/news-card-skeleton";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { AlertCircle } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Pagination } from "@/components/pagination/pagination";

export function LatestNews() {
  const t = useTranslations();
  const { data: news, isLoading, error, refetch } = useAllNews();
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 6;

  // Calculate pagination
  const { paginatedNews, totalPages } = useMemo(() => {
    if (!news) return { paginatedNews: [], totalPages: 0 };
    
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const paginatedNews = news.slice(startIndex, endIndex);
    const totalPages = Math.ceil(news.length / itemsPerPage);
    
    return { paginatedNews, totalPages };
  }, [news, currentPage]);

  if (isLoading) {
    return (
      <section id="latest" className="space-y-6">
        <div className="flex items-center justify-between">
          <h2 className="text-3xl font-bold">{t('home.latest')}</h2>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {Array.from({ length: 6 }).map((_, i) => (
            <NewsCardSkeleton key={i} />
          ))}
        </div>
      </section>
    );
  }

  if (error) {
    return (
      <section id="latest" className="space-y-6">
        <h2 className="text-3xl font-bold">{t('home.latest')}</h2>
        <Alert variant="destructive">
          <AlertCircle className="h-4 w-4" />
          <AlertTitle>{t('common.error')}</AlertTitle>
          <AlertDescription>
            {error.message}
            <Button
              variant="outline"
              size="sm"
              onClick={() => refetch()}
              className="ml-4"
            >
              {t('common.tryAgain')}
            </Button>
          </AlertDescription>
        </Alert>
      </section>
    );
  }

  if (!news || news.length === 0) {
    return (
      <section id="latest" className="space-y-6">
        <h2 className="text-3xl font-bold">{t('home.latest')}</h2>
        <Alert>
          <AlertCircle className="h-4 w-4" />
          <AlertTitle>{t('common.noResults')}</AlertTitle>
          <AlertDescription>
            Şu anda gösterilecek haber bulunmuyor.
          </AlertDescription>
        </Alert>
      </section>
    );
  }

  return (
    <section id="latest" className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-3xl font-bold">{t('home.latest')}</h2>
        <p className="text-muted-foreground">
          Toplam {news.length} haber
        </p>
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {paginatedNews.map((item, index) => (
          <AnimatedNewsCard key={item.id} news={item} index={index} />
        ))}
      </div>
      {totalPages > 1 && (
        <div className="flex justify-center pt-4">
          <Pagination
            currentPage={currentPage}
            totalPages={totalPages}
            onPageChange={setCurrentPage}
          />
        </div>
      )}
    </section>
  );
}
