import { Header } from "@/components/layout/header";
import { Footer } from "@/components/layout/footer";
import { HeroSection } from "@/components/home/hero-section";
import { LatestNews } from "@/components/home/latest-news";
import { CategoriesSection } from "@/components/home/categories-section";
import { Metadata } from "next";

// ISR: Revalidate every 30 minutes for fresh Reddit news
export const revalidate = 1800; // 30 minutes

export const metadata: Metadata = {
  alternates: {
    canonical: "/",
  },
};

export default function HomePage() {
  return (
    <div className="flex min-h-screen flex-col">
      <Header />
      <main className="flex-1">
        <HeroSection />
        <div className="container mx-auto space-y-16 px-4 py-12">
          <LatestNews />
          <CategoriesSection />
        </div>
      </main>
      <Footer />
    </div>
  );
}
