import { Metadata } from "next";
import { Newspaper, Code, Zap, Shield } from "lucide-react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Header } from "@/components/layout/header";
import { Footer } from "@/components/layout/footer";

export const metadata: Metadata = {
  title: "Hakkımızda - Teknoloji Haberleri",
  description:
    "Türkiye'nin en modern teknoloji haber platformu. Next.js, React, TypeScript ve .NET teknolojileri ile geliştirilmiş, hızlı ve kullanıcı dostu haber okuma deneyimi.",
  alternates: {
    canonical: "/about",
  },
  openGraph: {
    title: "Hakkımızda - Teknoloji Haberleri",
    description:
      "Türkiye'nin en modern teknoloji haber platformu. Next.js, React, TypeScript ve .NET teknolojileri ile geliştirilmiş.",
    type: "website",
    url: "/about",
  },
  twitter: {
    card: "summary",
    title: "Hakkımızda - Teknoloji Haberleri",
    description:
      "Türkiye'nin en modern teknoloji haber platformu. Next.js, React, TypeScript ve .NET teknolojileri ile geliştirilmiş.",
  },
};

// ISR: Revalidate every hour for profile updates
export const revalidate = 3600; // 1 hour

export default function AboutPage() {
  const techStack = [
    "Next.js 16",
    "React 19",
    "TypeScript",
    "TailwindCSS v4",
    "Shadcn/ui",
    "React Query",
    ".NET 10",
    "MongoDB",
  ];

  const features = [
    {
      icon: Newspaper,
      title: "Güncel Haberler",
      description: "Teknoloji dünyasından son dakika haberleri ve güncel gelişmeler",
    },
    {
      icon: Zap,
      title: "Hızlı ve Modern",
      description: "En son teknolojilerle geliştirilmiş, hızlı ve kullanıcı dostu arayüz",
    },
    {
      icon: Code,
      title: "Açık Kaynak",
      description: "Modern teknolojiler ve en iyi pratiklerle geliştirildi",
    },
    {
      icon: Shield,
      title: "Güvenli",
      description: "Güvenlik odaklı geliştirme ve güncel teknolojiler",
    },
  ];

  return (
    <div className="flex min-h-screen flex-col">
      <Header />
      <main className="flex-1">
        <div className="container mx-auto max-w-5xl px-4 py-12">
          {/* Header */}
          <div className="mb-12 text-center">
            <h1 className="mb-4 text-4xl font-bold md:text-5xl">Hakkımızda</h1>
            <p className="text-muted-foreground mx-auto max-w-2xl text-xl">
              Modern teknolojilerle geliştirilmiş, Türkçe teknoloji haberleri platformu
            </p>
          </div>

          {/* Developer Profile */}
          <Card className="mb-8">
            <CardHeader>
              <CardTitle>Geliştirici</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="flex items-start gap-4">
                <div className="flex-1">
                  <h3 className="mb-2 text-xl font-semibold">Burak Kalafat</h3>
                  <p className="text-muted-foreground mb-2 text-sm font-medium">
                    Senior Software Engineer (Backend, .NET)
                  </p>
                  <p className="text-muted-foreground mb-4">
                    12+ yıl deneyimli kıdemli yazılım mühendisi. Yapay zeka destekli geliştirme, 
                    Clean Architecture ve yüksek performanslı backend sistemleri konusunda uzman. 
                    GitHub Copilot Enterprise şampiyonu olarak, büyük ölçekli legacy modernizasyon 
                    projelerinde 6-8× hızlanma sağlayan AI-assisted development tekniklerinde öncü.
                  </p>
                  <div className="flex flex-wrap gap-2">
                    <Badge variant="secondary">C# & .NET Core</Badge>
                    <Badge variant="secondary">GitHub Copilot Enterprise</Badge>
                    <Badge variant="secondary">Clean Architecture</Badge>
                    <Badge variant="secondary">Azure DevOps</Badge>
                    <Badge variant="secondary">REST APIs</Badge>
                    <Badge variant="secondary">MongoDB</Badge>
                  </div>
                  <div className="mt-4 flex gap-4 text-sm">
                    <a 
                      href="https://github.com/bkalafat" 
                      target="_blank" 
                      rel="noopener noreferrer"
                      className="text-primary hover:underline"
                    >
                      GitHub
                    </a>
                    <a 
                      href="https://linkedin.com/in/bkalafat" 
                      target="_blank" 
                      rel="noopener noreferrer"
                      className="text-primary hover:underline"
                    >
                      LinkedIn
                    </a>
                    <a 
                      href="mailto:burakkalafat89@gmail.com"
                      className="text-primary hover:underline"
                    >
                      Email
                    </a>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Mission */}
          <Card className="mb-8">
            <CardHeader>
              <CardTitle>Proje Hakkında</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <p className="text-muted-foreground">
                Bu proje, Reddit&apos;ten yapay zeka, robotik ve yazılım geliştirme haberlerini 
                toplayan modern bir haber platformudur. OAuth 2.0 entegrasyonu ile günlük otomatik 
                haber güncellemesi sağlar.
              </p>
              <p className="text-muted-foreground">
                Platform, en son web teknolojileri ve best practices kullanılarak geliştirilmiştir. 
                Clean Architecture prensiplerine uygun, ölçeklenebilir ve sürdürülebilir bir mimari 
                ile .NET 10 backend ve Next.js 16 frontend teknolojilerini birleştirir.
              </p>
            </CardContent>
          </Card>

          {/* Features */}
          <div className="mb-12">
            <h2 className="mb-6 text-center text-3xl font-bold">Özellikler</h2>
            <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
              {features.map((feature) => {
                const Icon = feature.icon;
                return (
                  <Card key={feature.title}>
                    <CardHeader>
                      <div className="flex items-center gap-3">
                        <div className="bg-primary/10 rounded-lg p-2">
                          <Icon className="text-primary h-6 w-6" />
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
                  <h3 className="mb-2 font-semibold">Frontend</h3>
                  <p className="text-muted-foreground text-sm">
                    Next.js 16 App Router, React 19, TypeScript, TailwindCSS v4 ile modern, hızlı ve
                    SEO-friendly bir kullanıcı arayüzü
                  </p>
                </div>
                <div>
                  <h3 className="mb-2 font-semibold">Backend</h3>
                  <p className="text-muted-foreground text-sm">
                    .NET 10 Web API, Clean Architecture, MongoDB ile ölçeklenebilir ve
                    sürdürülebilir backend mimarisi
                  </p>
                </div>
                <div>
                  <h3 className="mb-2 font-semibold">Özellikler</h3>
                  <ul className="text-muted-foreground list-inside list-disc space-y-1 text-sm">
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
          <div className="text-muted-foreground mt-12 text-center">
            <p className="text-sm">
              Bu proje, modern web teknolojilerinin gücünü göstermek için geliştirilmiştir.
            </p>
            <p className="mt-2 text-sm">
              &copy; {new Date().getFullYear()} Teknoloji Haberleri. Tüm hakları saklıdır.
            </p>
          </div>
        </div>
      </main>
      <Footer />
    </div>
  );
}
