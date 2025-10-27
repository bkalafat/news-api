'use client';

import Link from 'next/link';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Home, Search, ArrowLeft, Newspaper } from 'lucide-react';

export default function NotFound() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-background to-muted p-4">
      <Card className="max-w-2xl w-full">
        <CardHeader className="text-center space-y-4">
          <div className="flex justify-center">
            <div className="w-24 h-24 rounded-full bg-primary/10 flex items-center justify-center">
              <Newspaper className="w-12 h-12 text-primary" />
            </div>
          </div>
          <div>
            <div className="text-6xl font-bold text-primary mb-2">404</div>
            <CardTitle className="text-3xl">Sayfa Bulunamadı</CardTitle>
          </div>
          <CardDescription className="text-lg">
            Aradığınız sayfa mevcut değil veya taşınmış olabilir.
          </CardDescription>
        </CardHeader>
        
        <CardContent className="space-y-6">
          {/* Suggestions */}
          <div className="bg-muted/50 rounded-lg p-6 space-y-3">
            <h3 className="font-semibold text-lg">Size Yardımcı Olabilecek Linkler:</h3>
            <div className="grid gap-3">
              <Link 
                href="/" 
                className="flex items-center gap-2 text-muted-foreground hover:text-primary transition-colors"
              >
                <Home className="h-4 w-4" />
                <span>Ana Sayfa</span>
              </Link>
              <Link 
                href="/categories" 
                className="flex items-center gap-2 text-muted-foreground hover:text-primary transition-colors"
              >
                <Newspaper className="h-4 w-4" />
                <span>Tüm Kategoriler</span>
              </Link>
              <Link 
                href="/search" 
                className="flex items-center gap-2 text-muted-foreground hover:text-primary transition-colors"
              >
                <Search className="h-4 w-4" />
                <span>Haber Ara</span>
              </Link>
              <Link 
                href="/about" 
                className="flex items-center gap-2 text-muted-foreground hover:text-primary transition-colors"
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
            <Button 
              variant="outline" 
              className="flex-1"
              onClick={() => window.history.back()}
            >
              <ArrowLeft className="mr-2 h-4 w-4" />
              Geri Git
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
