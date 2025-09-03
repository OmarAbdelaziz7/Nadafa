using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddMissingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Only create tables if they don't exist
            // This is a manual migration to add missing entities

            // Create Notifications table if it doesn't exist
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS `Notifications` (
                    `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                    `UserId` int NOT NULL,
                    `Title` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
                    `Message` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
                    `IsRead` tinyint(1) NOT NULL,
                    `NotificationType` int NOT NULL,
                    `CreatedAt` datetime(6) NOT NULL,
                    CONSTRAINT `PK_Notifications` PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_Notifications_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
                ) CHARACTER SET utf8mb4;
            ");

            // Create index for Notifications if it doesn't exist
            migrationBuilder.Sql(@"
                SET @index_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
                                   WHERE TABLE_SCHEMA = 'NadafaDB' 
                                   AND TABLE_NAME = 'Notifications' 
                                   AND INDEX_NAME = 'IX_Notifications_UserId');
                SET @sql = IF(@index_exists > 0, 
                    'SELECT ''Index already exists'' as result',
                    'CREATE INDEX `IX_Notifications_UserId` ON `Notifications` (`UserId`)');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Create Purchases table if it doesn't exist
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS `Purchases` (
                    `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                    `MarketplaceItemId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                    `FactoryId` int NOT NULL,
                    `Quantity` decimal(18,2) NOT NULL,
                    `PricePerUnit` decimal(18,2) NOT NULL,
                    `StripePaymentIntentId` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
                    `PaymentStatus` int NOT NULL,
                    `PurchaseDate` datetime(6) NOT NULL,
                    `CreatedAt` datetime(6) NOT NULL,
                    `UpdatedAt` datetime(6) NOT NULL,
                    CONSTRAINT `PK_Purchases` PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_Purchases_Factories_FactoryId` FOREIGN KEY (`FactoryId`) REFERENCES `Factories` (`Id`) ON DELETE RESTRICT
                ) CHARACTER SET utf8mb4;
            ");

            // Create indexes for Purchases if they don't exist
            migrationBuilder.Sql(@"
                SET @index_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
                                   WHERE TABLE_SCHEMA = 'NadafaDB' 
                                   AND TABLE_NAME = 'Purchases' 
                                   AND INDEX_NAME = 'IX_Purchases_FactoryId');
                SET @sql = IF(@index_exists > 0, 
                    'SELECT ''Index already exists'' as result',
                    'CREATE INDEX `IX_Purchases_FactoryId` ON `Purchases` (`FactoryId`)');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Add ImageUrls column to PickupRequests if it doesn't exist
            migrationBuilder.Sql(@"
                SET @column_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                                     WHERE TABLE_SCHEMA = 'NadafaDB' 
                                     AND TABLE_NAME = 'PickupRequests' 
                                     AND COLUMN_NAME = 'ImageUrls');
                SET @sql = IF(@column_exists > 0, 
                    'SELECT ''Column already exists'' as result',
                    'ALTER TABLE `PickupRequests` ADD COLUMN `ImageUrls` longtext CHARACTER SET utf8mb4 NOT NULL');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Add AdminId column to PickupRequests if it doesn't exist
            migrationBuilder.Sql(@"
                SET @column_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                                     WHERE TABLE_SCHEMA = 'NadafaDB' 
                                     AND TABLE_NAME = 'PickupRequests' 
                                     AND COLUMN_NAME = 'AdminId');
                SET @sql = IF(@column_exists > 0, 
                    'SELECT ''Column already exists'' as result',
                    'ALTER TABLE `PickupRequests` ADD COLUMN `AdminId` int NULL');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Add foreign key for AdminId if it doesn't exist
            migrationBuilder.Sql(@"
                SET @fk_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                                 WHERE CONSTRAINT_SCHEMA = 'NadafaDB' 
                                 AND TABLE_NAME = 'PickupRequests' 
                                 AND CONSTRAINT_NAME = 'FK_PickupRequests_Users_AdminId');
                SET @sql = IF(@fk_exists > 0, 
                    'SELECT ''Foreign key already exists'' as result',
                    'ALTER TABLE `PickupRequests` ADD CONSTRAINT `FK_PickupRequests_Users_AdminId` FOREIGN KEY (`AdminId`) REFERENCES `Users` (`Id`) ON DELETE RESTRICT');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Add missing columns to MarketplaceItems if they don't exist
            migrationBuilder.Sql(@"
                SET @column_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'NadafaDB' AND TABLE_NAME = 'MarketplaceItems' AND COLUMN_NAME = 'ImageUrls');
                SET @sql = IF(@column_exists > 0, 'SELECT ''Column already exists'' as result', 'ALTER TABLE `MarketplaceItems` ADD COLUMN `ImageUrls` longtext CHARACTER SET utf8mb4 NOT NULL');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
                
                SET @column_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'NadafaDB' AND TABLE_NAME = 'MarketplaceItems' AND COLUMN_NAME = 'IsAvailable');
                SET @sql = IF(@column_exists > 0, 'SELECT ''Column already exists'' as result', 'ALTER TABLE `MarketplaceItems` ADD COLUMN `IsAvailable` tinyint(1) NOT NULL DEFAULT 0');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
                
                SET @column_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'NadafaDB' AND TABLE_NAME = 'MarketplaceItems' AND COLUMN_NAME = 'CreatedAt');
                SET @sql = IF(@column_exists > 0, 'SELECT ''Column already exists'' as result', 'ALTER TABLE `MarketplaceItems` ADD COLUMN `CreatedAt` datetime(6) NOT NULL DEFAULT ''1900-01-01 00:00:00''');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
                
                SET @column_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'NadafaDB' AND TABLE_NAME = 'MarketplaceItems' AND COLUMN_NAME = 'UpdatedAt');
                SET @sql = IF(@column_exists > 0, 'SELECT ''Column already exists'' as result', 'ALTER TABLE `MarketplaceItems` ADD COLUMN `UpdatedAt` datetime(6) NOT NULL DEFAULT ''1900-01-01 00:00:00''');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
                
                SET @column_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'NadafaDB' AND TABLE_NAME = 'MarketplaceItems' AND COLUMN_NAME = 'UserId');
                SET @sql = IF(@column_exists > 0, 'SELECT ''Column already exists'' as result', 'ALTER TABLE `MarketplaceItems` ADD COLUMN `UserId` int NULL');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");

            // Add foreign key for UserId to MarketplaceItems if it doesn't exist
            migrationBuilder.Sql(@"
                SET @fk_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                                 WHERE CONSTRAINT_SCHEMA = 'NadafaDB' 
                                 AND TABLE_NAME = 'MarketplaceItems' 
                                 AND CONSTRAINT_NAME = 'FK_MarketplaceItems_Users_UserId');
                SET @sql = IF(@fk_exists > 0, 
                    'SELECT ''Foreign key already exists'' as result',
                    'ALTER TABLE `MarketplaceItems` ADD CONSTRAINT `FK_MarketplaceItems_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE RESTRICT');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Add indexes if they don't exist
            migrationBuilder.Sql(@"
                SET @index_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
                                   WHERE TABLE_SCHEMA = 'NadafaDB' 
                                   AND TABLE_NAME = 'PickupRequests' 
                                   AND INDEX_NAME = 'IX_PickupRequests_AdminId');
                SET @sql = IF(@index_exists > 0, 
                    'SELECT ''Index already exists'' as result',
                    'CREATE INDEX `IX_PickupRequests_AdminId` ON `PickupRequests` (`AdminId`)');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @index_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
                                   WHERE TABLE_SCHEMA = 'NadafaDB' 
                                   AND TABLE_NAME = 'MarketplaceItems' 
                                   AND INDEX_NAME = 'IX_MarketplaceItems_UserId');
                SET @sql = IF(@index_exists > 0, 
                    'SELECT ''Index already exists'' as result',
                    'CREATE INDEX `IX_MarketplaceItems_UserId` ON `MarketplaceItems` (`UserId`)');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the tables and columns we added
            migrationBuilder.DropTable(name: "Notifications");
            migrationBuilder.DropTable(name: "Purchases");

            migrationBuilder.DropColumn(name: "ImageUrls", table: "PickupRequests");
            migrationBuilder.DropColumn(name: "AdminId", table: "PickupRequests");

            migrationBuilder.DropColumn(name: "ImageUrls", table: "MarketplaceItems");
            migrationBuilder.DropColumn(name: "IsAvailable", table: "MarketplaceItems");
            migrationBuilder.DropColumn(name: "CreatedAt", table: "MarketplaceItems");
            migrationBuilder.DropColumn(name: "UpdatedAt", table: "MarketplaceItems");
            migrationBuilder.DropColumn(name: "UserId", table: "MarketplaceItems");
        }
    }
}
