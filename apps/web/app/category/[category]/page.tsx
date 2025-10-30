'use client';

import { use, useState, useMemo } from 'react';
import { useTranslations } from "next-intl";
import { useNewsByCategory } from "@/lib/api/hooks";
import { Header } from "@/components/layout/header";
import { Footer } from "@/components/layout/footer";
import { AnimatedNewsCard } from "@/components/news/animated-news-card";
import { NewsCardSkeleton } from "@/components/news/news-card-skeleton";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { AlertCircle } from "lucide-react";
import { Button } from "@/components/ui/button";
import { DateRangeFilter } from "@/components/filters/date-range-filter";
import { Pagination } from "@/components/pagination/pagination";

interface CategoryPageProps {
  params: Promise<{
    category: string;
  }>;
}

export default function CategoryPage({ params }: CategoryPageProps) {
  const resolvedParams = use(params);
  const t = useTranslations();
  const { data: news, isLoading, error, refetch } = useNewsByCategory(resolvedParams.category);
  const [dateRange, setDateRange] = useState<{ start: Date | null; end: Date | null }>({
    start: null,
    end: null,
  });
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 9;

  const categoryName = t(`categories.${resolvedParams.category}`);

  // Filter news by date range and paginate
  const { filteredNews, paginatedNews, totalPages } = useMemo(() => {
    if (!news) return { filteredNews: [], paginatedNews: [], totalPages: 0 };

    let filtered = news;

    // Apply date filter
    if (dateRange.start || dateRange.end) {
      filtered = news.filter((item) => {
        const publishedDate = item.publishedAt;
        if (!publishedDate) return false;

        if (dateRange.start && publishedDate < dateRange.start) return false;
        if (dateRange.end) {
          const endOfDay = new Date(dateRange.end);
          endOfDay.setHours(23, 59, 59, 999);
          if (publishedDate > endOfDay) return false;
        }

        return true;
      });
    }

    // Paginate
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const paginatedNews = filtered.slice(startIndex, endIndex);
    const totalPages = Math.ceil(filtered.length / itemsPerPage);

    return { filteredNews: filtered, paginatedNews, totalPages };
  }, [news, dateRange, currentPage]);

  const handleDateFilter = (start: Date | null, end: Date | null) => {
    setDateRange({ start, end });
    setCurrentPage(1); // Reset to first page when filtering
  };

  return (
    <div className="min-h-screen flex flex-col">
      <Header />
      <main className="flex-1">
        <div className="container mx-auto px-4 py-12">
          <div className="mb-8">
            <div className="flex items-center justify-between mb-4">
              <div>
                <h1 className="text-4xl font-bold mb-2">{categoryName}</h1>
                <p className="text-muted-foreground">
                  {categoryName} kategorisindeki son haberler
                  {filteredNews.length > 0 && ` (${filteredNews.length} haber)`}
                </p>
              </div>
              <DateRangeFilter onFilter={handleDateFilter} />
            </div>
          </div>

          {isLoading && (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {Array.from({ length: 6 }).map((_, i) => (
                <NewsCardSkeleton key={i} />
              ))}
            </div>
          )}

          {error && (
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
          )}

          {!isLoading && !error && news && news.length === 0 && (
            <Alert>
              <AlertCircle className="h-4 w-4" />
              <AlertTitle>{t('common.noResults')}</AlertTitle>
              <AlertDescription>
                Bu kategoride henüz haber bulunmuyor.
              </AlertDescription>
            </Alert>
          )}

          {!isLoading && !error && paginatedNews.length > 0 && (
            <>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {paginatedNews.map((item, index) => (
                  <AnimatedNewsCard key={item.id} news={item} index={index} />
                ))}
              </div>
              {totalPages > 1 && (
                <div className="flex justify-center mt-8">
                  <Pagination
                    currentPage={currentPage}
                    totalPages={totalPages}
                    onPageChange={setCurrentPage}
                  />
                </div>
              )}
            </>
          )}
        </div>
      </main>
      <Footer />
    </div>
  );
}
