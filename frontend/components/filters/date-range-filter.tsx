"use client";

import { useState } from "react";
import { Calendar } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";

interface DateRangeFilterProps {
  onFilter: (startDate: Date | null, endDate: Date | null) => void;
  className?: string;
}

export function DateRangeFilter({ onFilter, className }: DateRangeFilterProps) {
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [isActive, setIsActive] = useState(false);

  const handleApply = () => {
    const start = startDate ? new Date(startDate) : null;
    const end = endDate ? new Date(endDate) : null;

    if (start || end) {
      onFilter(start, end);
      setIsActive(true);
    }
  };

  const handleClear = () => {
    setStartDate("");
    setEndDate("");
    onFilter(null, null);
    setIsActive(false);
  };

  return (
    <div className={className}>
      <Popover>
        <PopoverTrigger asChild>
          <Button variant="outline" size="sm" className="gap-2">
            <Calendar className="h-4 w-4" />
            Tarih Filtrele
            {isActive && (
              <Badge variant="secondary" className="ml-1 h-5 px-1">
                ✓
              </Badge>
            )}
          </Button>
        </PopoverTrigger>
        <PopoverContent className="w-80" align="end">
          <div className="space-y-4">
            <h4 className="leading-none font-medium">Tarih Aralığı</h4>

            <div className="space-y-2">
              <Label htmlFor="start-date">Başlangıç Tarihi</Label>
              <Input
                id="start-date"
                type="date"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="end-date">Bitiş Tarihi</Label>
              <Input
                id="end-date"
                type="date"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
                min={startDate}
              />
            </div>

            <div className="flex gap-2">
              <Button
                onClick={handleApply}
                size="sm"
                className="flex-1"
                disabled={!startDate && !endDate}
              >
                Uygula
              </Button>
              <Button onClick={handleClear} size="sm" variant="outline" className="flex-1">
                Temizle
              </Button>
            </div>
          </div>
        </PopoverContent>
      </Popover>
    </div>
  );
}
