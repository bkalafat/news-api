import { Suspense } from 'react';
import { SearchResults } from '@/components/search/search-results';
import { NewsCardSkeleton } from '@/components/news/news-card-skeleton';

export const metadata = {
  title: 'Arama Sonuçları - Türk Haber',
  description: 'Haber arama sonuçları',
};

export default function SearchPage() {
  return (
    <div className="container mx-auto px-4 py-8">
      <Suspense fallback={
        <div className="space-y-8">
          <div className="h-10 w-64 bg-gray-200 animate-pulse rounded" />
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {[...Array(6)].map((_, i) => (
              <NewsCardSkeleton key={i} />
            ))}
          </div>
        </div>
      }>
        <SearchResults />
      </Suspense>
    </div>
  );
}
