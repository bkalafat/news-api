# 📰 News API - Complete Documentation

**Base URL:** `http://localhost:5000` (Development)  
**API Version:** 1.0  
**Date:** September 27, 2025  
**Architecture:** Clean Architecture with .NET 8, MongoDB  

## 🔐 Authentication

**Type:** JWT Bearer Token  
**Header:** `Authorization: Bearer <token>`  

**Public Endpoints:** No authentication required  
**Protected Endpoints:** Require JWT token for Create, Update, Delete operations

---

## 📋 Endpoints Overview

| Endpoint | Method | Auth | Description |
|----------|---------|------|-------------|
| `/api/news` | GET | Public | Get all news with filtering |
| `/api/news/{id}` | GET | Public | Get news by ID |
| `/api/news/by-url` | GET | Public | Get news by URL slug |
| `/api/news` | POST | Protected | Create new news |
| `/api/news/{id}` | PUT | Protected | Update existing news |
| `/api/news/{id}` | DELETE | Protected | Delete news |
| `/health` | GET | Public | Health check |

---

## 🔍 **1. GET All News**

### **Endpoint:** `GET /api/news`
**Authentication:** Public (No auth required)

**Query Parameters:**
```javascript
{
  "category": "string", // Optional - Filter by category (Technology, World, Business, Science, Health, Entertainment)
  "type": "string"      // Optional - Filter by type (Article, Breaking News, etc.)
}
```

**Request Example:**
```bash
GET /api/news
GET /api/news?category=Technology
GET /api/news?category=Technology&type=Article
```

**Response:** `200 OK`
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "category": "Technology",
    "type": "Article",
    "caption": "BBC News Technology Update",
    "keywords": "tech, innovation, AI",
    "socialTags": "#tech #innovation #AI",
    "summary": "Latest technology developments from around the world",
    "imgPath": "https://ichef.bbci.co.uk/news/800/cpsprodpb/image.jpg",
    "imgAlt": "Technology news image",
    "content": "Full article content here...",
    "subjects": ["Technology", "Innovation"],
    "authors": ["BBC News"],
    "expressDate": "2025-09-27T10:00:00Z",
    "createDate": "2025-09-27T09:30:00Z",
    "updateDate": "2025-09-27T09:30:00Z",
    "priority": 50,
    "isActive": true,
    "url": "bbc-technology-update-2025",
    "viewCount": 0,
    "isSecondPageNews": false
  }
]
```

---

## 🎯 **2. GET News by ID**

### **Endpoint:** `GET /api/news/{id}`
**Authentication:** Public (No auth required)

**Path Parameters:**
- `id` (string, required): News article ID (GUID format)

**Request Example:**
```bash
GET /api/news/550e8400-e29b-41d4-a716-446655440000
```

**Response:** `200 OK`
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "category": "Technology",
  "type": "Article",
  "caption": "BBC News Technology Update",
  "keywords": "tech, innovation, AI",
  "socialTags": "#tech #innovation #AI",
  "summary": "Latest technology developments from around the world",
  "imgPath": "https://ichef.bbci.co.uk/news/800/cpsprodpb/image.jpg",
  "imgAlt": "Technology news image",
  "content": "Full article content here...",
  "subjects": ["Technology", "Innovation"],
  "authors": ["BBC News"],
  "expressDate": "2025-09-27T10:00:00Z",
  "createDate": "2025-09-27T09:30:00Z",
  "updateDate": "2025-09-27T09:30:00Z",
  "priority": 50,
  "isActive": true,
  "url": "bbc-technology-update-2025",
  "viewCount": 0,
  "isSecondPageNews": false
}
```

**Error Response:** `404 Not Found`
```json
{
  "message": "News article not found"
}
```

---

## 🔗 **3. GET News by URL**

### **Endpoint:** `GET /api/news/by-url`
**Authentication:** Public (No auth required)

**Query Parameters:**
- `url` (string, required): News article URL slug

**Request Example:**
```bash
GET /api/news/by-url?url=bbc-technology-update-2025
```

**Response:** Same as GET by ID

---

## ➕ **4. CREATE News**

### **Endpoint:** `POST /api/news`
**Authentication:** Protected (JWT Required)

**Headers:**
```
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "category": "Technology",           // Required, max 100 chars
  "type": "Article",                  // Required, max 50 chars
  "caption": "News Article Title",    // Required, max 500 chars
  "keywords": "tech, news, api",      // Optional, max 1000 chars
  "socialTags": "#tech #news",        // Optional, max 500 chars
  "summary": "Article summary",       // Required, max 2000 chars
  "imgPath": "https://example.com/image.jpg", // Optional, max 500 chars
  "imgAlt": "Alt text for image",     // Optional, max 200 chars
  "content": "Full article content",  // Required
  "subjects": ["Technology", "API"],  // Optional array
  "authors": ["John Doe"],            // Optional array
  "expressDate": "2025-09-27T10:00:00Z", // Required ISO date
  "priority": 50,                     // Optional, 1-100
  "isActive": true,                   // Optional, default true
  "url": "unique-article-slug",       // Optional, max 500 chars
  "isSecondPageNews": false          // Optional, default false
}
```

**Response:** `201 Created`
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "category": "Technology",
  // ... full news object
}
```

**Error Response:** `400 Bad Request`
```json
{
  "errors": {
    "Category": ["The Category field is required."],
    "Caption": ["The Caption field is required."]
  }
}
```

---

## ✏️ **5. UPDATE News**

### **Endpoint:** `PUT /api/news/{id}`
**Authentication:** Protected (JWT Required)

**Headers:**
```
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Path Parameters:**
- `id` (string, required): News article ID

**Request Body:** (All fields optional - only send what you want to update)
```json
{
  "category": "Updated Category",
  "caption": "Updated Title",
  "content": "Updated content",
  "isActive": false
}
```

**Response:** `204 No Content`

**Error Response:** `404 Not Found`
```json
{
  "message": "News article not found"
}
```

---

## 🗑️ **6. DELETE News**

### **Endpoint:** `DELETE /api/news/{id}`
**Authentication:** Protected (JWT Required)

**Headers:**
```
Authorization: Bearer <your_jwt_token>
```

**Path Parameters:**
- `id` (string, required): News article ID

**Response:** `204 No Content`

**Error Response:** `404 Not Found`
```json
{
  "message": "News article not found"
}
```

---

## 🏥 **7. Health Check**

### **Endpoint:** `GET /health`
**Authentication:** Public

**Response:** `200 OK`
```json
"Healthy"
```

---

## 📊 Data Models

### **News Entity (Full Response)**
```javascript
{
  "id": "string",              // GUID
  "category": "string",         // Max 100 chars
  "type": "string",             // Max 50 chars  
  "caption": "string",          // Max 500 chars
  "keywords": "string",         // Max 1000 chars
  "socialTags": "string",       // Max 500 chars
  "summary": "string",          // Max 2000 chars
  "imgPath": "string",          // Max 500 chars (HTTP/HTTPS URL)
  "imgAlt": "string",           // Max 200 chars
  "content": "string",          // Full content
  "subjects": ["string"],       // Array of subjects
  "authors": ["string"],        // Array of authors
  "expressDate": "datetime",    // ISO 8601 format
  "createDate": "datetime",     // Auto-generated
  "updateDate": "datetime",     // Auto-updated
  "priority": "number",         // 1-100
  "isActive": "boolean",        // true/false
  "url": "string",              // URL slug, max 500 chars
  "viewCount": "number",        // Auto-managed
  "isSecondPageNews": "boolean" // true/false
}
```

---

## 🎨 Frontend Integration Guide

### **Common Categories:**
- Technology
- World  
- Business
- Science
- Health
- Entertainment

### **Common Types:**
- Article
- Breaking News
- Opinion
- Analysis

### **Image URLs:**
Real BBC News images are served from RSS feeds:
- Format: `https://ichef.bbci.co.uk/news/{size}/cpsprodpb/{hash}.jpg`
- All images are already optimized and CDN-served

### **Date Handling:**
All dates are in ISO 8601 format (UTC). Convert to local time in frontend.

### **Error Handling:**
All endpoints return consistent error format:
```json
{
  "message": "Error description",
  "error": "Technical details" // Only in development
}
```

### **Pagination:**
Currently not implemented. All news returned in single response.

---

## 🔧 Development Setup

**Base URL:** `http://localhost:5000`  
**MongoDB:** Required (connection configured via user secrets)  
**CORS:** Configured for `http://localhost:3000` (React dev server)

### **Sample Frontend Integration:**

```javascript
// Get all news
const response = await fetch('http://localhost:5000/api/news');
const news = await response.json();

// Get filtered news  
const techNews = await fetch('http://localhost:5000/api/news?category=Technology');

// Get single article
const article = await fetch(`http://localhost:5000/api/news/${articleId}`);

// Error handling
if (!response.ok) {
  const error = await response.json();
  console.error('API Error:', error.message);
}
```

---

## 📝 Notes for Frontend Developer

1. **Real Data**: API serves real BBC News articles via RSS integration
2. **Images**: All image URLs are valid and optimized
3. **Content**: Articles contain real, formatted content
4. **SEO-Friendly**: URL slugs available for SEO-friendly routes
5. **Responsive**: Image URLs work across all device sizes
6. **Performance**: API includes caching (30-minute cache)
7. **Clean Architecture**: Well-structured, maintainable backend

**Questions?** Contact the backend team for any clarifications!