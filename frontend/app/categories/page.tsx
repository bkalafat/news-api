import { Metadata } from "next";
import Link from "next/link";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import {
  MessageSquare,
  Github,
  Twitter,
  Linkedin,
  Facebook,
  Instagram,
  Music,
  Youtube,
  Cpu,
  TrendingUp,
  ArrowRight,
} from "lucide-react";
import { Header } from "@/components/layout/header";
import { Footer } from "@/components/layout/footer";

export const metadata: Metadata = {
  title: "Kategoriler - Teknoloji Haberleri",
  description:
    "Teknoloji, bilim, iş dünyası, sosyal medya ve daha fazlası. Tüm haber kategorilerine göz atın ve ilgi alanınıza göre haberleri keşfedin.",
  keywords: [
    "teknoloji kategorileri",
    "haber kategorileri",
    "reddit haberleri",
    "github haberleri",
    "twitter haberleri",
    "teknoloji",
    "bilim",
    "iş dünyası",
  ],
  alternates: {
    canonical: "/categories",
  },
  openGraph: {
    title: "Kategoriler - Teknoloji Haberleri",
    description:
      "Teknoloji, bilim, iş dünyası, sosyal medya ve daha fazlası. Tüm haber kategorilerine göz atın.",
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
    id: "reddit",
    icon: MessageSquare,
    color: "text-orange-600 dark:text-orange-400",
    bgColor: "bg-orange-50 dark:bg-orange-950",
    description: "Reddit'ten teknoloji ve toplum haberleri",
  },
  {
    id: "github",
    icon: Github,
    color: "text-gray-900 dark:text-gray-100",
    bgColor: "bg-gray-50 dark:bg-gray-950",
    description: "GitHub'tan geliştirici haberleri ve açık kaynak projeleri",
  },
  {
    id: "twitter",
    icon: Twitter,
    color: "text-blue-400 dark:text-blue-300",
    bgColor: "bg-blue-50 dark:bg-blue-950",
    description: "X/Twitter'dan sosyal medya trendleri ve haberler",
  },
  {
    id: "linkedin",
    icon: Linkedin,
    color: "text-blue-700 dark:text-blue-400",
    bgColor: "bg-blue-50 dark:bg-blue-950",
    description: "LinkedIn'den iş dünyası ve kariyer haberleri",
  },
  {
    id: "facebook",
    icon: Facebook,
    color: "text-blue-600 dark:text-blue-400",
    bgColor: "bg-blue-50 dark:bg-blue-950",
    description: "Facebook'tan sosyal medya haberleri ve gelişmeler",
  },
  {
    id: "instagram",
    icon: Instagram,
    color: "text-pink-600 dark:text-pink-400",
    bgColor: "bg-pink-50 dark:bg-pink-950",
    description: "Instagram'dan görsel içerik ve trendler",
  },
  {
    id: "tiktok",
    icon: Music,
    color: "text-gray-900 dark:text-gray-100",
    bgColor: "bg-gray-50 dark:bg-gray-950",
    description: "TikTok'tan video içerik ve viral trendler",
  },
  {
    id: "youtube",
    icon: Youtube,
    color: "text-red-600 dark:text-red-400",
    bgColor: "bg-red-50 dark:bg-red-950",
    description: "YouTube'dan video platformu haberleri",
  },
  {
    id: "technology",
    icon: Cpu,
    color: "text-purple-600 dark:text-purple-400",
    bgColor: "bg-purple-50 dark:bg-purple-950",
    description: "Genel teknoloji haberleri ve yenilikler",
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
              Sosyal medya platformlarından ve teknoloji kaynaklarından haberler
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
                          {category.id === "reddit" && "Reddit"}
                          {category.id === "github" && "GitHub"}
                          {category.id === "twitter" && "X/Twitter"}
                          {category.id === "linkedin" && "LinkedIn"}
                          {category.id === "facebook" && "Facebook"}
                          {category.id === "instagram" && "Instagram"}
                          {category.id === "tiktok" && "TikTok"}
                          {category.id === "youtube" && "YouTube"}
                          {category.id === "technology" && "Teknoloji"}
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
                  Sosyal medya ve teknoloji kaynaklarından sürekli güncellenen içerikler
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="mt-4 grid grid-cols-3 gap-4">
                  <div className="text-center">
                    <div className="text-primary text-3xl font-bold">9</div>
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
