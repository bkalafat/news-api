'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import {
  Plus,
  Search,
  Filter,
  Eye,
  Edit,
  Trash2,
  Calendar,
  Clock,
  MoreVertical,
} from 'lucide-react';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Badge } from '@/components/ui/badge';

interface NewsItem {
  id: string;
  title: string;
  slug: string;
  caption: string;
  category: string;
  author: string;
  publishedAt: string;
  views: number;
  status: 'published' | 'draft';
  imageUrl?: string;
}

export default function AdminNewsPage() {
  const [news, setNews] = useState<NewsItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<string>('all');

  useEffect(() => {
    fetchNews();
  }, []);

  const fetchNews = async () => {
    try {
      const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';
      const response = await fetch(`${API_URL}/api/News`);
      const data = await response.json();
      setNews(data);
    } catch (error) {
      console.error('Failed to fetch news:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Bu haberi silmek istediğinizden emin misiniz?')) return;

    try {
      const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';
      const response = await fetch(`${API_URL}/api/News/${id}`, {
        method: 'DELETE',
      });

      if (response.ok) {
        setNews(news.filter((item) => item.id !== id));
      }
    } catch (error) {
      console.error('Failed to delete news:', error);
      alert('Haber silinirken bir hata oluştu');
    }
  };

  const filteredNews = news.filter((item) => {
    const matchesSearch =
      item.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      item.caption.toLowerCase().includes(searchQuery.toLowerCase());
    const matchesCategory =
      selectedCategory === 'all' || item.category === selectedCategory;
    return matchesSearch && matchesCategory;
  });

  const categories = ['all', ...Array.from(new Set(news.map((item) => item.category)))];

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
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold">Haberler</h1>
          <p className="text-slate-600 dark:text-slate-400 mt-1">
            Toplam {filteredNews.length} haber
          </p>
        </div>
        <Link href="/admin/news/new">
          <Button>
            <Plus className="w-4 h-4 mr-2" />
            Yeni Haber
          </Button>
        </Link>
      </div>

      {/* Filters */}
      <Card>
        <CardContent className="p-4">
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" />
              <Input
                placeholder="Haber ara..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="pl-10"
              />
            </div>
            <div className="flex gap-2">
              <Button
                variant="outline"
                size="icon"
                className="sm:hidden"
              >
                <Filter className="w-4 h-4" />
              </Button>
              <div className="hidden sm:flex gap-2 overflow-x-auto">
                {categories.map((cat) => (
                  <Button
                    key={cat}
                    variant={selectedCategory === cat ? 'default' : 'outline'}
                    size="sm"
                    onClick={() => setSelectedCategory(cat)}
                    className="capitalize whitespace-nowrap"
                  >
                    {cat === 'all' ? 'Tümü' : cat}
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
          <Card key={item.id} className="overflow-hidden hover:shadow-md transition-shadow">
            <CardContent className="p-0">
              <div className="flex flex-col sm:flex-row">
                {/* Image */}
                {item.imageUrl && (
                  <div className="w-full sm:w-48 h-48 sm:h-auto bg-slate-200 dark:bg-slate-800 flex-shrink-0">
                    <img
                      src={item.imageUrl}
                      alt={item.title}
                      className="w-full h-full object-cover"
                    />
                  </div>
                )}

                {/* Content */}
                <div className="flex-1 p-6">
                  <div className="flex items-start justify-between gap-4">
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-2 mb-2">
                        <Badge variant="secondary" className="capitalize">
                          {item.category}
                        </Badge>
                        <Badge
                          variant={item.status === 'published' ? 'default' : 'outline'}
                        >
                          {item.status === 'published' ? 'Yayında' : 'Taslak'}
                        </Badge>
                      </div>

                      <h3 className="text-xl font-semibold mb-2 line-clamp-2">
                        {item.title}
                      </h3>

                      <p className="text-slate-600 dark:text-slate-400 line-clamp-2 mb-4">
                        {item.caption}
                      </p>

                      <div className="flex flex-wrap items-center gap-4 text-sm text-slate-500">
                        <span className="flex items-center gap-1">
                          <Calendar className="w-3 h-3" />
                          {new Date(item.publishedAt).toLocaleDateString('tr-TR')}
                        </span>
                        <span>•</span>
                        <span>{item.author}</span>
                        <span>•</span>
                        <span className="flex items-center gap-1">
                          <Eye className="w-3 h-3" />
                          {item.views?.toLocaleString('tr-TR') || 0} görüntülenme
                        </span>
                      </div>
                    </div>

                    {/* Actions */}
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button variant="ghost" size="icon" className="flex-shrink-0">
                          <MoreVertical className="w-4 h-4" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem asChild>
                          <Link
                            href={`/news/${item.slug}`}
                            target="_blank"
                            className="flex items-center cursor-pointer"
                          >
                            <Eye className="w-4 h-4 mr-2" />
                            Görüntüle
                          </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem asChild>
                          <Link
                            href={`/admin/news/edit/${item.id}`}
                            className="flex items-center cursor-pointer"
                          >
                            <Edit className="w-4 h-4 mr-2" />
                            Düzenle
                          </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem
                          onClick={() => handleDelete(item.id)}
                          className="text-red-600 focus:text-red-600"
                        >
                          <Trash2 className="w-4 h-4 mr-2" />
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
              <Newspaper className="w-12 h-12 mx-auto text-slate-400 mb-4" />
              <h3 className="text-lg font-semibold mb-2">Haber bulunamadı</h3>
              <p className="text-slate-600 dark:text-slate-400">
                {searchQuery
                  ? 'Arama kriterlerinize uygun haber bulunamadı.'
                  : 'Henüz hiç haber eklenmemiş.'}
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
    <svg
      className={className}
      fill="none"
      stroke="currentColor"
      viewBox="0 0 24 24"
    >
      <path
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth={2}
        d="M19 20H5a2 2 0 01-2-2V6a2 2 0 012-2h10a2 2 0 012 2v1m2 13a2 2 0 01-2-2V7m2 13a2 2 0 002-2V9a2 2 0 00-2-2h-2m-4-3H9M7 16h6M7 8h6v4H7V8z"
      />
    </svg>
  );
}
