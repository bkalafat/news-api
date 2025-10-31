import { Metadata } from "next";
import Link from "next/link";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Cpu,
  TrendingUp,
  ArrowRight,
  MessageSquare,
} from "lucide-react";
import { Header } from "@/components/layout/header";
import { Footer } from "@/components/layout/footer";

export const metadata: Metadata = {
  title: "Kategoriler - Teknoloji Haberleri",
  description:
    "GitHub, Reddit ve genel teknoloji haberlerini kategorilere göre keşfedin.",
  keywords: [
    "github",
    "reddit",
    "teknoloji",
    "yazılım geliştirme",
    "teknoloji haberleri",
    "github haberleri",
    "reddit haberleri",
  ],
  alternates: {
    canonical: "/categories",
  },
  openGraph: {
    title: "Kategoriler - Teknoloji Haberleri",
    description:
      "GitHub, Reddit ve teknoloji haberlerini kategorilere göre keşfedin.",
    type: "website",
    url: "/categories",
  },
  twitter: {
    card: "summary",
    title: "Kategoriler - Teknoloji Haberleri",
    description: "GitHub, Reddit ve teknoloji haberlerini keşfedin.",
  },
};

// Force revalidation on every request to show updated Reddit categories
export const revalidate = 0; // Disable ISR cache

const categories = [
  {
    id: "github",
    icon: Cpu,
    color: "text-purple-600 dark:text-purple-400",
    bgColor: "bg-purple-50 dark:bg-purple-950",
    description: "GitHub haberleri ve güncellemeler",
    displayName: "GitHub",
  },
  {
    id: "reddit",
    icon: MessageSquare,
    color: "text-orange-600 dark:text-orange-400",
    bgColor: "bg-orange-50 dark:bg-orange-950",
    description: "Reddit'ten teknoloji haberleri",
    displayName: "Reddit",
  },
  {
    id: "technology",
    icon: TrendingUp,
    color: "text-blue-600 dark:text-blue-400",
    bgColor: "bg-blue-50 dark:bg-blue-950",
    description: "Genel teknoloji haberleri",
    displayName: "Teknoloji",
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
              GitHub, Reddit ve teknoloji haberlerini kategorilere göre keşfedin
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
                          {category.displayName}
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
                  GitHub, Reddit ve teknoloji dünyasından güncel haberler
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="mt-4 grid grid-cols-3 gap-4">
                  <div className="text-center">
                    <div className="text-primary text-3xl font-bold">3</div>
                    <div className="text-muted-foreground text-sm">Teknoloji Kategorisi</div>
                  </div>
                  <div className="text-center">
                    <div className="text-primary text-3xl font-bold">Günlük</div>
                    <div className="text-muted-foreground text-sm">Otomatik Güncelleme</div>
                  </div>
                  <div className="text-center">
                    <div className="text-primary text-3xl font-bold">Güncel</div>
                    <div className="text-muted-foreground text-sm">Son Haberler</div>
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
