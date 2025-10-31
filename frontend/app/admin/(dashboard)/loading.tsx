import { Card } from "@/components/ui/card";

export default function Loading() {
  return (
    <div className="animate-pulse space-y-6">
      {/* Stats Cards Skeleton */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {[1, 2, 3, 4].map((i) => (
          <Card key={i} className="p-6">
            <div className="space-y-2">
              <div className="h-4 w-24 rounded bg-gray-200" />
              <div className="h-8 w-16 rounded bg-gray-300" />
              <div className="h-3 w-32 rounded bg-gray-200" />
            </div>
          </Card>
        ))}
      </div>

      {/* Recent News Skeleton */}
      <Card className="p-6">
        <div className="mb-4 h-6 w-32 rounded bg-gray-300" />
        <div className="space-y-3">
          {[1, 2, 3, 4, 5].map((i) => (
            <div key={i} className="flex items-center justify-between border-b py-3">
              <div className="flex-1 space-y-2">
                <div className="h-4 w-3/4 rounded bg-gray-200" />
                <div className="h-3 w-1/2 rounded bg-gray-100" />
              </div>
              <div className="ml-4 h-4 w-16 rounded bg-gray-200" />
            </div>
          ))}
        </div>
      </Card>
    </div>
  );
}
