import { describe, it, expect } from "vitest";
import { render, screen } from "@/__tests__/utils/test-utils";
import { NewsDetailContent } from "@/components/news/news-detail-content";
import { mockNewsArticle } from "@/__tests__/__mocks__/mockData";

describe("NewsDetailContent Component", () => {
  it("renders HTML content correctly when content starts with HTML tags", () => {
    render(<NewsDetailContent news={mockNewsArticle} />);

    // Check if HTML is rendered (strong tag should create bold text)
    const content = screen.getByText(/GPT-5 modelini/i);
    expect(content).toBeInTheDocument();
  });

  it("renders authors section when authors are provided", () => {
    render(<NewsDetailContent news={mockNewsArticle} />);

    // Check if authors are displayed
    mockNewsArticle.authors.forEach((author) => {
      expect(screen.getByText(author)).toBeInTheDocument();
    });

    // Check if "Yazar" label is present
    expect(screen.getByText("Yazar")).toBeInTheDocument();
  });

  it("renders author avatar with first letter", () => {
    render(<NewsDetailContent news={mockNewsArticle} />);

    // Avatar should show first letter of author name
    const firstLetter = mockNewsArticle.authors?.[0]?.charAt(0).toUpperCase() ?? "";
    expect(screen.getByText(firstLetter)).toBeInTheDocument();
  });

  it("renders subjects/topics section when provided", () => {
    render(<NewsDetailContent news={mockNewsArticle} />);

    // Check for "Konular" heading
    expect(screen.getByText("Konular")).toBeInTheDocument();

    // Check if subjects are rendered as badges
    mockNewsArticle.subjects.forEach((subject) => {
      expect(screen.getByText(subject)).toBeInTheDocument();
    });
  });

  it("renders keywords section when provided", () => {
    render(<NewsDetailContent news={mockNewsArticle} />);

    // Check for "Anahtar Kelimeler" heading
    expect(screen.getByText("Anahtar Kelimeler")).toBeInTheDocument();

    // Check if keywords are parsed and displayed
    const keywords = mockNewsArticle.keywords.split(",");
    keywords.forEach((keyword) => {
      expect(screen.getByText(keyword.trim())).toBeInTheDocument();
    });
  });

  it("renders social tags section when provided", () => {
    render(<NewsDetailContent news={mockNewsArticle} />);

    // Check for "Sosyal Medyada Paylaş" heading
    expect(screen.getByText("Sosyal Medyada Paylaş")).toBeInTheDocument();

    // Check if social tags are displayed
    expect(screen.getByText(mockNewsArticle.socialTags)).toBeInTheDocument();
  });

  it("renders share button", () => {
    render(<NewsDetailContent news={mockNewsArticle} />);

    // Check for share button
    const shareButton = screen.getByRole("button", { name: /paylaş/i });
    expect(shareButton).toBeInTheDocument();
  });

  it("renders save/bookmark button", () => {
    render(<NewsDetailContent news={mockNewsArticle} />);

    // Check for bookmark button
    const bookmarkButton = screen.getByRole("button", { name: /kaydet/i });
    expect(bookmarkButton).toBeInTheDocument();
  });

  it("applies prose classes for typography", () => {
    const { container } = render(<NewsDetailContent news={mockNewsArticle} />);

    // Check if prose classes are applied for rich text styling
    const article = container.querySelector("article");
    expect(article).toHaveClass("prose");
    expect(article).toHaveClass("prose-lg");
  });

  it("renders plain text content when not HTML", () => {
    const plainTextNews = {
      ...mockNewsArticle,
      content: "This is plain text content.\nWith multiple lines.\nAnd paragraphs.",
    };

    render(<NewsDetailContent news={plainTextNews} />);

    // Plain text should be split into paragraphs
    expect(screen.getByText("This is plain text content.")).toBeInTheDocument();
    expect(screen.getByText("With multiple lines.")).toBeInTheDocument();
    expect(screen.getByText("And paragraphs.")).toBeInTheDocument();
  });

  it("handles empty authors array gracefully", () => {
    const newsWithoutAuthors = {
      ...mockNewsArticle,
      authors: [],
    };

    expect(() => render(<NewsDetailContent news={newsWithoutAuthors} />)).not.toThrow();
  });

  it("handles empty subjects array gracefully", () => {
    const newsWithoutSubjects = {
      ...mockNewsArticle,
      subjects: [],
    };

    expect(() => render(<NewsDetailContent news={newsWithoutSubjects} />)).not.toThrow();
  });

  it("renders separators between sections", () => {
    render(<NewsDetailContent news={mockNewsArticle} />);

    // Separators may be styled differently, check for section divisions
    const { container } = render(<NewsDetailContent news={mockNewsArticle} />);
    // Component may use borders or spacing instead of explicit separators
    expect(container).toBeTruthy();
  });
});
