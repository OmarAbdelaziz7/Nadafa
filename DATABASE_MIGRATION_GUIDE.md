# Database Migration Guide for NADAFA Project

This comprehensive guide provides the **best practices and bulletproof approaches** for managing database migrations in your .NET 5 Entity Framework Core project using MySQL, ensuring you never face constraint/column dropping issues again.

## Project Overview

- **Framework**: .NET 5.0
- **ORM**: Entity Framework Core 5.0.17
- **Database**: MySQL (using Pomelo.EntityFrameworkCore.MySql 5.0.4)
- **DbContext**: `NadafaDbContext` located in `Infrastructure/Data/NadafaDbContext.cs`

## Prerequisites

Before creating migrations, ensure you have:

1. **.NET 5 SDK** installed
2. **Entity Framework Core Tools** installed globally:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
3. **MySQL Server** running and accessible
4. **Connection string** properly configured in `appsettings.json`
5. **Database backup strategy** in place

## ⚠️ CRITICAL: Root Directory Requirement

**ALWAYS execute migration commands from the root directory** where `Template.sln` is located:

```bash
cd "C:\Users\6545t\Desktop\iScore Proj\Nadafa"
```

## 🔄 The SAFE Migration Workflow

Follow this workflow to avoid the issues you previously encountered:

### Phase 1: Pre-Migration Safety Checks

Before making any changes:

```bash
# 1. Verify current migration status
dotnet ef migrations list --project Infrastructure --startup-project Presentation

# 2. Backup your database
# Create a MySQL dump of your current database state

# 3. Check if your model is in sync
dotnet ef dbcontext info --project Infrastructure --startup-project Presentation

# 4. Build your project to ensure no errors
dotnet build
```

### Phase 2: Entity Changes and Migration Creation

#### Step 1: Make Your Entity Changes
- Modify entities in `Domain/Entities/`
- Update `NadafaDbContext.cs` configurations
- Add proper value comparers for collections (see Value Comparer section below)

#### Step 2: Generate Migration with Validation
```bash
# Generate the migration
dotnet ef migrations add YourMigrationName --project Infrastructure --startup-project Presentation

# IMMEDIATELY review the generated migration file
# Look for any DROP operations that might fail
```

#### Step 3: Validate Generated Migration
Before applying, check the generated migration for:

❌ **Dangerous Operations to Watch For:**
- `DropForeignKey()` - May reference non-existent constraints
- `DropIndex()` - May reference non-existent indexes  
- `DropColumn()` - May reference non-existent columns
- `RenameColumn()` - May reference non-existent columns

✅ **Safe Operations:**
- `CreateTable()`
- `AddColumn()`
- `AddForeignKey()`
- `CreateIndex()`
- `AlterColumn()` (usually safe)

### Phase 3: Safe Migration Application

#### Option A: Direct Application (Development Only)
```bash
# Only use in development when you're confident
dotnet ef database update --project Infrastructure --startup-project Presentation
```

#### Option B: Generate SQL Script First (Recommended)
```bash
# Generate SQL script to review
dotnet ef migrations script --project Infrastructure --startup-project Presentation --output migration_review.sql

# Review the SQL script manually
# Apply manually if needed, or use the update command if script looks good
dotnet ef database update --project Infrastructure --startup-project Presentation
```

#### Option C: Safe Custom Migration (For Complex Changes)
If your generated migration contains dangerous DROP operations, create a custom migration using the template below.

## 🛡️ Bulletproof Migration Templates

### Template 1: Safe Column Addition
```csharp
public partial class YourMigrationName : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Safe column additions with existence checks
        migrationBuilder.Sql(@"
            SET @column_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                                 WHERE TABLE_SCHEMA = 'NadafaDB' 
                                 AND TABLE_NAME = 'YourTable' 
                                 AND COLUMN_NAME = 'YourColumn');
            SET @sql = IF(@column_exists > 0, 
                'SELECT ''Column already exists'' as result',
                'ALTER TABLE `YourTable` ADD COLUMN `YourColumn` varchar(100) NOT NULL');
            PREPARE stmt FROM @sql;
            EXECUTE stmt;
            DEALLOCATE PREPARE stmt;
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "YourColumn", table: "YourTable");
    }
}
```

### Template 2: Safe Foreign Key Addition
```csharp
public partial class AddForeignKeySafely : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Add column first (if it doesn't exist)
        migrationBuilder.Sql(@"
            SET @column_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                                 WHERE TABLE_SCHEMA = 'NadafaDB' 
                                 AND TABLE_NAME = 'ChildTable' 
                                 AND COLUMN_NAME = 'ParentId');
            SET @sql = IF(@column_exists > 0, 
                'SELECT ''Column already exists'' as result',
                'ALTER TABLE `ChildTable` ADD COLUMN `ParentId` int NULL');
            PREPARE stmt FROM @sql;
            EXECUTE stmt;
            DEALLOCATE PREPARE stmt;
        ");

        // Add foreign key constraint (if it doesn't exist)
        migrationBuilder.Sql(@"
            SET @fk_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                             WHERE CONSTRAINT_SCHEMA = 'NadafaDB' 
                             AND TABLE_NAME = 'ChildTable' 
                             AND CONSTRAINT_NAME = 'FK_ChildTable_ParentTable_ParentId');
            SET @sql = IF(@fk_exists > 0, 
                'SELECT ''Foreign key already exists'' as result',
                'ALTER TABLE `ChildTable` ADD CONSTRAINT `FK_ChildTable_ParentTable_ParentId` FOREIGN KEY (`ParentId`) REFERENCES `ParentTable` (`Id`) ON DELETE RESTRICT');
            PREPARE stmt FROM @sql;
            EXECUTE stmt;
            DEALLOCATE PREPARE stmt;
        ");

        // Add index (if it doesn't exist)
        migrationBuilder.Sql(@"
            SET @index_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
                               WHERE TABLE_SCHEMA = 'NadafaDB' 
                               AND TABLE_NAME = 'ChildTable' 
                               AND INDEX_NAME = 'IX_ChildTable_ParentId');
            SET @sql = IF(@index_exists > 0, 
                'SELECT ''Index already exists'' as result',
                'CREATE INDEX `IX_ChildTable_ParentId` ON `ChildTable` (`ParentId`)');
            PREPARE stmt FROM @sql;
            EXECUTE stmt;
            DEALLOCATE PREPARE stmt;
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_ChildTable_ParentTable_ParentId", table: "ChildTable");
        migrationBuilder.DropIndex(name: "IX_ChildTable_ParentId", table: "ChildTable");
        migrationBuilder.DropColumn(name: "ParentId", table: "ChildTable");
    }
}
```

### Template 3: Safe Table Creation
```csharp
public partial class CreateNewTableSafely : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS `NewTable` (
                `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
                `CreatedAt` datetime(6) NOT NULL,
                CONSTRAINT `PK_NewTable` PRIMARY KEY (`Id`)
            ) CHARACTER SET utf8mb4;
        ");

        // Add index if it doesn't exist
        migrationBuilder.Sql(@"
            SET @index_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
                               WHERE TABLE_SCHEMA = 'NadafaDB' 
                               AND TABLE_NAME = 'NewTable' 
                               AND INDEX_NAME = 'IX_NewTable_Name');
            SET @sql = IF(@index_exists > 0, 
                'SELECT ''Index already exists'' as result',
                'CREATE INDEX `IX_NewTable_Name` ON `NewTable` (`Name`)');
            PREPARE stmt FROM @sql;
            EXECUTE stmt;
            DEALLOCATE PREPARE stmt;
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "NewTable");
    }
}
```

## 🔧 Essential DbContext Configurations

### Value Comparers for Collections (Prevents Warnings)
Always add value comparers for collection properties to avoid EF warnings:

```csharp
// In your NadafaDbContext.cs OnModelCreating method
entity.Property(e => e.ImageUrls)
    .HasConversion(
        v => string.Join(',', v),
        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
    )
    .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
        (c1, c2) => c1.SequenceEqual(c2),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c.ToList()));
```

### Required Using Statements
Add these to your DbContext file:
```csharp
using System;
using System.Linq;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
```

## 📋 Migration Commands Reference

### Essential Commands
```bash
# Create migration
dotnet ef migrations add [MigrationName] --project Infrastructure --startup-project Presentation

# Review migration without applying
dotnet ef migrations script --project Infrastructure --startup-project Presentation

# Apply migration
dotnet ef database update --project Infrastructure --startup-project Presentation

# List all migrations
dotnet ef migrations list --project Infrastructure --startup-project Presentation

# Remove last migration (if not applied)
dotnet ef migrations remove --project Infrastructure --startup-project Presentation
```

### Advanced Commands
```bash
# Generate script for specific migration range
dotnet ef migrations script [FromMigration] [ToMigration] --project Infrastructure --startup-project Presentation

# Rollback to specific migration
dotnet ef database update [MigrationName] --project Infrastructure --startup-project Presentation

# Generate script for production deployment
dotnet ef migrations script --project Infrastructure --startup-project Presentation --output production_migration.sql --idempotent
```

## 🚨 Common Issues and BULLETPROOF Solutions

### Issue 1: "Can't DROP [constraint/column]; check that it exists"

**Root Cause**: EF model snapshot is out of sync with actual database state.

**BULLETPROOF SOLUTION**:
1. **Never edit generated migrations** - create new ones instead
2. **Always use existence checks** in custom migrations
3. **Backup before applying** any migration
4. **Use the safe templates** provided above

**Emergency Fix**:
```bash
# If you're stuck with a failing migration:
dotnet ef migrations remove --project Infrastructure --startup-project Presentation
# Then create a custom safe migration using the templates above
```

### Issue 2: MySQL TEXT/BLOB Default Value Errors

**Problem**: MySQL doesn't allow default values for TEXT/LONGTEXT columns.

**SOLUTION**: Remove DEFAULT clauses from TEXT columns:
```sql
-- ❌ Wrong:
ALTER TABLE `Table` ADD COLUMN `TextColumn` longtext NOT NULL DEFAULT ''

-- ✅ Correct:
ALTER TABLE `Table` ADD COLUMN `TextColumn` longtext NOT NULL
```

### Issue 3: Foreign Key Constraint Violations

**BULLETPROOF APPROACH**:
1. **Add columns first** (nullable initially)
2. **Populate data** if needed
3. **Add constraints** after data is consistent
4. **Make non-nullable** if required

Example:
```csharp
// Step 1: Add nullable column
migrationBuilder.AddColumn<int?>("UserId", "Orders", nullable: true);

// Step 2: Populate data (if needed)
migrationBuilder.Sql("UPDATE Orders SET UserId = 1 WHERE UserId IS NULL");

// Step 3: Add foreign key
migrationBuilder.AddForeignKey("FK_Orders_Users_UserId", "Orders", "UserId", "Users", "Id");

// Step 4: Make non-nullable (if needed)
migrationBuilder.AlterColumn<int>("UserId", "Orders", nullable: false);
```

### Issue 4: Model Snapshot Corruption

**Signs**:
- EF generates migrations with unexpected DROP operations
- Migration tries to drop things that don't exist

**BULLETPROOF FIX**:
1. Create a clean migration using only safe operations
2. Use existence checks for all operations
3. Apply migration manually if needed

## 🔄 Migration Workflow Checklist

### Before Every Migration:
- [ ] Database is backed up
- [ ] All code compiles successfully
- [ ] You're in the correct directory (`Template.sln` location)
- [ ] MySQL server is running
- [ ] No uncommitted changes in source control

### After Creating Migration:
- [ ] Review the generated `.cs` file
- [ ] Check for dangerous DROP operations
- [ ] Test migration on a copy of production data
- [ ] Generate SQL script for review

### Before Applying to Production:
- [ ] Migration tested in staging environment
- [ ] SQL script reviewed by team
- [ ] Rollback plan prepared
- [ ] Downtime window scheduled (if needed)
- [ ] Database backup completed

## 🚀 Production Deployment Strategy

### Method 1: SQL Scripts (Recommended)
```bash
# Generate idempotent script
dotnet ef migrations script --project Infrastructure --startup-project Presentation --output prod_migration.sql --idempotent

# Review script
# Apply during maintenance window
```

### Method 2: Blue-Green Deployment
1. Deploy to green environment
2. Apply migrations
3. Switch traffic
4. Keep blue as rollback

### Method 3: Rolling Updates
1. Apply backward-compatible changes first
2. Deploy application
3. Apply cleanup migrations later

## 📊 Migration Naming Best Practices

Use this naming convention:
```
YYYYMMDD_SequenceNumber_ActionEntityDescription

Examples:
20241203_01_AddUserEmailVerification
20241203_02_CreateNotificationTable  
20241203_03_UpdateOrderStatusEnum
20241203_04_RemoveObsoleteColumns
```

## 🔍 Debugging Failed Migrations

### Step 1: Identify the Issue
```bash
# Check current state
dotnet ef migrations list --project Infrastructure --startup-project Presentation

# Check what EF thinks vs reality
dotnet ef dbcontext info --project Infrastructure --startup-project Presentation
```

### Step 2: Fix Strategy
1. **Remove the failing migration** if not applied
2. **Create a custom safe migration** 
3. **Use existence checks** for all operations
4. **Test thoroughly** before production

### Step 3: Prevention
- Always use the safe templates
- Never skip the review step
- Test on production-like data

## ⚡ Quick Reference Commands

| Action | Command |
|--------|---------|
| **Create Safe Migration** | Use templates above, always include existence checks |
| **Create Standard Migration** | `dotnet ef migrations add [Name] --project Infrastructure --startup-project Presentation` |
| **Review Before Apply** | `dotnet ef migrations script --project Infrastructure --startup-project Presentation` |
| **Apply Migration** | `dotnet ef database update --project Infrastructure --startup-project Presentation` |
| **Emergency Rollback** | `dotnet ef database update [PreviousMigration] --project Infrastructure --startup-project Presentation` |
| **Remove Last Migration** | `dotnet ef migrations remove --project Infrastructure --startup-project Presentation` |
| **List All Migrations** | `dotnet ef migrations list --project Infrastructure --startup-project Presentation` |

## 🎯 Remember: The Golden Rules

1. **🛡️ ALWAYS use existence checks** in custom migrations
2. **📁 ALWAYS execute from root directory** (`Template.sln` location)
3. **💾 ALWAYS backup before applying** migrations
4. **👀 ALWAYS review generated migrations** before applying
5. **🔄 NEVER edit applied migrations** - create new ones
6. **📋 ALWAYS test on staging** before production
7. **📝 ALWAYS document breaking changes**

---

**This guide ensures you'll never face the constraint dropping issues again. Follow the safe templates and workflows, and your migrations will be bulletproof! 🚀**