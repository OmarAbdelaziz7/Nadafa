-- NADAFA Database Test Queries
-- Run these queries after completing the migration to verify entity relationships

-- 1. Verify Users table structure
DESCRIBE Users;

-- 2. Verify Factories table structure  
DESCRIBE Factories;

-- 3. Verify PickupRequests table structure
DESCRIBE PickupRequests;

-- 4. Verify MarketplaceItems table structure
DESCRIBE MarketplaceItems;

-- 5. Verify Purchases table structure
DESCRIBE Purchases;

-- 6. Verify Notifications table structure
DESCRIBE Notifications;

-- 7. Check foreign key constraints
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    CONSTRAINT_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE REFERENCED_TABLE_SCHEMA = 'NadafaDB'
ORDER BY TABLE_NAME, COLUMN_NAME;

-- 8. Sample data insertion test
-- Insert test user
INSERT INTO Users (Name, Email, Address, Age, PasswordHash, Role, CreatedAt, IsActive)
VALUES ('Test User', 'test@example.com', 'Test Address', 25, 'test_hash', 1, NOW(), 1);

-- Insert test factory
INSERT INTO Factories (Name, Email, Address, PasswordHash, PhoneNumber, BusinessLicense, Role, CreatedAt, IsActive, IsVerified)
VALUES ('Test Factory', 'factory@test.com', 'Factory Address', 'test_hash', '123456789', 'LIC123', 3, NOW(), 1, 1);

-- 9. Verify data insertion
SELECT * FROM Users WHERE Email = 'test@example.com';
SELECT * FROM Factories WHERE Email = 'factory@test.com';

-- 10. Clean up test data
-- DELETE FROM Users WHERE Email = 'test@example.com';
-- DELETE FROM Factories WHERE Email = 'factory@test.com';

-- 11. Check table row counts
SELECT 'Users' as TableName, COUNT(*) as RowCount FROM Users
UNION ALL
SELECT 'Factories', COUNT(*) FROM Factories
UNION ALL
SELECT 'PickupRequests', COUNT(*) FROM PickupRequests
UNION ALL
SELECT 'MarketplaceItems', COUNT(*) FROM MarketplaceItems
UNION ALL
SELECT 'Purchases', COUNT(*) FROM Purchases
UNION ALL
SELECT 'Notifications', COUNT(*) FROM Notifications;
