'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { MarkdownEditor } from '@/components/admin/markdown-editor';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { ArrowLeft, Save, Eye } from 'lucide-react';
import Link from 'next/link';
import { authFetch } from '@/lib/auth';

interface NewsFormData {
  title: string;
  slug: string;
  caption: string;
  content: string;
  category: string;
  author: string;
  imageUrl: string;
  imgAlt: string;
  tags: string[];
  topics: string[];
  status: 'published' | 'draft';
}

const categories = [
  'technology',
  'sports',
  'world',
  'business',
  'science',
  'health',
  'entertainment',
];

export default function NewsEditorPage() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState<NewsFormData>({
    title: '',
    slug: '',
    caption: '',
    content: '',
    category: 'technology',
    author: '',
    imageUrl: '',
    imgAlt: '',
    tags: [],
    topics: [],
    status: 'draft',
  });

  // Auto-generate slug from title
  useEffect(() => {
    if (formData.title) {
      const slug = formData.title
        .toLowerCase()
        .replace(/ğ/g, 'g')
        .replace(/ü/g, 'u')
        .replace(/ş/g, 's')
        .replace(/ı/g, 'i')
        .replace(/ö/g, 'o')
        .replace(/ç/g, 'c')
        .replace(/[^a-z0-9]+/g, '-')
        .replace(/(^-|-$)/g, '');
      setFormData((prev) => ({ ...prev, slug }));
    }
  }, [formData.title]);

  const handleSubmit = async (e: React.FormEvent, status: 'published' | 'draft' = 'draft') => {
    e.preventDefault();
    setLoading(true);

    try {
      const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';
      
      // Backend NewsArticle DTO'suna uygun format
      const newsData = {
        category: formData.category,
        type: 'standard', // Backend için gerekli
        caption: formData.title, // Backend caption field'ı title için kullanılıyor
        keywords: formData.tags.filter(t => t).join(', '), // Array'i string'e çevir
        socialTags: formData.topics.filter(t => t).join(', '), // Array'i string'e çevir
        summary: formData.caption, // Frontend caption backend summary'ye map ediliyor
        imgPath: formData.imageUrl,
        imgAlt: formData.imgAlt,
        imageUrl: formData.imageUrl,
        thumbnailUrl: formData.imageUrl,
        content: formData.content,
        subjects: formData.topics.filter(t => t), // Boş elemanları temizle
        authors: [formData.author],
        expressDate: new Date().toISOString(),
        priority: status === 'published' ? 1 : 0,
        isActive: status === 'published',
        isSecondPageNews: false,
      };

      console.log('Sending news data:', newsData); // Debug için

      const response = await authFetch(`${API_URL}/api/NewsArticle`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(newsData),
      });

      if (response.ok) {
        alert('Haber başarıyla kaydedildi!');
        router.push('/admin/news');
      } else {
        const errorData = await response.json().catch(() => ({ message: 'Bilinmeyen hata' }));
        console.error('Error response:', errorData);
        alert(`Haber kaydedilemedi: ${errorData.message || JSON.stringify(errorData)}`);
      }
    } catch (error) {
      console.error('Failed to save news:', error);
      alert('Bir hata oluştu: ' + (error instanceof Error ? error.message : 'Bilinmeyen hata'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-5xl mx-auto space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Link href="/admin/news">
            <Button variant="ghost" size="icon">
              <ArrowLeft className="w-5 h-5" />
            </Button>
          </Link>
          <div>
            <h1 className="text-3xl font-bold">Yeni Haber Oluştur</h1>
            <p className="text-slate-600 dark:text-slate-400 mt-1">
              Tüm alanları doldurun ve yayınlayın
            </p>
          </div>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={(e) => handleSubmit(e, 'draft')} disabled={loading}>
            <Save className="w-4 h-4 mr-2" />
            Taslak Kaydet
          </Button>
          <Button onClick={(e) => handleSubmit(e, 'published')} disabled={loading}>
            <Eye className="w-4 h-4 mr-2" />
            Yayınla
          </Button>
        </div>
      </div>

      <form onSubmit={(e) => handleSubmit(e, 'published')} className="space-y-6">
        {/* Basic Info */}
        <Card>
          <CardHeader>
            <CardTitle>Temel Bilgiler</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="title">Başlık *</Label>
              <Input
                id="title"
                value={formData.title}
                onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                placeholder="Haber başlığını girin"
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="slug">URL Slug</Label>
              <Input
                id="slug"
                value={formData.slug}
                onChange={(e) => setFormData({ ...formData, slug: e.target.value })}
                placeholder="url-slug"
                className="font-mono text-sm"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="caption">Özet *</Label>
              <textarea
                id="caption"
                value={formData.caption}
                onChange={(e) => setFormData({ ...formData, caption: e.target.value })}
                placeholder="Haber özeti (maksimum 500 karakter)"
                className="w-full min-h-[100px] px-3 py-2 rounded-md border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-950 resize-y"
                maxLength={500}
                required
              />
              <p className="text-xs text-slate-500">{formData.caption.length}/500 karakter</p>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="category">Kategori *</Label>
                <Select
                  value={formData.category}
                  onValueChange={(value) => setFormData({ ...formData, category: value })}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Kategori seçin" />
                  </SelectTrigger>
                  <SelectContent>
                    {categories.map((cat) => (
                      <SelectItem key={cat} value={cat} className="capitalize">
                        {cat}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label htmlFor="author">Yazar *</Label>
                <Input
                  id="author"
                  value={formData.author}
                  onChange={(e) => setFormData({ ...formData, author: e.target.value })}
                  placeholder="Yazar adı"
                  required
                />
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Content */}
        <Card>
          <CardHeader>
            <CardTitle>İçerik (Markdown)</CardTitle>
          </CardHeader>
          <CardContent>
            <MarkdownEditor
              value={formData.content}
              onChange={(value) => setFormData({ ...formData, content: value })}
              placeholder="Haber içeriğini Markdown formatında yazın. Resim eklemek için araç çubuğundaki 'Resim Ekle' butonunu kullanın..."
            />
          </CardContent>
        </Card>

        {/* Media */}
        <Card>
          <CardHeader>
            <CardTitle>Medya</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="imageUrl">Görsel URL *</Label>
              <Input
                id="imageUrl"
                type="url"
                value={formData.imageUrl}
                onChange={(e) => setFormData({ ...formData, imageUrl: e.target.value })}
                placeholder="https://example.com/image.jpg"
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="imgAlt">Görsel Açıklaması</Label>
              <Input
                id="imgAlt"
                value={formData.imgAlt}
                onChange={(e) => setFormData({ ...formData, imgAlt: e.target.value })}
                placeholder="Görselin açıklaması (SEO için önemli)"
              />
            </div>

            {formData.imageUrl && (
              <div className="border rounded-lg p-4">
                <p className="text-sm font-medium mb-2">Görsel Önizleme:</p>
                <img
                  src={formData.imageUrl}
                  alt={formData.imgAlt || 'Preview'}
                  className="w-full max-w-md rounded-lg"
                  onError={(e) => {
                    e.currentTarget.src = '/placeholder.png';
                  }}
                />
              </div>
            )}
          </CardContent>
        </Card>

        {/* Metadata */}
        <Card>
          <CardHeader>
            <CardTitle>Metadata & SEO</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="tags">Etiketler (virgülle ayırın)</Label>
              <Input
                id="tags"
                value={formData.tags.join(', ')}
                onChange={(e) =>
                  setFormData({ ...formData, tags: e.target.value.split(',').map((t) => t.trim()) })
                }
                placeholder="yapay zeka, teknoloji, GPT-5"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="topics">Konular (virgülle ayırın)</Label>
              <Input
                id="topics"
                value={formData.topics.join(', ')}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    topics: e.target.value.split(',').map((t) => t.trim()),
                  })
                }
                placeholder="Yapay Zeka, Teknoloji, İnovasyon"
              />
            </div>
          </CardContent>
        </Card>
      </form>
    </div>
  );
}
