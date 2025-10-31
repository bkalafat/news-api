"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Plus, Search, Filter, Eye, Edit, Trash2, Calendar, MoreVertical } from "lucide-react";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Badge } from "@/components/ui/badge";

interface NewsItem {
  id: string;
  title: string;
  slug: string;
  caption: string;
  category: string;
  author: string;
  publishedAt: string;
  views: number;
  status: "published" | "draft";
  imageUrl?: string;
}

export default function AdminNewsPage() {
  const [news, setNews] = useState<NewsItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedCategory, setSelectedCategory] = useState<string>("all");

  useEffect(() => {
    fetchNews();
  }, []);

  const fetchNews = async () => {
    try {
      const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
      const response = await fetch(`${API_URL}/api/NewsArticle`);

      if (!response.ok) {
        throw new Error(`API returned ${response.status}`);
      }

      const data = await response.json();

      // Map backend data to frontend format
      const mappedData = data.map((article: any) => ({
        id: article.id,
        title: article.caption || "Başlıksız",
        category: article.category,
        author: article.author || "Bilinmeyen",
        publishedAt: article.expressDate || article.createdDate,
        status: article.isActive ? "published" : "draft",
        views: article.views || 0,
      }));

      setNews(mappedData);
    } catch (error) {
      console.error("Failed to fetch news:", error);
      setNews([]);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Bu haberi silmek istediğinizden emin misiniz?")) return;

    try {
      const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
      const response = await fetch(`${API_URL}/api/News/${id}`, {
        method: "DELETE",
      });

      if (response.ok) {
        setNews(news.filter((item) => item.id !== id));
      }
    } catch (error) {
      console.error("Failed to delete news:", error);
      alert("Haber silinirken bir hata oluştu");
    }
  };

  const filteredNews = news.filter((item) => {
    const matchesSearch =
      item.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      item.caption.toLowerCase().includes(searchQuery.toLowerCase());
    const matchesCategory = selectedCategory === "all" || item.category === selectedCategory;
    return matchesSearch && matchesCategory;
  });

  const categories = ["all", ...Array.from(new Set(news.map((item) => item.category)))];

  if (loading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <h1 className="text-3xl font-bold">Haberler</h1>
        </div>
        <div className="grid gap-4">
          {[1, 2, 3].map((i) => (
            <Card key={i}>
              <CardContent className="p-6">
                <div className="h-20 animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold">Haberler</h1>
          <p className="mt-1 text-slate-600 dark:text-slate-400">
            Toplam {filteredNews.length} haber
          </p>
        </div>
        <Link href="/admin/news/new">
          <Button>
            <Plus className="mr-2 h-4 w-4" />
            Yeni Haber
          </Button>
        </Link>
      </div>

      {/* Filters */}
      <Card>
        <CardContent className="p-4">
          <div className="flex flex-col gap-4 sm:flex-row">
            <div className="relative flex-1">
              <Search className="absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 text-slate-400" />
              <Input
                placeholder="Haber ara..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="pl-10"
              />
            </div>
            <div className="flex gap-2">
              <Button variant="outline" size="icon" className="sm:hidden">
                <Filter className="h-4 w-4" />
              </Button>
              <div className="hidden gap-2 overflow-x-auto sm:flex">
                {categories.map((cat) => (
                  <Button
                    key={cat}
                    variant={selectedCategory === cat ? "default" : "outline"}
                    size="sm"
                    onClick={() => setSelectedCategory(cat)}
                    className="whitespace-nowrap capitalize"
                  >
                    {cat === "all" ? "Tümü" : cat}
                  </Button>
                ))}
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* News List */}
      <div className="grid gap-4">
        {filteredNews.map((item) => (
          <Card key={item.id} className="overflow-hidden transition-shadow hover:shadow-md">
            <CardContent className="p-0">
              <div className="flex flex-col sm:flex-row">
                {/* Image */}
                {item.imageUrl && (
                  <div className="h-48 w-full flex-shrink-0 bg-slate-200 sm:h-auto sm:w-48 dark:bg-slate-800">
                    <img
                      src={item.imageUrl}
                      alt={item.title}
                      className="h-full w-full object-cover"
                    />
                  </div>
                )}

                {/* Content */}
                <div className="flex-1 p-6">
                  <div className="flex items-start justify-between gap-4">
                    <div className="min-w-0 flex-1">
                      <div className="mb-2 flex items-center gap-2">
                        <Badge variant="secondary" className="capitalize">
                          {item.category}
                        </Badge>
                        <Badge variant={item.status === "published" ? "default" : "outline"}>
                          {item.status === "published" ? "Yayında" : "Taslak"}
                        </Badge>
                      </div>

                      <h3 className="mb-2 line-clamp-2 text-xl font-semibold">{item.title}</h3>

                      <p className="mb-4 line-clamp-2 text-slate-600 dark:text-slate-400">
                        {item.caption}
                      </p>

                      <div className="flex flex-wrap items-center gap-4 text-sm text-slate-500">
                        <span className="flex items-center gap-1">
                          <Calendar className="h-3 w-3" />
                          {new Date(item.publishedAt).toLocaleDateString("tr-TR")}
                        </span>
                        <span>•</span>
                        <span>{item.author}</span>
                        <span>•</span>
                        <span className="flex items-center gap-1">
                          <Eye className="h-3 w-3" />
                          {item.views?.toLocaleString("tr-TR") || 0} görüntülenme
                        </span>
                      </div>
                    </div>

                    {/* Actions */}
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button variant="ghost" size="icon" className="flex-shrink-0">
                          <MoreVertical className="h-4 w-4" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem asChild>
                          <Link
                            href={`/news/${item.slug}`}
                            target="_blank"
                            className="flex cursor-pointer items-center"
                          >
                            <Eye className="mr-2 h-4 w-4" />
                            Görüntüle
                          </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem asChild>
                          <Link
                            href={`/admin/news/edit/${item.id}`}
                            className="flex cursor-pointer items-center"
                          >
                            <Edit className="mr-2 h-4 w-4" />
                            Düzenle
                          </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem
                          onClick={() => handleDelete(item.id)}
                          className="text-red-600 focus:text-red-600"
                        >
                          <Trash2 className="mr-2 h-4 w-4" />
                          Sil
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}

        {filteredNews.length === 0 && (
          <Card>
            <CardContent className="p-12 text-center">
              <Newspaper className="mx-auto mb-4 h-12 w-12 text-slate-400" />
              <h3 className="mb-2 text-lg font-semibold">Haber bulunamadı</h3>
              <p className="text-slate-600 dark:text-slate-400">
                {searchQuery
                  ? "Arama kriterlerinize uygun haber bulunamadı."
                  : "Henüz hiç haber eklenmemiş."}
              </p>
            </CardContent>
          </Card>
        )}
      </div>
    </div>
  );
}

function Newspaper({ className }: { className?: string }) {
  return (
    <svg className={className} fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth={2}
        d="M19 20H5a2 2 0 01-2-2V6a2 2 0 012-2h10a2 2 0 012 2v1m2 13a2 2 0 01-2-2V7m2 13a2 2 0 002-2V9a2 2 0 00-2-2h-2m-4-3H9M7 16h6M7 8h6v4H7V8z"
      />
    </svg>
  );
}
