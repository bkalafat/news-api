'use client';

import { useEffect, useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { authFetch } from '@/lib/auth';
import {
  Newspaper,
  TrendingUp,
  Eye,
  Users,
  Calendar,
  ArrowUp,
  ArrowDown,
  Clock,
} from 'lucide-react';

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
  status: 'published' | 'draft';
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
      const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';
      
      // Fetch news for stats
      const newsResponse = await fetch(`${API_URL}/api/News`);
      const newsData = await newsResponse.json();

      // Calculate stats
      const totalNews = newsData.length;
      const publishedNews = newsData.filter((n: any) => n.status === 'published').length;
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

      // Get recent news
      const recent = newsData
        .sort((a: any, b: any) => new Date(b.publishedAt).getTime() - new Date(a.publishedAt).getTime())
        .slice(0, 5)
        .map((n: any) => ({
          id: n.id,
          title: n.title,
          category: n.category,
          publishedAt: n.publishedAt,
          views: n.views || 0,
          status: n.status || 'published',
        }));

      setRecentNews(recent);
    } catch (error) {
      console.error('Failed to fetch dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const statCards = [
    {
      title: 'Toplam Haber',
      value: stats?.totalNews || 0,
      icon: Newspaper,
      color: 'text-blue-500',
      bgColor: 'bg-blue-500/10',
    },
    {
      title: 'Yayınlanan',
      value: stats?.publishedNews || 0,
      icon: TrendingUp,
      color: 'text-green-500',
      bgColor: 'bg-green-500/10',
    },
    {
      title: 'Taslak',
      value: stats?.draftNews || 0,
      icon: Clock,
      color: 'text-orange-500',
      bgColor: 'bg-orange-500/10',
    },
    {
      title: 'Toplam Görüntülenme',
      value: stats?.totalViews || 0,
      icon: Eye,
      color: 'text-purple-500',
      bgColor: 'bg-purple-500/10',
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
                <div className="h-20 bg-slate-100 dark:bg-slate-800 rounded animate-pulse" />
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
        <p className="text-slate-600 dark:text-slate-400 mt-1">
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
                    <p className="text-3xl font-bold">
                      {stat.value.toLocaleString('tr-TR')}
                    </p>
                    {stat.change && (
                      <div className="flex items-center gap-1 text-sm">
                        {stat.change > 0 ? (
                          <>
                            <ArrowUp className="w-4 h-4 text-green-500" />
                            <span className="text-green-500">+{stat.change}%</span>
                          </>
                        ) : (
                          <>
                            <ArrowDown className="w-4 h-4 text-red-500" />
                            <span className="text-red-500">{stat.change}%</span>
                          </>
                        )}
                        <span className="text-slate-500">bu hafta</span>
                      </div>
                    )}
                  </div>
                  <div className={`${stat.bgColor} ${stat.color} p-3 rounded-full`}>
                    <Icon className="w-6 h-6" />
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
                className="flex items-center justify-between p-4 border border-slate-200 dark:border-slate-800 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-900 transition-colors"
              >
                <div className="flex-1 min-w-0">
                  <h3 className="font-medium truncate">{news.title}</h3>
                  <div className="flex items-center gap-3 mt-1 text-sm text-slate-600 dark:text-slate-400">
                    <span className="capitalize">{news.category}</span>
                    <span>•</span>
                    <span className="flex items-center gap-1">
                      <Calendar className="w-3 h-3" />
                      {new Date(news.publishedAt).toLocaleDateString('tr-TR')}
                    </span>
                    <span>•</span>
                    <span className="flex items-center gap-1">
                      <Eye className="w-3 h-3" />
                      {news.views.toLocaleString('tr-TR')}
                    </span>
                  </div>
                </div>
                <div
                  className={`px-3 py-1 rounded-full text-xs font-medium ${
                    news.status === 'published'
                      ? 'bg-green-500/10 text-green-500'
                      : 'bg-orange-500/10 text-orange-500'
                  }`}
                >
                  {news.status === 'published' ? 'Yayında' : 'Taslak'}
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
