# MongoDB Data Migration Script
# From old schema to new Clean Architecture schema

# This script migrates data from the existing MongoDB structure to the new Clean Architecture structure.
# Run this after the new application is deployed but before switching traffic.

## Prerequisites
# 1. MongoDB instance accessible
# 2. Backup of existing data (recommended)
# 3. New application deployed and ready

## Migration Steps

### Step 1: Verify Current Collection Structure
```javascript
// Connect to MongoDB
use NewsDb

// Check current collection
db.News.findOne()

// Count existing records
print("Total records to migrate: " + db.News.count())
```

### Step 2: Data Validation and Cleanup
```javascript
// Find documents with missing required fields
db.News.find({
  $or: [
    { Category: { $exists: false } },
    { Type: { $exists: false } },
    { Caption: { $exists: false } },
    { Summary: { $exists: false } },
    { Content: { $exists: false } }
  ]
})

// Update documents with missing string fields to empty strings
db.News.updateMany(
  { Category: { $exists: false } },
  { $set: { Category: "" } }
)

db.News.updateMany(
  { Type: { $exists: false } },
  { $set: { Type: "" } }
)

db.News.updateMany(
  { Caption: { $exists: false } },
  { $set: { Caption: "" } }
)

// Ensure arrays are properly initialized
db.News.updateMany(
  { Subjects: { $exists: false } },
  { $set: { Subjects: [] } }
)

db.News.updateMany(
  { Authors: { $exists: false } },
  { $set: { Authors: [] } }
)
```

### Step 3: Schema Updates
```javascript
// Add CreateDate and UpdateDate if missing
db.News.updateMany(
  { CreateDate: { $exists: false } },
  { $set: { CreateDate: new Date() } }
)

db.News.updateMany(
  { UpdateDate: { $exists: false } },
  { $set: { UpdateDate: new Date() } }
)

// Ensure IsActive field exists (default to true for existing records)
db.News.updateMany(
  { IsActive: { $exists: false } },
  { $set: { IsActive: true } }
)

// Set default Priority if missing
db.News.updateMany(
  { Priority: { $exists: false } },
  { $set: { Priority: 1 } }
)
```

### Step 4: Index Creation for Performance
```javascript
// Create indexes for common queries
db.News.createIndex({ "IsActive": 1, "ExpressDate": -1 })
db.News.createIndex({ "Category": 1, "IsActive": 1 })
db.News.createIndex({ "Type": 1, "IsActive": 1 })
db.News.createIndex({ "Url": 1 }, { unique: true, sparse: true })
db.News.createIndex({ "CreateDate": -1 })
```

### Step 5: Verification
```javascript
// Verify migration completed successfully
var totalCount = db.News.count()
var activeCount = db.News.count({ IsActive: true })
var withCategoryCount = db.News.count({ Category: { $ne: "" } })

print("Migration Verification:")
print("Total records: " + totalCount)
print("Active records: " + activeCount)  
print("Records with Category: " + withCategoryCount)

// Check for any remaining null or undefined values
db.News.find({
  $or: [
    { Category: null },
    { Type: null },
    { Caption: null },
    { Summary: null },
    { Content: null }
  ]
}).count()
```

## PowerShell Script to Execute Migration
```powershell
# Execute MongoDB migration script
# Usage: .\migrate-data.ps1 -ConnectionString "mongodb://localhost:27017" -DatabaseName "NewsDb"

param(
    [Parameter(Mandatory=$true)]
    [string]$ConnectionString,
    
    [Parameter(Mandatory=$true)]
    [string]$DatabaseName
)

$migrationScript = @"
use $DatabaseName

print('Starting migration...')
var startTime = new Date()

// Step 1: Data validation and cleanup
print('Step 1: Data validation and cleanup')
db.News.updateMany({ Category: { `$exists: false } }, { `$set: { Category: "" } })
db.News.updateMany({ Type: { `$exists: false } }, { `$set: { Type: "" } })
db.News.updateMany({ Caption: { `$exists: false } }, { `$set: { Caption: "" } })
db.News.updateMany({ Keywords: { `$exists: false } }, { `$set: { Keywords: "" } })
db.News.updateMany({ SocialTags: { `$exists: false } }, { `$set: { SocialTags: "" } })
db.News.updateMany({ Summary: { `$exists: false } }, { `$set: { Summary: "" } })
db.News.updateMany({ ImgPath: { `$exists: false } }, { `$set: { ImgPath: "" } })
db.News.updateMany({ ImgAlt: { `$exists: false } }, { `$set: { ImgAlt: "" } })
db.News.updateMany({ Content: { `$exists: false } }, { `$set: { Content: "" } })
db.News.updateMany({ Url: { `$exists: false } }, { `$set: { Url: "" } })

// Step 2: Array fields
print('Step 2: Initialize array fields')
db.News.updateMany({ Subjects: { `$exists: false } }, { `$set: { Subjects: [] } })
db.News.updateMany({ Authors: { `$exists: false } }, { `$set: { Authors: [] } })

// Step 3: Date and status fields
print('Step 3: Date and status fields')
var now = new Date()
db.News.updateMany({ CreateDate: { `$exists: false } }, { `$set: { CreateDate: now } })
db.News.updateMany({ UpdateDate: { `$exists: false } }, { `$set: { UpdateDate: now } })
db.News.updateMany({ IsActive: { `$exists: false } }, { `$set: { IsActive: true } })
db.News.updateMany({ Priority: { `$exists: false } }, { `$set: { Priority: 1 } })
db.News.updateMany({ ViewCount: { `$exists: false } }, { `$set: { ViewCount: 0 } })
db.News.updateMany({ IsSecondPageNews: { `$exists: false } }, { `$set: { IsSecondPageNews: false } })

// Step 4: Create indexes
print('Step 4: Creating indexes')
db.News.createIndex({ "IsActive": 1, "ExpressDate": -1 })
db.News.createIndex({ "Category": 1, "IsActive": 1 })
db.News.createIndex({ "Type": 1, "IsActive": 1 })
db.News.createIndex({ "CreateDate": -1 })

// Step 5: Final verification
print('Step 5: Verification')
var totalCount = db.News.count()
var activeCount = db.News.count({ IsActive: true })

print('Migration completed successfully!')
print('Total records: ' + totalCount)
print('Active records: ' + activeCount)
print('Duration: ' + (new Date() - startTime) + 'ms')
"@

$migrationScript | mongosh $ConnectionString
```

## Rollback Script (Emergency)
```javascript
// Emergency rollback script - only if critical issues occur
use NewsDb

// Remove indexes created during migration (if needed)
db.News.dropIndex({ "IsActive": 1, "ExpressDate": -1 })
db.News.dropIndex({ "Category": 1, "IsActive": 1 })
db.News.dropIndex({ "Type": 1, "IsActive": 1 })
db.News.dropIndex({ "CreateDate": -1 })

print('Rollback completed - restore from backup if needed')
```

## Post-Migration Checklist
- [ ] Verify application can connect to database
- [ ] Test CRUD operations through API
- [ ] Verify caching is working correctly
- [ ] Check performance with indexes
- [ ] Monitor error logs for 24 hours
- [ ] Remove old Startup.cs and legacy files after successful deployment