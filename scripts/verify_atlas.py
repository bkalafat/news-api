from pymongo import MongoClient

# Connect to MongoDB Atlas
client = MongoClient('mongodb+srv://bkalafat:dbuserpassword123@cluster0.xwbfl1o.mongodb.net/')
db = client['NewsDb']
collection = db['News']

# Get count
count = collection.count_documents({})
print(f'Documents in MongoDB Atlas: {count}')

# Get sample document
sample = collection.find_one()
if sample:
    print(f'\nSample Document:')
    print(f'  Caption: {sample.get("Caption", "N/A")}')
    print(f'  Category: {sample.get("Category", "N/A")}')
    print(f'  Slug: {sample.get("Slug", "N/A")}')
    
# List some categories
categories = collection.distinct('Category')
print(f'\nCategories found: {categories}')

client.close()
