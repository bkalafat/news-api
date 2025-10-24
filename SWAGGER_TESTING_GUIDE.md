# News API Swagger UI Testing Guide

## 🚀 **Swagger UI Successfully Configured!**

Your News API now has a comprehensive Swagger UI interface available at:
- **HTTP**: http://localhost:5000/swagger
- **HTTPS**: https://localhost:5001/swagger

## 📋 **Testing the Swagger UI**

### **1. Access Swagger UI**
Open your browser and navigate to: `http://localhost:5000/swagger`

You should see:
- ✅ **Enhanced Documentation**: "News API v1" with detailed descriptions
- ✅ **Try It Out**: Enabled by default for easy testing
- ✅ **Request Duration**: Shows response times
- ✅ **Expanded Models**: Better schema visibility

### **2. Available Endpoints**

#### **📰 GET /api/news** (Public)
- **Description**: Get all news articles with optional filtering
- **Query Parameters**: 
  - `category` (optional): Filter by category (Technology, World, Business, etc.)
  - `type` (optional): Filter by type (Article, Breaking News, Feature, etc.)
- **Test Examples**:
  ```
  GET /api/news
  GET /api/news?category=Technology
  GET /api/news?type=Breaking%20News
  ```

#### **🔍 GET /api/news/{id}** (Public)  
- **Description**: Get a specific news article by ID
- **Path Parameter**: `id` (UUID)
- **Test Example**: Use any ID from the GET /api/news response

#### **🌐 GET /api/news/url/{url}** (Public)
- **Description**: Get news article by URL slug
- **Path Parameter**: `url` (string)
- **Test Example**: Use a URL slug from any article

#### **➕ POST /api/news** (Requires Auth)
- **Description**: Create a new news article
- **Request Body**: CreateNewsDto with all required fields
- **Test Data Example**:
  ```json
  {
    "category": "Technology",
    "type": "Article", 
    "caption": "Test Article from Swagger",
    "keywords": "test, swagger, api",
    "socialTags": "#test #swagger",
    "summary": "This is a test article created via Swagger UI",
    "imgPath": "https://via.placeholder.com/800x400/0066CC/FFFFFF?text=Test+Article",
    "imgAlt": "Test article image",
    "content": "This is the full content of our test article created through the Swagger interface.",
    "expressDate": "2025-01-15T10:30:00.000Z",
    "priority": 50,
    "url": "test-article-from-swagger",
    "subjects": ["Technology", "Testing"],
    "authors": ["Swagger User"]
  }
  ```

#### **✏️ PUT /api/news/{id}** (Requires Auth)
- **Description**: Update an existing news article
- **Path Parameter**: `id` (UUID)
- **Request Body**: UpdateNewsDto

#### **🗑️ DELETE /api/news/{id}** (Requires Auth)
- **Description**: Delete a news article
- **Path Parameter**: `id` (UUID)

### **3. Real Data Testing**

Since your API has RSS integration, you should see real BBC News articles with:
- ✅ **Real Images**: URLs like `https://ichef.bbci.co.uk/news/976/cpsprodpb/xxxx/production/_xxxxx.jpg`
- ✅ **Live Content**: Fresh articles from BBC RSS feeds
- ✅ **Multiple Categories**: Technology, World, Business, Science, Health, Entertainment

### **4. Schema Documentation**

Swagger UI displays detailed schemas for:
- **News Entity**: Complete news article structure
- **CreateNewsDto**: Input model for creating articles  
- **UpdateNewsDto**: Input model for updating articles

### **5. Testing Authentication Endpoints**

For protected endpoints (POST, PUT, DELETE), you'll need JWT authentication:
1. First obtain a JWT token (if you have auth endpoints)
2. Click the "Authorize" button in Swagger UI
3. Enter: `Bearer YOUR_JWT_TOKEN`

### **6. Response Examples**

#### **Successful Response (200 OK)**
```json
[
  {
    "id": "12345678-1234-5678-9012-123456789012",
    "category": "Technology",
    "type": "Article",
    "caption": "AI breakthrough promises revolutionary changes",
    "keywords": "artificial, intelligence, breakthrough, technology",
    "socialTags": "#ai #technology #breakthrough",
    "summary": "Researchers announce major AI advancement...",
    "imgPath": "https://ichef.bbci.co.uk/news/976/cpsprodpb/1234/production/_123456.jpg",
    "imgAlt": "AI technology illustration", 
    "content": "Full article content here...",
    "subjects": ["Technology", "AI"],
    "authors": ["BBC News"],
    "expressDate": "2025-01-15T10:30:00Z",
    "createDate": "2025-01-15T10:30:00Z",
    "updateDate": "2025-01-15T10:30:00Z",
    "priority": 25,
    "isActive": true,
    "url": "ai-breakthrough-promises-revolutionary-changes",
    "viewCount": 0,
    "isSecondPageNews": false
  }
]
```

## 🎯 **Quick Test Steps**

1. **Open Swagger**: Navigate to http://localhost:5000/swagger
2. **Test GET /api/news**: Click "Try it out" → Execute → See real BBC articles
3. **Filter by Category**: Add `?category=Technology` parameter
4. **Test Individual Article**: Copy an ID and test GET /api/news/{id}
5. **Examine Schemas**: Scroll down to see detailed model documentation

## 🔧 **Swagger Configuration Features**

Your enhanced Swagger setup includes:
- ✅ **Custom API Information**: Title, description, contact details
- ✅ **XML Documentation**: Enhanced descriptions (when XML comments are added)
- ✅ **Try It Out Enabled**: Default interactive testing
- ✅ **Request Duration Display**: Performance visibility
- ✅ **Expanded Models**: Better schema understanding

## 🎉 **Results**

Your News API Swagger UI is now fully functional and provides:
- **Interactive Testing**: Test all endpoints directly in the browser
- **Real Data**: See actual BBC news articles with real images
- **Complete Documentation**: Detailed API specifications
- **Schema Visibility**: Clear input/output models
- **Authentication Ready**: JWT support for protected endpoints

Happy API testing! 🚀