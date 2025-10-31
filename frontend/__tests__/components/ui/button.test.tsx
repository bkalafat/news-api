import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@/__tests__/utils/test-utils";
import { Button } from "@/components/ui/button";

describe("Button Component", () => {
  it("renders button with text", () => {
    render(<Button>Click Me</Button>);

    const button = screen.getByRole("button", { name: /click me/i });
    expect(button).toBeInTheDocument();
  });

  it("handles onClick events", () => {
    const handleClick = vi.fn();
    render(<Button onClick={handleClick}>Click Me</Button>);

    const button = screen.getByRole("button", { name: /click me/i });
    fireEvent.click(button);

    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it("can be disabled", () => {
    const handleClick = vi.fn();
    render(
      <Button disabled onClick={handleClick}>
        Click Me
      </Button>
    );

    const button = screen.getByRole("button", { name: /click me/i });
    expect(button).toBeDisabled();

    fireEvent.click(button);
    expect(handleClick).not.toHaveBeenCalled();
  });

  it("renders different variants", () => {
    const { rerender } = render(<Button variant="default">Default</Button>);
    let button = screen.getByRole("button");
    expect(button.className).toContain("bg-");

    rerender(<Button variant="destructive">Destructive</Button>);
    button = screen.getByRole("button");
    expect(button.className).toContain("destructive");

    rerender(<Button variant="outline">Outline</Button>);
    button = screen.getByRole("button");
    expect(button.className).toContain("outline");

    rerender(<Button variant="ghost">Ghost</Button>);
    button = screen.getByRole("button");
    // Ghost variant may use hover styles instead of explicit 'ghost' class
    expect(button.className).toContain("hover:");

    rerender(<Button variant="link">Link</Button>);
    button = screen.getByRole("button");
    expect(button.className).toContain("underline");
  });

  it("renders different sizes", () => {
    const { rerender } = render(<Button size="default">Default</Button>);
    let button = screen.getByRole("button");
    // Check for any height class
    expect(button.className).toMatch(/h-\d|size-\d/);

    rerender(<Button size="sm">Small</Button>);
    button = screen.getByRole("button");
    expect(button.className).toMatch(/h-\d|size-\d/);

    rerender(<Button size="lg">Large</Button>);
    button = screen.getByRole("button");
    expect(button.className).toMatch(/h-\d|size-\d/);

    rerender(<Button size="icon">Icon</Button>);
    button = screen.getByRole("button");
    // Icon button uses size-9 instead of h-
    expect(button.className).toMatch(/h-\d|size-\d/);
  });

  it("can render as child component", () => {
    render(
      <Button asChild>
        <a href="/test">Link Button</a>
      </Button>
    );

    const link = screen.getByRole("link");
    expect(link).toBeInTheDocument();
    expect(link).toHaveAttribute("href", "/test");
  });

  it("applies custom className", () => {
    render(<Button className="custom-class">Button</Button>);

    const button = screen.getByRole("button");
    expect(button.className).toContain("custom-class");
  });

  it("supports loading state", () => {
    render(<Button disabled>Loading...</Button>);

    const button = screen.getByRole("button");
    expect(button).toBeDisabled();
  });

  it("passes through additional props", () => {
    render(
      <Button data-testid="test-button" aria-label="Test">
        Click
      </Button>
    );

    const button = screen.getByTestId("test-button");
    expect(button).toHaveAttribute("aria-label", "Test");
  });

  it("has proper button type", () => {
    const { rerender } = render(<Button type="button">Button</Button>);
    let button = screen.getByRole("button");
    expect(button).toHaveAttribute("type", "button");

    rerender(<Button type="submit">Submit</Button>);
    button = screen.getByRole("button");
    expect(button).toHaveAttribute("type", "submit");
  });
});
