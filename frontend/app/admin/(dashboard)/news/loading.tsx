import { Card } from "@/components/ui/card";

export default function Loading() {
  return (
    <div className="animate-pulse space-y-6">
      {/* Header Skeleton */}
      <div className="flex items-center justify-between">
        <div className="h-8 w-48 rounded bg-gray-300" />
        <div className="h-10 w-32 rounded bg-gray-200" />
      </div>

      {/* Search and Filters Skeleton */}
      <div className="flex gap-4">
        <div className="h-10 flex-1 rounded bg-gray-200" />
        <div className="h-10 w-40 rounded bg-gray-200" />
        <div className="h-10 w-32 rounded bg-gray-200" />
      </div>

      {/* Table Skeleton */}
      <Card>
        <div className="space-y-4 p-6">
          {[1, 2, 3, 4, 5, 6, 7, 8].map((i) => (
            <div key={i} className="flex items-center justify-between border-b py-3">
              <div className="flex-1 space-y-2">
                <div className="h-4 w-2/3 rounded bg-gray-200" />
                <div className="h-3 w-1/3 rounded bg-gray-100" />
              </div>
              <div className="flex gap-2">
                <div className="h-8 w-20 rounded bg-gray-200" />
                <div className="h-8 w-8 rounded bg-gray-200" />
                <div className="h-8 w-8 rounded bg-gray-200" />
              </div>
            </div>
          ))}
        </div>
      </Card>
    </div>
  );
}
