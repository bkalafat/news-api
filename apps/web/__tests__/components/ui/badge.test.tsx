import { describe, it, expect } from 'vitest';
import { render, screen } from '@/__tests__/utils/test-utils';
import { Badge } from '@/components/ui/badge';

describe('Badge Component', () => {
  it('renders badge with text', () => {
    render(<Badge>Badge Text</Badge>);

    const badge = screen.getByText('Badge Text');
    expect(badge).toBeInTheDocument();
  });

  it('renders default variant', () => {
    render(<Badge variant="default">Default</Badge>);

    const badge = screen.getByText('Default');
    expect(badge.className).toContain('bg-');
  });

  it('renders secondary variant', () => {
    render(<Badge variant="secondary">Secondary</Badge>);

    const badge = screen.getByText('Secondary');
    expect(badge.className).toContain('secondary');
  });

  it('renders destructive variant', () => {
    render(<Badge variant="destructive">Destructive</Badge>);

    const badge = screen.getByText('Destructive');
    expect(badge.className).toContain('destructive');
  });

  it('renders outline variant', () => {
    render(<Badge variant="outline">Outline</Badge>);

    const badge = screen.getByText('Outline');
    expect(badge.className).toContain('border');
  });

  it('applies custom className', () => {
    render(<Badge className="custom-badge">Custom</Badge>);

    const badge = screen.getByText('Custom');
    expect(badge.className).toContain('custom-badge');
  });

  it('renders with proper inline-flex styling', () => {
    render(<Badge>Badge</Badge>);

    const badge = screen.getByText('Badge');
    expect(badge.className).toContain('inline-flex');
  });

  it('has rounded styling', () => {
    render(<Badge>Rounded</Badge>);

    const badge = screen.getByText('Rounded');
    expect(badge.className).toContain('rounded');
  });

  it('passes through additional props', () => {
    render(<Badge data-testid="test-badge">Test</Badge>);

    const badge = screen.getByTestId('test-badge');
    expect(badge).toBeInTheDocument();
  });

  it('renders multiple badges together', () => {
    render(
      <div>
        <Badge>Badge 1</Badge>
        <Badge>Badge 2</Badge>
        <Badge>Badge 3</Badge>
      </div>
    );

    expect(screen.getByText('Badge 1')).toBeInTheDocument();
    expect(screen.getByText('Badge 2')).toBeInTheDocument();
    expect(screen.getByText('Badge 3')).toBeInTheDocument();
  });

  it('renders badge with icon', () => {
    render(
      <Badge>
        <span aria-label="icon">✓</span>
        <span>Success</span>
      </Badge>
    );

    expect(screen.getByText('Success')).toBeInTheDocument();
    expect(screen.getByLabelText('icon')).toBeInTheDocument();
  });

  it('handles empty content', () => {
    const { container } = render(<Badge />);

    const badge = container.firstChild;
    expect(badge).toBeInTheDocument();
  });

  it('uses semantic HTML element', () => {
    const { container } = render(<Badge>Badge</Badge>);

    const badge = container.firstChild;
    // Badge should be a div or span
    expect(badge?.nodeName).toMatch(/DIV|SPAN/);
  });
});
