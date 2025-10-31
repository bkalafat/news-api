"use client";

import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Home, Search, ArrowLeft, Newspaper } from "lucide-react";

export default function NotFound() {
  return (
    <div className="from-background to-muted flex min-h-screen items-center justify-center bg-gradient-to-br p-4">
      <Card className="w-full max-w-2xl">
        <CardHeader className="space-y-4 text-center">
          <div className="flex justify-center">
            <div className="bg-primary/10 flex h-24 w-24 items-center justify-center rounded-full">
              <Newspaper className="text-primary h-12 w-12" />
            </div>
          </div>
          <div>
            <div className="text-primary mb-2 text-6xl font-bold">404</div>
            <CardTitle className="text-3xl">Sayfa Bulunamadı</CardTitle>
          </div>
          <CardDescription className="text-lg">
            Aradığınız sayfa mevcut değil veya taşınmış olabilir.
          </CardDescription>
        </CardHeader>

        <CardContent className="space-y-6">
          {/* Suggestions */}
          <div className="bg-muted/50 space-y-3 rounded-lg p-6">
            <h3 className="text-lg font-semibold">Size Yardımcı Olabilecek Linkler:</h3>
            <div className="grid gap-3">
              <Link
                href="/"
                className="text-muted-foreground hover:text-primary flex items-center gap-2 transition-colors"
              >
                <Home className="h-4 w-4" />
                <span>Ana Sayfa</span>
              </Link>
              <Link
                href="/categories"
                className="text-muted-foreground hover:text-primary flex items-center gap-2 transition-colors"
              >
                <Newspaper className="h-4 w-4" />
                <span>Tüm Kategoriler</span>
              </Link>
              <Link
                href="/search"
                className="text-muted-foreground hover:text-primary flex items-center gap-2 transition-colors"
              >
                <Search className="h-4 w-4" />
                <span>Haber Ara</span>
              </Link>
              <Link
                href="/about"
                className="text-muted-foreground hover:text-primary flex items-center gap-2 transition-colors"
              >
                <Newspaper className="h-4 w-4" />
                <span>Hakkımızda</span>
              </Link>
            </div>
          </div>

          {/* Action Buttons */}
          <div className="flex gap-3 pt-4">
            <Button asChild className="flex-1">
              <Link href="/">
                <Home className="mr-2 h-4 w-4" />
                Ana Sayfaya Dön
              </Link>
            </Button>
            <Button variant="outline" className="flex-1" onClick={() => window.history.back()}>
              <ArrowLeft className="mr-2 h-4 w-4" />
              Geri Git
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
