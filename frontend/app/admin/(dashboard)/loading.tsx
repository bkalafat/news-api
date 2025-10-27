import { Card } from "@/components/ui/card";

export default function Loading() {
  return (
    <div className="space-y-6 animate-pulse">
      {/* Stats Cards Skeleton */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {[1, 2, 3, 4].map((i) => (
          <Card key={i} className="p-6">
            <div className="space-y-2">
              <div className="h-4 w-24 bg-gray-200 rounded" />
              <div className="h-8 w-16 bg-gray-300 rounded" />
              <div className="h-3 w-32 bg-gray-200 rounded" />
            </div>
          </Card>
        ))}
      </div>

      {/* Recent News Skeleton */}
      <Card className="p-6">
        <div className="h-6 w-32 bg-gray-300 rounded mb-4" />
        <div className="space-y-3">
          {[1, 2, 3, 4, 5].map((i) => (
            <div key={i} className="flex items-center justify-between py-3 border-b">
              <div className="space-y-2 flex-1">
                <div className="h-4 w-3/4 bg-gray-200 rounded" />
                <div className="h-3 w-1/2 bg-gray-100 rounded" />
              </div>
              <div className="h-4 w-16 bg-gray-200 rounded ml-4" />
            </div>
          ))}
        </div>
      </Card>
    </div>
  );
}
