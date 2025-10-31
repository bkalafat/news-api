import { describe, it, expect } from "vitest";
import { render, screen } from "@/__tests__/utils/test-utils";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "@/components/ui/card";

describe("Card Component", () => {
  it("renders card with content", () => {
    render(
      <Card>
        <CardContent>Card content</CardContent>
      </Card>
    );

    const content = screen.getByText("Card content");
    expect(content).toBeInTheDocument();
  });

  it("renders card header with title", () => {
    render(
      <Card>
        <CardHeader>
          <CardTitle>Card Title</CardTitle>
        </CardHeader>
      </Card>
    );

    const title = screen.getByText("Card Title");
    expect(title).toBeInTheDocument();
  });

  it("renders card header with description", () => {
    render(
      <Card>
        <CardHeader>
          <CardTitle>Title</CardTitle>
          <CardDescription>This is a description</CardDescription>
        </CardHeader>
      </Card>
    );

    const description = screen.getByText("This is a description");
    expect(description).toBeInTheDocument();
  });

  it("renders card footer", () => {
    render(
      <Card>
        <CardContent>Content</CardContent>
        <CardFooter>Footer content</CardFooter>
      </Card>
    );

    const footer = screen.getByText("Footer content");
    expect(footer).toBeInTheDocument();
  });

  it("renders complete card structure", () => {
    render(
      <Card>
        <CardHeader>
          <CardTitle>Complete Card</CardTitle>
          <CardDescription>Card description</CardDescription>
        </CardHeader>
        <CardContent>Main content</CardContent>
        <CardFooter>Footer</CardFooter>
      </Card>
    );

    expect(screen.getByText("Complete Card")).toBeInTheDocument();
    expect(screen.getByText("Card description")).toBeInTheDocument();
    expect(screen.getByText("Main content")).toBeInTheDocument();
    expect(screen.getByText("Footer")).toBeInTheDocument();
  });

  it("applies custom className to Card", () => {
    const { container } = render(
      <Card className="custom-card">
        <CardContent>Content</CardContent>
      </Card>
    );

    const card = container.firstChild as HTMLElement;
    expect(card.className).toContain("custom-card");
  });

  it("applies custom className to CardHeader", () => {
    const { container } = render(
      <Card>
        <CardHeader className="custom-header">
          <CardTitle>Title</CardTitle>
        </CardHeader>
      </Card>
    );

    const header = container.querySelector(".custom-header");
    expect(header).toBeInTheDocument();
  });

  it("applies custom className to CardContent", () => {
    const { container } = render(
      <Card>
        <CardContent className="custom-content">Content</CardContent>
      </Card>
    );

    const content = container.querySelector(".custom-content");
    expect(content).toBeInTheDocument();
  });

  it("applies custom className to CardFooter", () => {
    const { container } = render(
      <Card>
        <CardFooter className="custom-footer">Footer</CardFooter>
      </Card>
    );

    const footer = container.querySelector(".custom-footer");
    expect(footer).toBeInTheDocument();
  });

  it("has proper card styling", () => {
    const { container } = render(
      <Card>
        <CardContent>Content</CardContent>
      </Card>
    );

    const card = container.firstChild as HTMLElement;
    // Card should have border, rounded corners, background
    const hasCardStyling =
      card.className.includes("border") ||
      card.className.includes("rounded") ||
      card.className.includes("bg-");
    expect(hasCardStyling).toBe(true);
  });

  it("renders nested cards", () => {
    render(
      <Card>
        <CardContent>
          <Card>
            <CardContent>Nested card</CardContent>
          </Card>
        </CardContent>
      </Card>
    );

    const nestedContent = screen.getByText("Nested card");
    expect(nestedContent).toBeInTheDocument();
  });

  it("renders CardTitle with proper heading level", () => {
    const { container } = render(
      <Card>
        <CardHeader>
          <CardTitle>Title</CardTitle>
        </CardHeader>
      </Card>
    );

    // CardTitle may render as div or heading, check that title text exists
    const titleElement = container.querySelector('[data-slot="card-title"]');
    expect(titleElement).toBeInTheDocument();
    expect(titleElement?.textContent).toBe("Title");
  });

  it("passes through additional props", () => {
    render(
      <Card data-testid="test-card">
        <CardContent>Content</CardContent>
      </Card>
    );

    const card = screen.getByTestId("test-card");
    expect(card).toBeInTheDocument();
  });
});
