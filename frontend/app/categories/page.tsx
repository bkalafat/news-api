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
    "Yapay zeka, robotik, yazılım geliştirme ve daha fazlası. Reddit'ten teknoloji haberlerini kategorilere göre keşfedin.",
  keywords: [
    "yapay zeka",
    "AI",
    "OpenAI",
    "Claude AI",
    "GitHub Copilot",
    "robotik",
    ".NET",
    "DeepSeek",
    "MCP",
    "teknoloji haberleri",
    "reddit teknoloji",
  ],
  alternates: {
    canonical: "/categories",
  },
  openGraph: {
    title: "Kategoriler - Teknoloji Haberleri",
    description:
      "Yapay zeka, robotik ve yazılım geliştirme. Reddit'ten teknoloji haberlerini keşfedin.",
    type: "website",
    url: "/categories",
  },
  twitter: {
    card: "summary",
    title: "Kategoriler - Teknoloji Haberleri",
    description: "Reddit'ten yapay zeka ve teknoloji haberlerini kategorilere göre keşfedin.",
  },
};

// Force revalidation on every request to show updated Reddit categories
export const revalidate = 0; // Disable ISR cache

const categories = [
  {
    id: "popular",
    icon: TrendingUp,
    color: "text-orange-600 dark:text-orange-400",
    bgColor: "bg-orange-50 dark:bg-orange-950",
    description: "Reddit'ten en popüler haberler",
    subreddit: "r/popular",
  },
  {
    id: "artificialintelligence",
    icon: Cpu,
    color: "text-purple-600 dark:text-purple-400",
    bgColor: "bg-purple-50 dark:bg-purple-950",
    description: "Yapay zeka haberleri ve gelişmeleri",
    subreddit: "r/ArtificialIntelligence",
  },
  {
    id: "githubcopilot",
    icon: Cpu,
    color: "text-blue-600 dark:text-blue-400",
    bgColor: "bg-blue-50 dark:bg-blue-950",
    description: "GitHub Copilot haberleri",
    subreddit: "r/GithubCopilot",
  },
  {
    id: "mcp",
    icon: Cpu,
    color: "text-green-600 dark:text-green-400",
    bgColor: "bg-green-50 dark:bg-green-950",
    description: "MCP (Model Context Protocol) haberleri",
    subreddit: "r/mcp",
  },
  {
    id: "openai",
    icon: Cpu,
    color: "text-teal-600 dark:text-teal-400",
    bgColor: "bg-teal-50 dark:bg-teal-950",
    description: "OpenAI ve GPT haberleri",
    subreddit: "r/OpenAI",
  },
  {
    id: "robotics",
    icon: Cpu,
    color: "text-red-600 dark:text-red-400",
    bgColor: "bg-red-50 dark:bg-red-950",
    description: "Robotik ve otomasyon",
    subreddit: "r/robotics",
  },
  {
    id: "deepseek",
    icon: Cpu,
    color: "text-indigo-600 dark:text-indigo-400",
    bgColor: "bg-indigo-50 dark:bg-indigo-950",
    description: "DeepSeek AI haberleri",
    subreddit: "r/DeepSeek",
  },
  {
    id: "dotnet",
    icon: Cpu,
    color: "text-violet-600 dark:text-violet-400",
    bgColor: "bg-violet-50 dark:bg-violet-950",
    description: ".NET ve C# gelişmeleri",
    subreddit: "r/dotnet",
  },
  {
    id: "claudeai",
    icon: Cpu,
    color: "text-amber-600 dark:text-amber-400",
    bgColor: "bg-amber-50 dark:bg-amber-950",
    description: "Claude AI haberleri",
    subreddit: "r/ClaudeAI",
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
              Reddit'ten yapay zeka, robotik ve yazılım geliştirme haberleri
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
                          {category.id === "popular" && "Popüler"}
                          {category.id === "artificialintelligence" && "Yapay Zeka"}
                          {category.id === "githubcopilot" && "GitHub Copilot"}
                          {category.id === "mcp" && "MCP"}
                          {category.id === "openai" && "OpenAI"}
                          {category.id === "robotics" && "Robotik"}
                          {category.id === "deepseek" && "DeepSeek"}
                          {category.id === "dotnet" && ".NET"}
                          {category.id === "claudeai" && "Claude AI"}
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
                  Reddit'ten yapay zeka, robotik ve yazılım geliştirme haberleri
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="mt-4 grid grid-cols-3 gap-4">
                  <div className="text-center">
                    <div className="text-primary text-3xl font-bold">9</div>
                    <div className="text-muted-foreground text-sm">Teknoloji Kategorisi</div>
                  </div>
                  <div className="text-center">
                    <div className="text-primary text-3xl font-bold">Günlük</div>
                    <div className="text-muted-foreground text-sm">Otomatik Güncelleme</div>
                  </div>
                  <div className="text-center">
                    <div className="text-primary text-3xl font-bold">Top</div>
                    <div className="text-muted-foreground text-sm">Son 1 Hafta</div>
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
