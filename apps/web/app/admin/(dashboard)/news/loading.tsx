import { Card } from "@/components/ui/card";

export default function Loading() {
  return (
    <div className="space-y-6 animate-pulse">
      {/* Header Skeleton */}
      <div className="flex justify-between items-center">
        <div className="h-8 w-48 bg-gray-300 rounded" />
        <div className="h-10 w-32 bg-gray-200 rounded" />
      </div>

      {/* Search and Filters Skeleton */}
      <div className="flex gap-4">
        <div className="h-10 flex-1 bg-gray-200 rounded" />
        <div className="h-10 w-40 bg-gray-200 rounded" />
        <div className="h-10 w-32 bg-gray-200 rounded" />
      </div>

      {/* Table Skeleton */}
      <Card>
        <div className="p-6 space-y-4">
          {[1, 2, 3, 4, 5, 6, 7, 8].map((i) => (
            <div key={i} className="flex items-center justify-between py-3 border-b">
              <div className="space-y-2 flex-1">
                <div className="h-4 w-2/3 bg-gray-200 rounded" />
                <div className="h-3 w-1/3 bg-gray-100 rounded" />
              </div>
              <div className="flex gap-2">
                <div className="h-8 w-20 bg-gray-200 rounded" />
                <div className="h-8 w-8 bg-gray-200 rounded" />
                <div className="h-8 w-8 bg-gray-200 rounded" />
              </div>
            </div>
          ))}
        </div>
      </Card>
    </div>
  );
}
