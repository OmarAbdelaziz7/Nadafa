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
