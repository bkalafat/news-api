import { Metadata } from 'next';
import { Newspaper, Code, Zap, Shield } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';

export const metadata: Metadata = {
  title: 'Hakkımızda - Teknoloji Haberleri',
  description: 'Modern teknoloji haberleri platformu hakkında bilgi edinin',
};

export default function AboutPage() {
  const techStack = [
    'Next.js 15',
    'React 19',
    'TypeScript',
    'TailwindCSS v4',
    'Shadcn/ui',
    'React Query',
    '.NET 10',
    'MongoDB',
  ];

  const features = [
    {
      icon: Newspaper,
      title: 'Güvenilir Kaynaklar',
      description: 'BBC ve diğer güvenilir kaynaklardan güncel haberler',
    },
    {
      icon: Zap,
      title: 'Hızlı ve Modern',
      description: 'En son teknolojilerle geliştirilmiş, hızlı ve kullanıcı dostu arayüz',
    },
    {
      icon: Code,
      title: 'Açık Kaynak',
      description: 'Modern teknolojiler ve en iyi pratiklerle geliştirildi',
    },
    {
      icon: Shield,
      title: 'Güvenli',
      description: 'Güvenlik odaklı geliştirme ve güncel teknolojiler',
    },
  ];

  return (
    <div className="container mx-auto px-4 py-12 max-w-5xl">
      {/* Header */}
      <div className="text-center mb-12">
        <h1 className="text-4xl md:text-5xl font-bold mb-4">Hakkımızda</h1>
        <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
          Modern teknolojilerle geliştirilmiş, Türkçe teknoloji haberleri platformu
        </p>
      </div>

      {/* Mission */}
      <Card className="mb-8">
        <CardHeader>
          <CardTitle>Misyonumuz</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <p className="text-muted-foreground">
            Teknoloji Haberleri, Türkiye&apos;deki teknoloji meraklılarına BBC ve diğer
            güvenilir kaynaklardan gelen en güncel teknoloji haberlerini sunmak için
            oluşturulmuş modern bir platformdur.
          </p>
          <p className="text-muted-foreground">
            Amacımız, en son web teknolojilerini kullanarak hızlı, güvenli ve kullanıcı
            dostu bir haber okuma deneyimi sunmaktır. Platform, modern yazılım geliştirme
            prensipleri ve en iyi pratiklerle geliştirilmiştir.
          </p>
        </CardContent>
      </Card>

      {/* Features */}
      <div className="mb-12">
        <h2 className="text-3xl font-bold mb-6 text-center">Özellikler</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {features.map((feature) => {
            const Icon = feature.icon;
            return (
              <Card key={feature.title}>
                <CardHeader>
                  <div className="flex items-center gap-3">
                    <div className="p-2 rounded-lg bg-primary/10">
                      <Icon className="h-6 w-6 text-primary" />
                    </div>
                    <CardTitle>{feature.title}</CardTitle>
                  </div>
                </CardHeader>
                <CardContent>
                  <CardDescription>{feature.description}</CardDescription>
                </CardContent>
              </Card>
            );
          })}
        </div>
      </div>

      {/* Technology Stack */}
      <Card>
        <CardHeader>
          <CardTitle>Teknoloji Yığını</CardTitle>
          <CardDescription>
            Platform, en son ve en güçlü teknolojiler kullanılarak geliştirilmiştir
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex flex-wrap gap-2">
            {techStack.map((tech) => (
              <Badge key={tech} variant="secondary" className="text-sm">
                {tech}
              </Badge>
            ))}
          </div>
          <div className="mt-6 space-y-4">
            <div>
              <h3 className="font-semibold mb-2">Frontend</h3>
              <p className="text-sm text-muted-foreground">
                Next.js 15 App Router, React 19, TypeScript, TailwindCSS v4 ile modern,
                hızlı ve SEO-friendly bir kullanıcı arayüzü
              </p>
            </div>
            <div>
              <h3 className="font-semibold mb-2">Backend</h3>
              <p className="text-sm text-muted-foreground">
                .NET 10 Web API, Clean Architecture, MongoDB ile ölçeklenebilir ve
                sürdürülebilir backend mimarisi
              </p>
            </div>
            <div>
              <h3 className="font-semibold mb-2">Özellikler</h3>
              <ul className="text-sm text-muted-foreground list-disc list-inside space-y-1">
                <li>Server-side rendering (SSR) ile SEO optimizasyonu</li>
                <li>React Query ile akıllı veri önbellekleme</li>
                <li>Responsive tasarım ve mobil uyumluluk</li>
                <li>Dark mode desteği</li>
                <li>Sosyal medya paylaşım entegrasyonu</li>
                <li>Kategori bazlı haber filtreleme</li>
                <li>Arama ve sayfalama özellikleri</li>
              </ul>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Footer */}
      <div className="text-center mt-12 text-muted-foreground">
        <p className="text-sm">
          Bu proje, modern web teknolojilerinin gücünü göstermek için geliştirilmiştir.
        </p>
        <p className="text-sm mt-2">
          &copy; {new Date().getFullYear()} Teknoloji Haberleri. Tüm hakları saklıdır.
        </p>
      </div>
    </div>
  );
}
