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

// ISR: Revalidate every 7 days (static content, rarely changes)
export const revalidate = 604800; // 7 days

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

          {/* Mission */}
          <Card className="mb-8">
            <CardHeader>
              <CardTitle>Misyonumuz</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <p className="text-muted-foreground">
                Teknoloji Haberleri, Türkiye&apos;deki teknoloji meraklılarına en güncel teknoloji
                haberlerini, analizleri ve gelişmeleri sunmak için oluşturulmuş modern bir
                platformdur.
              </p>
              <p className="text-muted-foreground">
                Amacımız, en son web teknolojilerini kullanarak hızlı, güvenli ve kullanıcı dostu
                bir haber okuma deneyimi sunmaktır. Platform, modern yazılım geliştirme prensipleri
                ve en iyi pratiklerle geliştirilmiştir.
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
