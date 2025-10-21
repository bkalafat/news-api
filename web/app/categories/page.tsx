import { Metadata } from 'next';
import Link from 'next/link';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { 
  Cpu, 
  Globe, 
  Briefcase, 
  Microscope, 
  Heart, 
  Film,
  TrendingUp,
  ArrowRight
} from 'lucide-react';
import { useTranslations } from 'next-intl';

export const metadata: Metadata = {
  title: 'Kategoriler - Teknoloji Haberleri',
  description: 'Tüm haber kategorilerine göz atın',
};

const categories = [
  {
    id: 'technology',
    icon: Cpu,
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-950',
    description: 'Teknoloji dünyasından son dakika haberleri, yenilikler ve analizler',
  },
  {
    id: 'world',
    icon: Globe,
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-950',
    description: 'Dünya genelinden önemli gelişmeler ve uluslararası haberler',
  },
  {
    id: 'business',
    icon: Briefcase,
    color: 'text-purple-600 dark:text-purple-400',
    bgColor: 'bg-purple-50 dark:bg-purple-950',
    description: 'İş dünyası, ekonomi ve finans haberleri',
  },
  {
    id: 'science',
    icon: Microscope,
    color: 'text-orange-600 dark:text-orange-400',
    bgColor: 'bg-orange-50 dark:bg-orange-950',
    description: 'Bilim dünyasından keşifler, araştırmalar ve gelişmeler',
  },
  {
    id: 'health',
    icon: Heart,
    color: 'text-red-600 dark:text-red-400',
    bgColor: 'bg-red-50 dark:bg-red-950',
    description: 'Sağlık, tıp ve yaşam haberleri',
  },
  {
    id: 'entertainment',
    icon: Film,
    color: 'text-pink-600 dark:text-pink-400',
    bgColor: 'bg-pink-50 dark:bg-pink-950',
    description: 'Eğlence, kültür ve sanat dünyasından haberler',
  },
];

export default function CategoriesPage() {
  return (
    <div className="container mx-auto px-4 py-12 max-w-6xl">
      {/* Header */}
      <div className="text-center mb-12">
        <div className="flex items-center justify-center gap-2 mb-4">
          <TrendingUp className="h-8 w-8 text-primary" />
          <h1 className="text-4xl md:text-5xl font-bold">Kategoriler</h1>
        </div>
        <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
          İlgilendiğiniz konulara göre haberleri keşfedin
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
                      {category.id === 'technology' && 'Teknoloji'}
                      {category.id === 'world' && 'Dünya'}
                      {category.id === 'business' && 'İş Dünyası'}
                      {category.id === 'science' && 'Bilim'}
                      {category.id === 'health' && 'Sağlık'}
                      {category.id === 'entertainment' && 'Eğlence'}
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
              BBC ve güvenilir kaynaklardan sürekli güncellenen içerikler
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-3 gap-4 mt-4">
              <div className="text-center">
                <div className="text-3xl font-bold text-primary">6</div>
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
  );
}
