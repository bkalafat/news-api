import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";

export function NewsCardSkeleton() {
  return (
    <Card className="overflow-hidden">
      {/* Image Skeleton */}
      <Skeleton className="h-48 w-full" />

      <CardHeader className="space-y-2">
        {/* Badge Skeleton */}
        <Skeleton className="h-5 w-20" />

        {/* Title Skeleton */}
        <Skeleton className="h-6 w-full" />
        <Skeleton className="h-6 w-4/5" />

        {/* Meta Skeleton */}
        <div className="flex items-center gap-4">
          <Skeleton className="h-4 w-24" />
          <Skeleton className="h-4 w-16" />
        </div>
      </CardHeader>

      <CardContent>
        {/* Description Skeleton */}
        <Skeleton className="mb-2 h-4 w-full" />
        <Skeleton className="mb-2 h-4 w-full" />
        <Skeleton className="h-4 w-3/4" />

        {/* Link Skeleton */}
        <Skeleton className="mt-4 h-4 w-24" />
      </CardContent>
    </Card>
  );
}
