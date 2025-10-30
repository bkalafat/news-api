import { Metadata } from 'next';
import Link from 'next/link';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
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
  ArrowRight
} from 'lucide-react';
import { useTranslations } from 'next-intl';
import { Header } from '@/components/layout/header';
import { Footer } from '@/components/layout/footer';

export const metadata: Metadata = {
  title: 'Kategoriler - Teknoloji Haberleri',
  description: 'Tüm haber kategorilerine göz atın',
};

// ISR: Revalidate every 24 hours (static content, no frequent changes)
export const revalidate = 86400; // 24 hours

const categories = [
  {
    id: 'reddit',
    icon: MessageSquare,
    color: 'text-orange-600 dark:text-orange-400',
    bgColor: 'bg-orange-50 dark:bg-orange-950',
    description: 'Reddit\'ten teknoloji ve toplum haberleri',
  },
  {
    id: 'github',
    icon: Github,
    color: 'text-gray-900 dark:text-gray-100',
    bgColor: 'bg-gray-50 dark:bg-gray-950',
    description: 'GitHub\'tan geliştirici haberleri ve açık kaynak projeleri',
  },
  {
    id: 'twitter',
    icon: Twitter,
    color: 'text-blue-400 dark:text-blue-300',
    bgColor: 'bg-blue-50 dark:bg-blue-950',
    description: 'X/Twitter\'dan sosyal medya trendleri ve haberler',
  },
  {
    id: 'linkedin',
    icon: Linkedin,
    color: 'text-blue-700 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-950',
    description: 'LinkedIn\'den iş dünyası ve kariyer haberleri',
  },
  {
    id: 'facebook',
    icon: Facebook,
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-950',
    description: 'Facebook\'tan sosyal medya haberleri ve gelişmeler',
  },
  {
    id: 'instagram',
    icon: Instagram,
    color: 'text-pink-600 dark:text-pink-400',
    bgColor: 'bg-pink-50 dark:bg-pink-950',
    description: 'Instagram\'dan görsel içerik ve trendler',
  },
  {
    id: 'tiktok',
    icon: Music,
    color: 'text-gray-900 dark:text-gray-100',
    bgColor: 'bg-gray-50 dark:bg-gray-950',
    description: 'TikTok\'tan video içerik ve viral trendler',
  },
  {
    id: 'youtube',
    icon: Youtube,
    color: 'text-red-600 dark:text-red-400',
    bgColor: 'bg-red-50 dark:bg-red-950',
    description: 'YouTube\'dan video platformu haberleri',
  },
  {
    id: 'technology',
    icon: Cpu,
    color: 'text-purple-600 dark:text-purple-400',
    bgColor: 'bg-purple-50 dark:bg-purple-950',
    description: 'Genel teknoloji haberleri ve yenilikler',
  },
];

export default function CategoriesPage() {
  return (
    <div className="min-h-screen flex flex-col">
      <Header />
      <main className="flex-1">
        <div className="container mx-auto px-4 py-12 max-w-6xl">
          {/* Header */}
          <div className="text-center mb-12">
            <div className="flex items-center justify-center gap-2 mb-4">
              <TrendingUp className="h-8 w-8 text-primary" />
              <h1 className="text-4xl md:text-5xl font-bold">Kategoriler</h1>
            </div>
            <p className="text-muted-foreground text-lg max-w-2xl mx-auto">
              Sosyal medya platformlarından ve teknoloji kaynaklarından haberler
            </p>
          </div>

          {/* Categories Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {categories.map((category) => {
              const Icon = category.icon;
              return (
                <Link 
                  key={category.id} 
                  href={`/category/${category.id}`}
                  className="group"
                >
                  <Card className="h-full transition-all duration-300 hover:shadow-lg hover:-translate-y-1 cursor-pointer">
                    <CardHeader>
                      <div className={`w-16 h-16 rounded-lg ${category.bgColor} flex items-center justify-center mb-4 group-hover:scale-110 transition-transform`}>
                        <Icon className={`h-8 w-8 ${category.color}`} />
                      </div>
                      <div className="flex items-center justify-between">
                        <CardTitle className="capitalize">
                          {category.id === 'reddit' && 'Reddit'}
                          {category.id === 'github' && 'GitHub'}
                          {category.id === 'twitter' && 'X/Twitter'}
                          {category.id === 'linkedin' && 'LinkedIn'}
                          {category.id === 'facebook' && 'Facebook'}
                          {category.id === 'instagram' && 'Instagram'}
                          {category.id === 'tiktok' && 'TikTok'}
                          {category.id === 'youtube' && 'YouTube'}
                          {category.id === 'technology' && 'Teknoloji'}
                        </CardTitle>
                        <ArrowRight className="h-5 w-5 text-muted-foreground group-hover:translate-x-1 transition-transform" />
                      </div>
                    </CardHeader>
                    <CardContent>
                      <CardDescription>
                        {category.description}
                      </CardDescription>
                    </CardContent>
                  </Card>
                </Link>
              );
            })}
          </div>

          {/* Stats Section */}
          <div className="mt-16 text-center">
            <Card className="max-w-2xl mx-auto">
              <CardHeader>
                <CardTitle>Her Gün Yeni Haberler</CardTitle>
                <CardDescription>
                  Sosyal medya ve teknoloji kaynaklarından sürekli güncellenen içerikler
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-3 gap-4 mt-4">
                  <div className="text-center">
                    <div className="text-3xl font-bold text-primary">9</div>
                    <div className="text-sm text-muted-foreground">Kategori</div>
                  </div>
                  <div className="text-center">
                    <div className="text-3xl font-bold text-primary">24/7</div>
                    <div className="text-sm text-muted-foreground">Güncelleme</div>
                  </div>
                  <div className="text-center">
                    <div className="text-3xl font-bold text-primary">∞</div>
                    <div className="text-sm text-muted-foreground">İçerik</div>
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
