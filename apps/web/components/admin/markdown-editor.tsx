'use client';

import { useState, useRef } from 'react';
import { Button } from '@/components/ui/button';
import { Image, Bold, Italic, Link as LinkIcon, List, ListOrdered, Code } from 'lucide-react';
import { Label } from '@/components/ui/label';

interface MarkdownEditorProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
}

export function MarkdownEditor({ value, onChange, placeholder }: MarkdownEditorProps) {
  const [uploading, setUploading] = useState(false);
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const insertText = (before: string, after: string = '') => {
    if (!textareaRef.current) return;

    const start = textareaRef.current.selectionStart;
    const end = textareaRef.current.selectionEnd;
    const selectedText = value.substring(start, end);
    const newText = value.substring(0, start) + before + selectedText + after + value.substring(end);

    onChange(newText);

    // Restore cursor position
    setTimeout(() => {
      if (textareaRef.current) {
        textareaRef.current.focus();
        textareaRef.current.setSelectionRange(start + before.length, start + before.length + selectedText.length);
      }
    }, 0);
  };

  const handleImageUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setUploading(true);

    try {
      const formData = new FormData();
      formData.append('file', file);
      formData.append('altText', file.name);

      const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';
      
      // Get token from localStorage
      const authUser = localStorage.getItem('auth_user');
      const token = authUser ? JSON.parse(authUser).token : null;

      const response = await fetch(`${API_URL}/api/NewsArticle/upload-content-image`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
        body: formData,
      });

      if (response.ok) {
        const data = await response.json();
        const imageMarkdown = `\n![${data.altText}](${data.url})\n`;
        
        if (textareaRef.current) {
          const start = textareaRef.current.selectionStart;
          const newText = value.substring(0, start) + imageMarkdown + value.substring(start);
          onChange(newText);
        }

        alert('Resim başarıyla yüklendi!');
      } else {
        alert('Resim yüklenemedi');
      }
    } catch (error) {
      console.error('Upload error:', error);
      alert('Bir hata oluştu');
    } finally {
      setUploading(false);
      if (fileInputRef.current) {
        fileInputRef.current.value = '';
      }
    }
  };

  return (
    <div className="space-y-2">
      <div className="flex items-center gap-1 p-2 bg-slate-100 dark:bg-slate-900 rounded-t-md border border-b-0 border-slate-200 dark:border-slate-800">
        <Button
          type="button"
          variant="ghost"
          size="sm"
          onClick={() => insertText('**', '**')}
          title="Bold"
        >
          <Bold className="w-4 h-4" />
        </Button>
        <Button
          type="button"
          variant="ghost"
          size="sm"
          onClick={() => insertText('*', '*')}
          title="Italic"
        >
          <Italic className="w-4 h-4" />
        </Button>
        <Button
          type="button"
          variant="ghost"
          size="sm"
          onClick={() => insertText('[Link metni](', ')')}
          title="Link"
        >
          <LinkIcon className="w-4 h-4" />
        </Button>
        <Button
          type="button"
          variant="ghost"
          size="sm"
          onClick={() => insertText('\n- ', '')}
          title="Unordered List"
        >
          <List className="w-4 h-4" />
        </Button>
        <Button
          type="button"
          variant="ghost"
          size="sm"
          onClick={() => insertText('\n1. ', '')}
          title="Ordered List"
        >
          <ListOrdered className="w-4 h-4" />
        </Button>
        <Button
          type="button"
          variant="ghost"
          size="sm"
          onClick={() => insertText('`', '`')}
          title="Inline Code"
        >
          <Code className="w-4 h-4" />
        </Button>
        <div className="ml-auto">
          <input
            ref={fileInputRef}
            type="file"
            accept="image/*"
            onChange={handleImageUpload}
            className="hidden"
            disabled={uploading}
          />
          <Button
            type="button"
            variant="ghost"
            size="sm"
            onClick={() => fileInputRef.current?.click()}
            disabled={uploading}
            title="Resim Ekle"
          >
            <Image className="w-4 h-4 mr-1" />
            {uploading ? 'Yükleniyor...' : 'Resim Ekle'}
          </Button>
        </div>
      </div>
      
      <textarea
        ref={textareaRef}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
        className="w-full min-h-[400px] px-3 py-2 rounded-b-md border border-t-0 border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-950 resize-y font-mono text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        required
      />
      
      <div className="text-xs text-slate-500 space-y-1">
        <p><strong>Markdown Kullanımı:</strong></p>
        <ul className="list-disc list-inside space-y-0.5">
          <li>**kalın metin** veya *italik metin*</li>
          <li>[Link metni](https://example.com)</li>
          <li>![Resim açıklaması](resim-url)</li>
          <li>- Liste öğesi veya 1. Numaralı liste</li>
          <li>`kod` veya ```kod bloğu```</li>
        </ul>
      </div>
    </div>
  );
}
