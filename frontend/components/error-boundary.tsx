"use client";

import React, { Component, ReactNode } from "react";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { AlertCircle, RefreshCcw } from "lucide-react";

interface Props {
  children: ReactNode;
  fallback?: ReactNode;
}

interface State {
  hasError: boolean;
  error?: Error;
}

export class ErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error };
  }

  override componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    // Log error to console or error reporting service
    console.error("ErrorBoundary caught an error:", error, errorInfo);
  }

  handleReset = () => {
    this.setState({ hasError: false, error: undefined });
  };

  override render() {
    if (this.state.hasError) {
      if (this.props.fallback) {
        return this.props.fallback;
      }

      return (
        <div className="container mx-auto px-4 py-8">
          <Alert variant="destructive" className="mx-auto max-w-2xl">
            <AlertCircle className="h-4 w-4" />
            <AlertTitle>Bir Hata Oluştu</AlertTitle>
            <AlertDescription className="space-y-4">
              <p>Üzgünüz, beklenmeyen bir hata oluştu. Lütfen sayfayı yenilemeyi deneyin.</p>
              {process.env.NODE_ENV === "development" && this.state.error && (
                <div className="bg-muted mt-4 rounded-md p-4">
                  <p className="text-destructive font-mono text-sm">{this.state.error.message}</p>
                  <pre className="mt-2 max-h-48 overflow-auto text-xs">
                    {this.state.error.stack}
                  </pre>
                </div>
              )}
              <div className="flex gap-2">
                <Button onClick={this.handleReset} variant="outline" size="sm">
                  <RefreshCcw className="mr-2 h-4 w-4" />
                  Tekrar Dene
                </Button>
                <Button onClick={() => window.location.reload()} variant="default" size="sm">
                  Sayfayı Yenile
                </Button>
              </div>
            </AlertDescription>
          </Alert>
        </div>
      );
    }

    return this.props.children;
  }
}
