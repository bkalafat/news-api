'use client';

import { useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { AlertCircle } from 'lucide-react';

export default function Error({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  useEffect(() => {
    console.error('News list error:', error);
  }, [error]);

  return (
    <div className="flex items-center justify-center min-h-[400px]">
      <Card className="p-8 max-w-md">
        <div className="flex flex-col items-center text-center space-y-4">
          <AlertCircle className="h-12 w-12 text-red-500" />
          <h2 className="text-xl font-semibold">Haberler Yüklenemedi</h2>
          <p className="text-sm text-muted-foreground">
            Haber listesini yüklerken bir sorun oluştu. Lütfen sayfayı yenileyin.
          </p>
          {error.message && (
            <p className="text-xs text-red-600 bg-red-50 p-2 rounded">
              {error.message}
            </p>
          )}
          <div className="flex gap-2">
            <Button onClick={reset} className="mt-4">
              Tekrar Dene
            </Button>
            <Button onClick={() => window.location.href = '/admin/dashboard'} variant="outline" className="mt-4">
              Dashboard'a Dön
            </Button>
          </div>
        </div>
      </Card>
    </div>
  );
}
