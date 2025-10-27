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
    // Log the error to an error reporting service
    console.error('Dashboard error:', error);
  }, [error]);

  return (
    <div className="flex items-center justify-center min-h-[400px]">
      <Card className="p-8 max-w-md">
        <div className="flex flex-col items-center text-center space-y-4">
          <AlertCircle className="h-12 w-12 text-red-500" />
          <h2 className="text-xl font-semibold">Bir Hata Oluştu</h2>
          <p className="text-sm text-muted-foreground">
            Dashboard verilerini yüklerken bir sorun oluştu. Lütfen tekrar deneyin.
          </p>
          {error.message && (
            <p className="text-xs text-red-600 bg-red-50 p-2 rounded">
              {error.message}
            </p>
          )}
          <Button onClick={reset} className="mt-4">
            Tekrar Dene
          </Button>
        </div>
      </Card>
    </div>
  );
}
