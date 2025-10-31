"use client";

import { useEffect, useState } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Newspaper, TrendingUp, Eye, Calendar, ArrowUp, ArrowDown, Clock } from "lucide-react";

interface DashboardStats {
  totalNews: number;
  publishedNews: number;
  draftNews: number;
  totalViews: number;
  todayViews: number;
  weeklyChange: number;
}

interface RecentNews {
  id: string;
  title: string;
  category: string;
  publishedAt: string;
  views: number;
  status: "published" | "draft";
}

export default function AdminDashboardPage() {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [recentNews, setRecentNews] = useState<RecentNews[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

      // Fetch news for stats - Correct endpoint is /api/NewsArticle
      const newsResponse = await fetch(`${API_URL}/api/NewsArticle`);

      if (!newsResponse.ok) {
        throw new Error(`API returned ${newsResponse.status}`);
      }

      const newsData = await newsResponse.json();

      // Calculate stats - Backend uses IsActive instead of status
      const totalNews = newsData.length;
      const publishedNews = newsData.filter((n: any) => n.isActive === true).length;
      const draftNews = totalNews - publishedNews;
      const totalViews = newsData.reduce((sum: number, n: any) => sum + (n.views || 0), 0);
      const todayViews = Math.floor(totalViews * 0.1); // Mock data
      const weeklyChange = 12.5; // Mock data

      setStats({
        totalNews,
        publishedNews,
        draftNews,
        totalViews,
        todayViews,
        weeklyChange,
      });

      // Get recent news - Backend uses Caption as title and ExpressDate
      const recent = newsData
        .sort(
          (a: any, b: any) =>
            new Date(b.expressDate || b.createdDate).getTime() -
            new Date(a.expressDate || a.createdDate).getTime()
        )
        .slice(0, 5)
        .map((n: any) => ({
          id: n.id,
          title: n.caption || "Başlıksız",
          category: n.category,
          publishedAt: n.expressDate || n.createdDate,
          views: n.views || 0,
          status: n.isActive ? "published" : "draft",
        }));

      setRecentNews(recent);
    } catch (error) {
      console.error("Failed to fetch dashboard data:", error);

      // Set default empty stats on error
      setStats({
        totalNews: 0,
        publishedNews: 0,
        draftNews: 0,
        totalViews: 0,
        todayViews: 0,
        weeklyChange: 0,
      });
      setRecentNews([]);
    } finally {
      setLoading(false);
    }
  };

  const statCards = [
    {
      title: "Toplam Haber",
      value: stats?.totalNews || 0,
      icon: Newspaper,
      color: "text-blue-500",
      bgColor: "bg-blue-500/10",
    },
    {
      title: "Yayınlanan",
      value: stats?.publishedNews || 0,
      icon: TrendingUp,
      color: "text-green-500",
      bgColor: "bg-green-500/10",
    },
    {
      title: "Taslak",
      value: stats?.draftNews || 0,
      icon: Clock,
      color: "text-orange-500",
      bgColor: "bg-orange-500/10",
    },
    {
      title: "Toplam Görüntülenme",
      value: stats?.totalViews || 0,
      icon: Eye,
      color: "text-purple-500",
      bgColor: "bg-purple-500/10",
      change: stats?.weeklyChange,
    },
  ];

  if (loading) {
    return (
      <div className="space-y-6">
        <h1 className="text-3xl font-bold">Dashboard</h1>
        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4">
          {[1, 2, 3, 4].map((i) => (
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
      <div>
        <h1 className="text-3xl font-bold">Dashboard</h1>
        <p className="mt-1 text-slate-600 dark:text-slate-400">
          Hoş geldiniz! İşte sistemin genel görünümü.
        </p>
      </div>

      {/* Stats Grid */}
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4">
        {statCards.map((stat) => {
          const Icon = stat.icon;
          return (
            <Card key={stat.title}>
              <CardContent className="p-6">
                <div className="flex items-center justify-between">
                  <div className="space-y-2">
                    <p className="text-sm font-medium text-slate-600 dark:text-slate-400">
                      {stat.title}
                    </p>
                    <p className="text-3xl font-bold">{stat.value.toLocaleString("tr-TR")}</p>
                    {stat.change && (
                      <div className="flex items-center gap-1 text-sm">
                        {stat.change > 0 ? (
                          <>
                            <ArrowUp className="h-4 w-4 text-green-500" />
                            <span className="text-green-500">+{stat.change}%</span>
                          </>
                        ) : (
                          <>
                            <ArrowDown className="h-4 w-4 text-red-500" />
                            <span className="text-red-500">{stat.change}%</span>
                          </>
                        )}
                        <span className="text-slate-500">bu hafta</span>
                      </div>
                    )}
                  </div>
                  <div className={`${stat.bgColor} ${stat.color} rounded-full p-3`}>
                    <Icon className="h-6 w-6" />
                  </div>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {/* Recent News */}
      <Card>
        <CardHeader>
          <CardTitle>Son Haberler</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {recentNews.map((news) => (
              <div
                key={news.id}
                className="flex items-center justify-between rounded-lg border border-slate-200 p-4 transition-colors hover:bg-slate-50 dark:border-slate-800 dark:hover:bg-slate-900"
              >
                <div className="min-w-0 flex-1">
                  <h3 className="truncate font-medium">{news.title}</h3>
                  <div className="mt-1 flex items-center gap-3 text-sm text-slate-600 dark:text-slate-400">
                    <span className="capitalize">{news.category}</span>
                    <span>•</span>
                    <span className="flex items-center gap-1">
                      <Calendar className="h-3 w-3" />
                      {new Date(news.publishedAt).toLocaleDateString("tr-TR")}
                    </span>
                    <span>•</span>
                    <span className="flex items-center gap-1">
                      <Eye className="h-3 w-3" />
                      {news.views.toLocaleString("tr-TR")}
                    </span>
                  </div>
                </div>
                <div
                  className={`rounded-full px-3 py-1 text-xs font-medium ${
                    news.status === "published"
                      ? "bg-green-500/10 text-green-500"
                      : "bg-orange-500/10 text-orange-500"
                  }`}
                >
                  {news.status === "published" ? "Yayında" : "Taslak"}
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
