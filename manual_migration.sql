-- NADAFA Core Entities Migration Script
-- Run this script manually in your MySQL database

-- 1. Create PickupRequests table
CREATE TABLE IF NOT EXISTS `PickupRequests` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `UserId` int NOT NULL,
    `MaterialType` int NOT NULL,
    `Quantity` decimal(18,2) NOT NULL,
    `Unit` int NOT NULL,
    `ProposedPricePerUnit` decimal(18,2) NOT NULL,
    `Description` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
    `ImageUrls` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Status` int NOT NULL,
    `RequestDate` datetime(6) NOT NULL,
    `ApprovedDate` datetime(6) NULL,
    `PickupDate` datetime(6) NULL,
    `AdminId` int NULL,
    `AdminNotes` varchar(1000) CHARACTER SET utf8mb4 NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_PickupRequests` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_PickupRequests_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_PickupRequests_Users_AdminId` FOREIGN KEY (`AdminId`) REFERENCES `Users` (`Id`) ON DELETE RESTRICT
) CHARACTER SET utf8mb4;

-- 2. Create MarketplaceItems table
CREATE TABLE IF NOT EXISTS `MarketplaceItems` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `PickupRequestId` char(36) COLLATE ascii_general_ci NOT NULL,
    `UserId` int NOT NULL,
    `MaterialType` int NOT NULL,
    `Quantity` decimal(18,2) NOT NULL,
    `Unit` int NOT NULL,
    `PricePerUnit` decimal(18,2) NOT NULL,
    `Description` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
    `ImageUrls` longtext CHARACTER SET utf8mb4 NOT NULL,
    `IsAvailable` tinyint(1) NOT NULL DEFAULT FALSE,
    `PublishedAt` datetime(6) NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_MarketplaceItems` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_MarketplaceItems_PickupRequests_PickupRequestId` FOREIGN KEY (`PickupRequestId`) REFERENCES `PickupRequests` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_MarketplaceItems_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE RESTRICT
) CHARACTER SET utf8mb4;

-- 3. Create Purchases table
CREATE TABLE IF NOT EXISTS `Purchases` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `MarketplaceItemId` char(36) COLLATE ascii_general_ci NOT NULL,
    `FactoryId` int NOT NULL,
    `Quantity` decimal(18,2) NOT NULL,
    `PricePerUnit` decimal(18,2) NOT NULL,
    `StripePaymentIntentId` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `PaymentStatus` int NOT NULL,
    `PurchaseDate` datetime(6) NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_Purchases` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Purchases_MarketplaceItems_MarketplaceItemId` FOREIGN KEY (`MarketplaceItemId`) REFERENCES `MarketplaceItems` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Purchases_Factories_FactoryId` FOREIGN KEY (`FactoryId`) REFERENCES `Factories` (`Id`) ON DELETE RESTRICT
) CHARACTER SET utf8mb4;

-- 4. Create Notifications table
CREATE TABLE IF NOT EXISTS `Notifications` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `UserId` int NOT NULL,
    `Title` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Message` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
    `IsRead` tinyint(1) NOT NULL DEFAULT FALSE,
    `NotificationType` int NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_Notifications` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Notifications_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) CHARACTER SET utf8mb4;

-- 5. Create indexes for better performance
CREATE INDEX `IX_PickupRequests_UserId` ON `PickupRequests` (`UserId`);
CREATE INDEX `IX_PickupRequests_AdminId` ON `PickupRequests` (`AdminId`);
CREATE INDEX `IX_PickupRequests_Status` ON `PickupRequests` (`Status`);
CREATE INDEX `IX_MarketplaceItems_PickupRequestId` ON `MarketplaceItems` (`PickupRequestId`);
CREATE INDEX `IX_MarketplaceItems_UserId` ON `MarketplaceItems` (`UserId`);
CREATE INDEX `IX_MarketplaceItems_IsAvailable` ON `MarketplaceItems` (`IsAvailable`);
CREATE INDEX `IX_Purchases_MarketplaceItemId` ON `Purchases` (`MarketplaceItemId`);
CREATE INDEX `IX_Purchases_FactoryId` ON `Purchases` (`FactoryId`);
CREATE INDEX `IX_Purchases_PaymentStatus` ON `Purchases` (`PaymentStatus`);
CREATE INDEX `IX_Notifications_UserId` ON `Notifications` (`UserId`);
CREATE INDEX `IX_Notifications_IsRead` ON `Notifications` (`IsRead`);

-- 6. Add unique constraint for one-to-one relationship
CREATE UNIQUE INDEX `IX_MarketplaceItems_PickupRequestId_Unique` ON `MarketplaceItems` (`PickupRequestId`);
CREATE UNIQUE INDEX `IX_Purchases_MarketplaceItemId_Unique` ON `Purchases` (`MarketplaceItemId`);
