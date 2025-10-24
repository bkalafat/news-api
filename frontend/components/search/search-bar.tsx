'use client';

import { useState, useEffect, useCallback } from 'react';
import { Search, X } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { useRouter } from 'next/navigation';

interface SearchBarProps {
  className?: string;
}

export function SearchBar({ className }: SearchBarProps) {
  const [query, setQuery] = useState('');
  const [isOpen, setIsOpen] = useState(false);
  const router = useRouter();

  // Debounce search
  useEffect(() => {
    if (!query) return;

    const timer = setTimeout(() => {
      // Navigate to search results
      router.push(`/search?q=${encodeURIComponent(query)}`);
    }, 500);

    return () => clearTimeout(timer);
  }, [query, router]);

  const handleSearch = useCallback((e: React.FormEvent) => {
    e.preventDefault();
    if (query.trim()) {
      router.push(`/search?q=${encodeURIComponent(query.trim())}`);
      setIsOpen(false);
    }
  }, [query, router]);

  const handleClear = useCallback(() => {
    setQuery('');
  }, []);

  return (
    <div className={className}>
      {/* Desktop Search */}
      <form onSubmit={handleSearch} className="hidden md:flex relative">
        <Input
          type="search"
          placeholder="Haber ara..."
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          className="w-64 pr-10"
        />
        {query && (
          <Button
            type="button"
            variant="ghost"
            size="icon"
            className="absolute right-0 top-0 h-full"
            onClick={handleClear}
          >
            <X className="h-4 w-4" />
          </Button>
        )}
      </form>

      {/* Mobile Search Button */}
      <Button
        variant="ghost"
        size="icon"
        className="md:hidden"
        onClick={() => setIsOpen(!isOpen)}
      >
        <Search className="h-5 w-5" />
      </Button>

      {/* Mobile Search Overlay */}
      {isOpen && (
        <div className="fixed inset-0 z-50 bg-background md:hidden">
          <div className="container mx-auto px-4 py-4">
            <form onSubmit={handleSearch} className="flex gap-2">
              <Input
                type="search"
                placeholder="Haber ara..."
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                autoFocus
                className="flex-1"
              />
              <Button type="button" variant="ghost" onClick={() => setIsOpen(false)}>
                <X className="h-5 w-5" />
              </Button>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
