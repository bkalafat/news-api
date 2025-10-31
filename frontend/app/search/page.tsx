import { Suspense } from "react";
import { SearchResults } from "@/components/search/search-results";
import { NewsCardSkeleton } from "@/components/news/news-card-skeleton";
import { Header } from "@/components/layout/header";
import { Footer } from "@/components/layout/footer";

export const metadata = {
  title: "Arama Sonuçları - Türk Haber",
  description: "Haber arama sonuçları",
};

export default function SearchPage() {
  return (
    <div className="flex min-h-screen flex-col">
      <Header />
      <main className="flex-1">
        <div className="container mx-auto px-4 py-8">
          <Suspense
            fallback={
              <div className="space-y-8">
                <div className="h-10 w-64 animate-pulse rounded bg-gray-200" />
                <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
                  {[...Array(6)].map((_, i) => (
                    <NewsCardSkeleton key={i} />
                  ))}
                </div>
              </div>
            }
          >
            <SearchResults />
          </Suspense>
        </div>
      </main>
      <Footer />
    </div>
  );
}
