using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddPurchaseAndNotificationEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // This migration safely ensures Purchase and Notification entities are properly configured
            // Most tables were already created in the AddMissingTables migration
            // This migration just ensures any missing pieces are added safely

            // Ensure TotalAmount column exists in Purchases table
            migrationBuilder.Sql(@"
                SET @column_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                                     WHERE TABLE_SCHEMA = 'NadafaDB' 
                                     AND TABLE_NAME = 'Purchases' 
                                     AND COLUMN_NAME = 'TotalAmount');
                SET @sql = IF(@column_exists > 0, 
                    'SELECT ''TotalAmount column already exists'' as result',
                    'ALTER TABLE `Purchases` ADD COLUMN `TotalAmount` decimal(18,2) NOT NULL DEFAULT 0');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Ensure foreign key between Purchases and MarketplaceItems exists (with type compatibility check)
            migrationBuilder.Sql(@"
                SET @purchases_type = (SELECT COLUMN_TYPE FROM INFORMATION_SCHEMA.COLUMNS 
                                      WHERE TABLE_SCHEMA = 'NadafaDB' 
                                      AND TABLE_NAME = 'Purchases' 
                                      AND COLUMN_NAME = 'MarketplaceItemId');
                                      
                SET @marketplace_type = (SELECT COLUMN_TYPE FROM INFORMATION_SCHEMA.COLUMNS 
                                        WHERE TABLE_SCHEMA = 'NadafaDB' 
                                        AND TABLE_NAME = 'MarketplaceItems' 
                                        AND COLUMN_NAME = 'Id');

                SET @types_match = (@purchases_type = @marketplace_type);
                
                SET @fk_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                                 WHERE CONSTRAINT_SCHEMA = 'NadafaDB' 
                                 AND TABLE_NAME = 'Purchases' 
                                 AND CONSTRAINT_NAME = 'FK_Purchases_MarketplaceItems_MarketplaceItemId');

                SET @sql = IF(@fk_exists > 0, 
                    'SELECT ''Foreign key already exists'' as result',
                    IF(@types_match = 1,
                        'ALTER TABLE `Purchases` ADD CONSTRAINT `FK_Purchases_MarketplaceItems_MarketplaceItemId` FOREIGN KEY (`MarketplaceItemId`) REFERENCES `MarketplaceItems` (`Id`) ON DELETE RESTRICT',
                        'SELECT ''Cannot create FK: incompatible types'' as result'));
                        
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Ensure unique index on Purchases.MarketplaceItemId exists
            migrationBuilder.Sql(@"
                SET @index_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
                                   WHERE TABLE_SCHEMA = 'NadafaDB' 
                                   AND TABLE_NAME = 'Purchases' 
                                   AND INDEX_NAME = 'IX_Purchases_MarketplaceItemId');
                SET @sql = IF(@index_exists > 0, 
                    'SELECT ''Index already exists'' as result',
                    'CREATE UNIQUE INDEX `IX_Purchases_MarketplaceItemId` ON `Purchases` (`MarketplaceItemId`)');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Ensure Notifications table has correct column lengths
            migrationBuilder.Sql(@"
                SET @title_length = (SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS 
                                    WHERE TABLE_SCHEMA = 'NadafaDB' 
                                    AND TABLE_NAME = 'Notifications' 
                                    AND COLUMN_NAME = 'Title');
                SET @sql = IF(@title_length >= 200, 
                    'SELECT ''Title column length is adequate'' as result',
                    'ALTER TABLE `Notifications` MODIFY COLUMN `Title` varchar(200) CHARACTER SET utf8mb4 NOT NULL');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @message_length = (SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS 
                                      WHERE TABLE_SCHEMA = 'NadafaDB' 
                                      AND TABLE_NAME = 'Notifications' 
                                      AND COLUMN_NAME = 'Message');
                SET @sql = IF(@message_length >= 1000, 
                    'SELECT ''Message column length is adequate'' as result',
                    'ALTER TABLE `Notifications` MODIFY COLUMN `Message` varchar(1000) CHARACTER SET utf8mb4 NOT NULL');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // For safety, we don't implement Down operations that could cause data loss
            // If rollback is needed, it should be done manually with proper data backup
        }
    }
}

