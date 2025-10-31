"use client";

import { useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { AlertCircle } from "lucide-react";

export default function Error({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  useEffect(() => {
    // Log the error to an error reporting service
    console.error("Dashboard error:", error);
  }, [error]);

  return (
    <div className="flex min-h-[400px] items-center justify-center">
      <Card className="max-w-md p-8">
        <div className="flex flex-col items-center space-y-4 text-center">
          <AlertCircle className="h-12 w-12 text-red-500" />
          <h2 className="text-xl font-semibold">Bir Hata Oluştu</h2>
          <p className="text-muted-foreground text-sm">
            Dashboard verilerini yüklerken bir sorun oluştu. Lütfen tekrar deneyin.
          </p>
          {error.message && (
            <p className="rounded bg-red-50 p-2 text-xs text-red-600">{error.message}</p>
          )}
          <Button onClick={reset} className="mt-4">
            Tekrar Dene
          </Button>
        </div>
      </Card>
    </div>
  );
}
