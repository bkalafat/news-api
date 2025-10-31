import { Metadata } from "next";
import Link from "next/link";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Music,
  Cpu,
  TrendingUp,
  ArrowRight,
} from "lucide-react";
import { Header } from "@/components/layout/header";
import { Footer } from "@/components/layout/footer";

export const metadata: Metadata = {
  title: "Kategoriler - Teknoloji Haberleri",
  description:
    "Teknoloji, iş dünyası, eğlence ve daha fazlası. Tüm haber kategorilerine göz atın ve ilgi alanınıza göre haberleri keşfedin.",
  keywords: [
    "teknoloji kategorileri",
    "haber kategorileri",
    "teknoloji haberleri",
    "iş dünyası haberleri",
    "eğlence haberleri",
    "teknoloji",
    "business",
    "entertainment",
  ],
  alternates: {
    canonical: "/categories",
  },
  openGraph: {
    title: "Kategoriler - Teknoloji Haberleri",
    description:
      "Teknoloji, iş dünyası, eğlence ve daha fazlası. Tüm haber kategorilerine göz atın.",
    type: "website",
    url: "/categories",
  },
  twitter: {
    card: "summary",
    title: "Kategoriler - Teknoloji Haberleri",
    description: "Tüm haber kategorilerine göz atın ve ilgi alanınıza göre haberleri keşfedin.",
  },
};

// ISR: Revalidate every 24 hours (static content, no frequent changes)
export const revalidate = 86400; // 24 hours

const categories = [
  {
    id: "technology",
    icon: Cpu,
    color: "text-purple-600 dark:text-purple-400",
    bgColor: "bg-purple-50 dark:bg-purple-950",
    description: "Teknoloji, yazılım, yapay zeka ve yenilikler",
  },
  {
    id: "business",
    icon: TrendingUp,
    color: "text-blue-700 dark:text-blue-400",
    bgColor: "bg-blue-50 dark:bg-blue-950",
    description: "İş dünyası, ekonomi, finans ve girişimcilik",
  },
  {
    id: "entertainment",
    icon: Music,
    color: "text-pink-600 dark:text-pink-400",
    bgColor: "bg-pink-50 dark:bg-pink-950",
    description: "Eğlence, film, müzik ve sosyal medya",
  },
];

export default function CategoriesPage() {
  return (
    <div className="flex min-h-screen flex-col">
      <Header />
      <main className="flex-1">
        <div className="container mx-auto max-w-6xl px-4 py-12">
          {/* Header */}
          <div className="mb-12 text-center">
            <div className="mb-4 flex items-center justify-center gap-2">
              <TrendingUp className="text-primary h-8 w-8" />
              <h1 className="text-4xl font-bold md:text-5xl">Kategoriler</h1>
            </div>
            <p className="text-muted-foreground mx-auto max-w-2xl text-lg">
              Teknoloji, iş dünyası ve eğlence dünyasından güncel haberler
            </p>
          </div>

          {/* Categories Grid */}
          <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
            {categories.map((category) => {
              const Icon = category.icon;
              return (
                <Link key={category.id} href={`/category/${category.id}`} className="group">
                  <Card className="h-full cursor-pointer transition-all duration-300 hover:-translate-y-1 hover:shadow-lg">
                    <CardHeader>
                      <div
                        className={`h-16 w-16 rounded-lg ${category.bgColor} mb-4 flex items-center justify-center transition-transform group-hover:scale-110`}
                      >
                        <Icon className={`h-8 w-8 ${category.color}`} />
                      </div>
                      <div className="flex items-center justify-between">
                        <CardTitle className="capitalize">
                          {category.id === "technology" && "Teknoloji"}
                          {category.id === "business" && "İş Dünyası"}
                          {category.id === "entertainment" && "Eğlence"}
                        </CardTitle>
                        <ArrowRight className="text-muted-foreground h-5 w-5 transition-transform group-hover:translate-x-1" />
                      </div>
                    </CardHeader>
                    <CardContent>
                      <CardDescription>{category.description}</CardDescription>
                    </CardContent>
                  </Card>
                </Link>
              );
            })}
          </div>

          {/* Stats Section */}
          <div className="mt-16 text-center">
            <Card className="mx-auto max-w-2xl">
              <CardHeader>
                <CardTitle>Her Gün Yeni Haberler</CardTitle>
                <CardDescription>
                  Teknoloji, iş dünyası ve eğlence dünyasından sürekli güncellenen içerikler
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="mt-4 grid grid-cols-3 gap-4">
                  <div className="text-center">
                    <div className="text-primary text-3xl font-bold">3</div>
                    <div className="text-muted-foreground text-sm">Kategori</div>
                  </div>
                  <div className="text-center">
                    <div className="text-primary text-3xl font-bold">24/7</div>
                    <div className="text-muted-foreground text-sm">Güncelleme</div>
                  </div>
                  <div className="text-center">
                    <div className="text-primary text-3xl font-bold">∞</div>
                    <div className="text-muted-foreground text-sm">İçerik</div>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      </main>
      <Footer />
    </div>
  );
}
