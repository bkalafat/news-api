import { Header } from "@/components/layout/header";
import { Footer } from "@/components/layout/footer";
import { HeroSection } from "@/components/home/hero-section";
import { LatestNews } from "@/components/home/latest-news";
import { CategoriesSection } from "@/components/home/categories-section";

// ISR: Revalidate every 12 hours (43200 seconds)
// Reduces backend API calls significantly
export const revalidate = 43200; // 12 hours

export default function HomePage() {
  return (
    <div className="min-h-screen flex flex-col">
      <Header />
      <main className="flex-1">
        <HeroSection />
        <div className="container mx-auto px-4 py-12 space-y-16">
          <LatestNews />
          <CategoriesSection />
        </div>
      </main>
      <Footer />
    </div>
  );
}
