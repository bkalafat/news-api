'use client';

import { useEffect, useRef } from 'react';
import dynamic from 'next/dynamic';
import 'react-quill/dist/quill.snow.css';

// Dynamic import to avoid SSR issues
const ReactQuill = dynamic(() => import('react-quill'), { ssr: false });

interface RichTextEditorProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
}

export function RichTextEditor({ value, onChange, placeholder }: RichTextEditorProps) {
  const modules = {
    toolbar: [
      [{ header: [1, 2, 3, false] }],
      ['bold', 'italic', 'underline', 'strike'],
      [{ list: 'ordered' }, { list: 'bullet' }],
      [{ indent: '-1' }, { indent: '+1' }],
      ['blockquote', 'code-block'],
      ['link', 'image'],
      [{ color: [] }, { background: [] }],
      [{ align: [] }],
      ['clean'],
    ],
  };

  const formats = [
    'header',
    'bold',
    'italic',
    'underline',
    'strike',
    'list',
    'bullet',
    'indent',
    'blockquote',
    'code-block',
    'link',
    'image',
    'color',
    'background',
    'align',
  ];

  return (
    <div className="rich-text-editor">
      <ReactQuill
        theme="snow"
        value={value}
        onChange={onChange}
        modules={modules}
        formats={formats}
        placeholder={placeholder || 'Haber içeriğini yazın...'}
        className="bg-white dark:bg-slate-950"
      />
      <style jsx global>{`
        .rich-text-editor .ql-container {
          min-height: 400px;
          font-size: 16px;
          font-family: inherit;
        }
        .rich-text-editor .ql-editor {
          min-height: 400px;
        }
        .rich-text-editor .ql-toolbar {
          border-radius: 0.5rem 0.5rem 0 0;
          background: hsl(var(--background));
          border-color: hsl(var(--border));
        }
        .rich-text-editor .ql-container {
          border-radius: 0 0 0.5rem 0.5rem;
          background: hsl(var(--background));
          border-color: hsl(var(--border));
        }
        .rich-text-editor .ql-editor.ql-blank::before {
          color: hsl(var(--muted-foreground));
          font-style: italic;
        }
      `}</style>
    </div>
  );
}
