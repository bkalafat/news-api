#!/usr/bin/env python3
"""
Migrate local MongoDB data to MongoDB Atlas
"""
import json
import sys
from pymongo import MongoClient
from datetime import datetime
from bson import ObjectId

# MongoDB Atlas connection
ATLAS_URI = "mongodb+srv://bkalafat:dbuserpassword123@cluster0.xwbfl1o.mongodb.net/"
DATABASE_NAME = "NewsDb"
COLLECTION_NAME = "News"

def convert_extended_json(doc):
    """
    Convert MongoDB extended JSON ($oid, $date) to standard BSON objects
    """
    if isinstance(doc, dict):
        new_doc = {}
        for key, value in doc.items():
            if key == '_id' and isinstance(value, dict) and '$oid' in value:
                # Convert $oid to ObjectId
                new_doc[key] = ObjectId(value['$oid'])
            elif isinstance(value, dict) and '$date' in value:
                # Convert $date to datetime
                date_str = value['$date']
                new_doc[key] = datetime.fromisoformat(date_str.replace('Z', '+00:00'))
            elif isinstance(value, dict):
                new_doc[key] = convert_extended_json(value)
            elif isinstance(value, list):
                new_doc[key] = [convert_extended_json(item) if isinstance(item, dict) else item for item in value]
            else:
                new_doc[key] = value
        return new_doc
    return doc

def migrate_data(json_file_path):
    """
    Read JSON export and import to MongoDB Atlas
    """
    print(f"Reading data from {json_file_path}...")
    
    # Read the JSON file
    documents = []
    with open(json_file_path, 'r', encoding='utf-8') as f:
        for line in f:
            doc = json.loads(line.strip())
            
            # Convert MongoDB extended JSON to standard format
            doc = convert_extended_json(doc)
            documents.append(doc)
    
    print(f"Found {len(documents)} documents to import")
    
    # Connect to MongoDB Atlas
    print("Connecting to MongoDB Atlas...")
    client = MongoClient(ATLAS_URI)
    db = client[DATABASE_NAME]
    collection = db[COLLECTION_NAME]
    
    # Check if collection already has data
    existing_count = collection.count_documents({})
    print(f"Existing documents in Atlas: {existing_count}")
    
    if existing_count > 0:
        response = input(f"Collection already has {existing_count} documents. Do you want to:\n"
                        f"1. Delete existing and import new data\n"
                        f"2. Append to existing data\n"
                        f"3. Cancel\n"
                        f"Enter choice (1/2/3): ")
        
        if response == '1':
            print("Deleting existing documents...")
            result = collection.delete_many({})
            print(f"Deleted {result.deleted_count} documents")
        elif response == '3':
            print("Migration cancelled")
            return
    
    # Insert documents
    print("Importing documents to Atlas...")
    try:
        if documents:
            result = collection.insert_many(documents, ordered=False)
            print(f"✅ Successfully imported {len(result.inserted_ids)} documents")
        else:
            print("⚠️ No documents to import")
    except Exception as e:
        print(f"❌ Error during import: {e}")
        sys.exit(1)
    
    # Verify
    final_count = collection.count_documents({})
    print(f"✅ Total documents in Atlas: {final_count}")
    
    # Show sample document
    sample = collection.find_one()
    if sample:
        print("\nSample document:")
        print(f"  Title: {sample.get('Title', 'N/A')}")
        print(f"  Slug: {sample.get('Slug', 'N/A')}")
        print(f"  Category: {sample.get('Category', 'N/A')}")
    
    client.close()
    print("\n✅ Migration completed successfully!")

if __name__ == "__main__":
    json_file = r"C:\dev\newsportal\news_export.json"
    migrate_data(json_file)
