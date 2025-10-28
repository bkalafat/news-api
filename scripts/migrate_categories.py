"""
MongoDB Category Migration Script
Converts Turkish categories to English for backend compatibility
"""

from pymongo import MongoClient
from datetime import datetime

# MongoDB Atlas connection
ATLAS_URI = "mongodb+srv://bkalafat:dbuserpassword123@cluster0.xwbfl1o.mongodb.net/"
DATABASE_NAME = "NewsDb"
COLLECTION_NAME = "News"

# Category mappings (Turkish → English)
CATEGORY_MAPPINGS = {
    # Technology group
    'Teknoloji': 'technology',
    'Programlama': 'technology',
    'Mobil': 'technology',
    'Yapay Zeka': 'technology',
    'Yazılım': 'technology',
    
    # Science
    'Robotik': 'science',
    
    # Business
    'Türkiye Teknoloji': 'business',
    
    # World
    'Türkiye': 'world',
}

def migrate_categories():
    """
    Update all news articles with English category names
    """
    print("🔗 Connecting to MongoDB Atlas...")
    client = MongoClient(ATLAS_URI)
    db = client[DATABASE_NAME]
    collection = db[COLLECTION_NAME]
    
    # Get current category distribution
    print("\n📊 Current category distribution:")
    pipeline = [
        {"$group": {"_id": "$Category", "count": {"$sum": 1}}},
        {"$sort": {"count": -1}}
    ]
    categories = list(collection.aggregate(pipeline))
    for cat in categories:
        print(f"  {cat['_id']}: {cat['count']} articles")
    
    print("\n" + "="*50)
    print("🔄 Starting category migration...")
    print("="*50 + "\n")
    
    total_updated = 0
    
    for turkish_cat, english_cat in CATEGORY_MAPPINGS.items():
        # Count documents with this category
        count = collection.count_documents({"Category": turkish_cat})
        
        if count > 0:
            print(f"📝 Updating '{turkish_cat}' → '{english_cat}' ({count} articles)...")
            
            result = collection.update_many(
                {"Category": turkish_cat},
                {"$set": {
                    "Category": english_cat,
                    "UpdateDate": datetime.utcnow()
                }}
            )
            
            print(f"   ✅ Updated {result.modified_count} articles")
            total_updated += result.modified_count
        else:
            print(f"⏭️  No articles found for '{turkish_cat}'")
    
    print("\n" + "="*50)
    print("📊 Migration Results")
    print("="*50 + "\n")
    
    # Get new category distribution
    print("New category distribution:")
    categories = list(collection.aggregate(pipeline))
    for cat in categories:
        print(f"  {cat['_id']}: {cat['count']} articles")
    
    print(f"\n✅ Migration complete! Total articles updated: {total_updated}")
    
    # Verify
    total_articles = collection.count_documents({})
    print(f"📈 Total articles in database: {total_articles}")
    
    # Show sample documents
    print("\n📄 Sample migrated articles:")
    samples = collection.find().limit(3)
    for i, doc in enumerate(samples, 1):
        print(f"\n{i}. {doc.get('Caption', 'N/A')}")
        print(f"   Category: {doc.get('Category', 'N/A')}")
        print(f"   Slug: {doc.get('Slug', 'N/A')}")
    
    client.close()
    print("\n🎉 Migration completed successfully!")

if __name__ == "__main__":
    try:
        migrate_categories()
    except Exception as e:
        print(f"\n❌ Error during migration: {e}")
        import traceback
        traceback.print_exc()
