/**
 * Authentication utilities for admin panel
 * Handles JWT token management and API authentication
 */

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';

export interface LoginCredentials {
  username: string;
  password: string;
}

export interface AuthUser {
  userId: string;
  username: string;
  token: string;
  expiresAt: number;
}

/**
 * Login to admin panel
 */
export async function login(credentials: LoginCredentials): Promise<AuthUser> {
  const response = await fetch(`${API_URL}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(credentials),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: 'Giriş başarısız' }));
    throw new Error(error.message || 'Giriş başarısız');
  }

  const data = await response.json();
  
  const user: AuthUser = {
    userId: data.userId,
    username: data.username,
    token: data.token,
    expiresAt: Date.now() + 60 * 60 * 1000, // 1 hour
  };

  // Store in localStorage
  if (typeof window !== 'undefined') {
    localStorage.setItem('auth_user', JSON.stringify(user));
  }

  return user;
}

/**
 * Logout from admin panel
 */
export function logout(): void {
  if (typeof window !== 'undefined') {
    localStorage.removeItem('auth_user');
    window.location.href = '/admin/login';
  }
}

/**
 * Get current authenticated user
 */
export function getUser(): AuthUser | null {
  if (typeof window === 'undefined') return null;

  const stored = localStorage.getItem('auth_user');
  if (!stored) return null;

  try {
    const user: AuthUser = JSON.parse(stored);
    
    // Check if token expired
    if (Date.now() > user.expiresAt) {
      logout();
      return null;
    }

    return user;
  } catch {
    return null;
  }
}

/**
 * Check if user is authenticated
 */
export function isAuthenticated(): boolean {
  return getUser() !== null;
}

/**
 * Get authorization header for API requests
 */
export function getAuthHeader(): Record<string, string> {
  const user = getUser();
  if (!user) return {};
  
  return {
    'Authorization': `Bearer ${user.token}`,
  };
}

/**
 * Authenticated fetch wrapper
 */
export async function authFetch(url: string, options: RequestInit = {}) {
  const headers = {
    ...options.headers,
    ...getAuthHeader(),
  };

  const response = await fetch(url, { ...options, headers });

  // Handle 401 Unauthorized
  if (response.status === 401) {
    logout();
    throw new Error('Session expired');
  }

  return response;
}
