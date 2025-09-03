# NADAFA Recycling Platform

A Clean Architecture .NET 5.0 recycling platform that connects users, admins, and factories for recyclable material management.

## 🏗️ Architecture

- **Domain Layer**: Core entities, enums, and business logic
- **Application Layer**: Services, DTOs, and contracts
- **Infrastructure Layer**: Data access, external services (Stripe, SendGrid)
- **Presentation Layer**: API controllers and configuration

## 🎯 Features

- **User Management**: Registration and authentication for Users, Admins, and Factories
- **Pickup Requests**: Users can request pickup of recyclable materials with pricing
- **Admin Approval**: Admins approve/reject pickup requests and process payments
- **Marketplace**: Approved materials are published for factories to purchase
- **Payment Processing**: Stripe integration for secure payments
- **Notifications**: SendGrid integration for email notifications

## 🚀 Prerequisites

- .NET 5.0 SDK
- MySQL Server 8.0+
- Visual Studio 2019/2022 or VS Code

## 📦 Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd Nadafa
```

### 2. Install Dependencies
```bash
dotnet restore
```

### 3. Database Setup

#### Create MySQL Database
```sql
CREATE DATABASE NadafaDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER 'nadafa_user'@'localhost' IDENTIFIED BY 'your_secure_password';
GRANT ALL PRIVILEGES ON NadafaDB.* TO 'nadafa_user'@'localhost';
FLUSH PRIVILEGES;
```

#### Update Connection String
Edit `Presentation/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=NadafaDB;User=nadafa_user;Password=your_secure_password;Port=3306;"
  }
}
```

### 4. Environment Variables
The `.env` file contains sensitive credentials. Update `appsettings.json` with these values:

```json
{
  "StripeSettings": {
    "SecretKey": "sk_test_51R5nR3Ho1FFIeLqZ2ht1OnmtTrjlmGyRJmbRA053o91fHpqMlp24pFnB0bbR7cXuMgjOTmO5DzejqR5L79v5Vy7V00WtBoFk2P",
    "PublicKey": "pk_test_51R5nR3Ho1FFIeLqZ2MqYVYAfYcDBEc8f6qqpYkbaPFh53hvBEVlvpHYgTCVeQNgXBxP8SuCPpy2m1tWDaUFQi3r400EJhnf3bo"
  },
  "SendGridSettings": {
    "ApiKey": "SG.jxI4CJSHQail953UGe_i9Q.482EbJQjjFQkksPCvFHLFBo-84YeC89GB59iF_DHHu8",
    "FromEmail": "islamhk1234@gmail.com"
  }
}
```

### 5. Database Migrations
```bash
# Navigate to Presentation project
cd Presentation

# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migration to database
dotnet ef database update
```

### 6. Run the Application
```bash
dotnet run
```

The API will be available at: `https://localhost:5001` or `http://localhost:5000`

## 🔐 Authentication Endpoints

### User Registration
```http
POST /api/auth/register/user
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "address": "123 Main St, City, State",
  "age": 25,
  "password": "SecurePass123!"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "User registered successfully",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-15T10:30:00Z",
  "email": "john@example.com",
  "role": "User"
}
```

### Factory Registration
```http
POST /api/auth/register/factory
Content-Type: application/json

{
  "name": "Green Recycling Co",
  "email": "info@greenrecycling.com",
  "address": "456 Industrial Blvd, City, State",
  "password": "SecurePass123!",
  "phoneNumber": "+1-555-0123",
  "businessLicense": "GRC123456"
}
```

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

### Validate Token
```http
POST /api/auth/validate-token
Authorization: Bearer <your-jwt-token>
```

### Get Current User
```http
GET /api/auth/me
Authorization: Bearer <your-jwt-token>
```

### Change Password (Requires Authentication)
```http
POST /api/auth/change-password
Authorization: Bearer <your-jwt-token>
Content-Type: application/json

{
  "currentPassword": "SecurePass123!",
  "newPassword": "NewSecurePass456!",
  "confirmNewPassword": "NewSecurePass456!"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Password changed successfully",
  "email": "john@example.com",
  "role": "User"
}
```

### Sign Out (Requires Authentication)
```http
POST /api/auth/signout
Authorization: Bearer <your-jwt-token>
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Successfully signed out"
}
```

## 🧪 Manual Testing Guide

### Prerequisites
1. Ensure the API is running: `dotnet run -p Presentation`
2. Open Swagger UI: `https://localhost:5001/swagger`
3. Have a test user registered (use the registration endpoints)

### Test Scenarios

#### 1. User Registration & Login Flow
1. **Register a new user** using `/api/auth/register/user`
2. **Login** using `/api/auth/login` with the registered credentials
3. **Copy the JWT token** from the response
4. **Test token validation** using `/api/auth/validate-token` with the token
5. **Get current user info** using `/api/auth/me` with the token

#### 2. Factory Registration & Login Flow
1. **Register a new factory** using `/api/auth/register/factory`
2. **Login** using `/api/auth/login` with the factory credentials
3. **Verify the role** in the response shows "Factory"

#### 3. Password Change Flow (Authenticated User)
1. **Login** to get a valid JWT token
2. **Change password** using `/api/auth/change-password` with:
   - Current password
   - New password
   - Confirm new password
3. **Verify success message**
4. **Try logging in** with the old password (should fail)
5. **Login with new password** (should succeed)

#### 4. Sign Out Flow
1. **Login** to get a valid JWT token
2. **Sign out** using `/api/auth/signout` with the token
3. **Verify success message**
4. **Try using the token** for any authenticated endpoint (should fail)

#### 5. Admin Login Flow
1. **Login as admin** using `/api/auth/login` with:
   - Email: `admin@nadafa.com`
   - Password: `Admin123!`
2. **Verify the role** shows "Admin"

### Expected Behaviors

#### Authentication
- ✅ Valid credentials return JWT token
- ✅ Invalid credentials return 401 Unauthorized
- ✅ JWT tokens work for authenticated endpoints
- ✅ Invalid/expired tokens return 401 Unauthorized

#### Password Management
- ✅ Password change requires current password verification
- ✅ New passwords must be confirmed
- ✅ Old passwords become invalid after change

#### Sign Out
- ✅ Signed out tokens become invalid
- ✅ Subsequent requests with signed out token fail
- ✅ Success message confirms sign out

#### Role-Based Access
- ✅ Users can access user-specific endpoints
- ✅ Factories can access factory-specific endpoints
- ✅ Admins can access admin-specific endpoints

### Common Test Cases

#### Error Scenarios
1. **Invalid email format** → Should return validation error
2. **Weak password** → Should return validation error
3. **Mismatched password confirmation** → Should return validation error
4. **Invalid current password** → Should return error
5. **Missing authorization header** → Should return 401
6. **Invalid JWT token** → Should return 401
7. **Using signed out token** → Should return 401

#### Security Scenarios
1. **Password confirmation** → New passwords must be confirmed
2. **Email validation** → Email addresses must be valid format
3. **Password strength** → Passwords must meet minimum requirements
4. **Token blacklisting** → Signed out tokens should be invalid

### Testing Tools
- **Swagger UI**: Interactive API documentation and testing
- **Postman**: Advanced API testing with collections
- **HTTP files**: Use the provided `test-api.http` file
- **cURL**: Command-line API testing

### Notes
- The current implementation uses in-memory storage for demo purposes
- In production, implement proper database storage and password verification
- Consider implementing rate limiting for security endpoints

## 🗄️ Database Migration Guide

### **Do You Need a Database Migration?**

**Answer: NO, no database migration is needed for the password change feature.**

### **Why No Migration is Required:**

1. **Password change uses existing tables**: The password change feature works with the existing `Users` and `Factories` tables
2. **No new tables needed**: Password changes update the existing `PasswordHash` field in the user/factory records
3. **Existing schema is sufficient**: The current database structure already supports password management

### **How Password Change Works:**

1. **User authenticates** with current password
2. **System verifies** current password against stored hash
3. **If correct**, system hashes the new password and updates the database
4. **No additional tables** or schema changes are required

### **If You Need to Create/Update Database Schema:**

#### **Option 1: Use Existing Migrations (Recommended)**
```bash
# Apply existing migrations to create the database
dotnet ef database update --project Infrastructure --startup-project Presentation
```

#### **Option 2: Manual Database Creation**
If you prefer to create the database manually:

1. **Create MySQL Database:**
```sql
CREATE DATABASE NadafaDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

2. **Create MySQL User:**
```sql
CREATE USER 'nadafa_user'@'localhost' IDENTIFIED BY 'your_secure_password';
GRANT ALL PRIVILEGES ON NadafaDB.* TO 'nadafa_user'@'localhost';
FLUSH PRIVILEGES;
```

3. **Update Connection String** in `Presentation/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=NadafaDB;User=nadafa_user;Password=your_secure_password;Port=3306;"
  }
}
```

4. **Apply Migrations:**
```bash
dotnet ef database update --project Infrastructure --startup-project Presentation
```

### **Database Schema Overview:**

The application uses these main tables:
- **Users**: Store user accounts with password hashes
- **Factories**: Store factory accounts with password hashes
- **PickupRequests**: Store pickup requests
- **MarketplaceItems**: Store marketplace items
- **Payments**: Store payment records

All password-related operations work with the existing `PasswordHash` fields in the `Users` and `Factories` tables.

## 🧪 Testing with Swagger

1. Open your browser and navigate to: `https://localhost:5001/swagger`
2. Use the Swagger UI to test all endpoints
3. For protected endpoints, click "Authorize" and enter your JWT token

## 🔧 Common Issues & Troubleshooting

### Build Errors
- **Target Framework Error**: Ensure you have .NET 5.0 SDK installed
- **Package Restore Issues**: Run `dotnet restore` and check internet connection

### Database Connection Issues
- **Connection Refused**: Ensure MySQL service is running
- **Authentication Failed**: Verify username/password in connection string
- **Database Not Found**: Create the database first

### Runtime Errors
- **JWT Configuration**: Ensure `JwtSettings:SecretKey` is at least 32 characters
- **Environment Variables**: Check if `.env` file is in the correct location
- **Port Conflicts**: Change ports in `launchSettings.json` if needed

### MySQL Specific Issues
- **Character Set**: Ensure database uses `utf8mb4` character set
- **User Privileges**: Verify user has all necessary permissions
- **Port**: Default MySQL port is 3306

## 📁 Project Structure

```
Nadafa/
├── Domain/                          # Core business logic
│   ├── Entities/                    # Domain entities
│   │   ├── User.cs                 # User entity
│   │   ├── Factory.cs              # Factory entity
│   │   ├── PickupRequest.cs        # Pickup request entity
│   │   ├── MarketplaceItem.cs      # Marketplace item entity
│   │   ├── Payment.cs              # Payment entity
│   │   └── Role.cs                 # Role enum
│   └── Results/                     # Result objects
├── Application/                      # Application services
│   ├── Contracts/                   # Service interfaces
│   ├── DTOs/                        # Data transfer objects
│   └── Implementations/             # Service implementations
├── Infrastructure/                   # Data access & external services
│   └── Data/                        # Entity Framework context
├── Presentation/                     # API controllers
│   └── Controllers/                 # API endpoints
└── .env                             # Environment variables
```

## 🔒 Security Features

- **JWT Authentication**: Secure token-based authentication
- **Password Hashing**: BCrypt for secure password storage
- **Role-Based Access**: Different permissions for Users, Admins, and Factories
- **Input Validation**: Data annotations for request validation

## 🚀 Next Steps

1. **Implement Pickup Request Management**
2. **Add Admin Dashboard Endpoints**
3. **Integrate Stripe Payment Processing**
4. **Add SendGrid Email Notifications**
5. **Implement Marketplace Browsing**
6. **Add Factory Verification System**

## 📞 Support

For issues and questions:
1. Check the troubleshooting section above
2. Review the Swagger documentation
3. Check the application logs for detailed error messages

## 📝 License

This project is licensed under the MIT License.

## 🧭 Commands & Scenarios (EF Core + MySQL)

### 1) One-time MySQL setup (run in MySQL client, not PowerShell)
```sql
CREATE DATABASE NadafaDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER 'nadafa_user'@'localhost' IDENTIFIED BY 'your_secure_password';
GRANT ALL PRIVILEGES ON NadafaDB.* TO 'nadafa_user'@'localhost';
FLUSH PRIVILEGES;
```

Then set connection string in `Presentation/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=NadafaDB;User=nadafa_user;Password=your_secure_password;Port=3306;"
}
```

### 2) Create a migration (always target Infrastructure, start Presentation)
Run from repo root:
```powershell
# Add migration files into Infrastructure/Data/Migrations
 dotnet ef migrations add <MigrationName> ^
  --project Infrastructure ^
  --startup-project Presentation ^
  --context Infrastructure.Data.NadafaDbContext ^
  --output-dir Data/Migrations
```
Alternative (run from Infrastructure directory):
```powershell
cd Infrastructure
 dotnet ef migrations add <MigrationName> ^
  --startup-project ../Presentation ^
  --context Infrastructure.Data.NadafaDbContext ^
  --output-dir Data/Migrations
```

### 3) Apply migrations to DB
From repo root:
```powershell
 dotnet ef database update ^
  --project Infrastructure ^
  --startup-project Presentation ^
  --context Infrastructure.Data.NadafaDbContext
```
Or from Infrastructure:
```powershell
cd Infrastructure
 dotnet ef database update ^
  --startup-project ../Presentation ^
  --context Infrastructure.Data.NadafaDbContext
```

### 4) Common scenarios and what to do
- New/changed entities or Fluent configurations:
  - Run “Create a migration” then “Apply migrations to DB”.
- Got error: “Your target project 'Presentation' doesn't match your migrations assembly 'Infrastructure'”:
  - Use the exact flags above (project=Infrastructure, startup=Presentation, context full name).
- Got error: “Access denied for user 'root'@'localhost'”:
  - Fix MySQL credentials and connection string; prefer a dedicated user (e.g., `nadafa_user`).
- Switching machines/environments:
  - Confirm MySQL installed and running, update connection string, then run “Apply migrations to DB”.
- Want to reset DB schema (dev only):
  - Drop DB in MySQL, recreate using section 1, then re-run “Apply migrations to DB”.

### 5) Quick run commands
```powershell
# Build everything
 dotnet build

# Run API
 dotnet run -p Presentation

# Swagger
# https://localhost:5001/swagger
```
