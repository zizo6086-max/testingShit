# API Template

A comprehensive, multi-layered ASP.NET Core Web API template with built-in JWT authentication, user management, and file handling capabilities.

## 🏗️ Architecture

This template follows a clean, layered architecture consisting of:

- **API Layer**: Entry point for HTTP requests, controllers, and API configurations
- **Application Layer**: Business logic, DTOs, and services
- **Domain Layer**: Core business entities and models
- **Infrastructure Layer**: Data access, repository implementations, and external service integrations

## ✨ Features

- **Authentication & Authorization**
    - JWT authentication with refresh token support
    - Role-based authorization (User/Admin)
    - User registration and login flows
    - Password change functionality

- **File Management**
    - File upload and storage service
    - Support for photo/image storage
    - Configurable file options

- **Data Access**
    - Entity Framework Core implementation
    - Repository pattern
    - Unit of Work pattern for transaction management
    - Database migrations

- **API Configuration**
    - Dependency injection setup
    - Scalar/OpenAPI documentation
    - Environment-specific configurations

## 🚀 Getting Started

### Prerequisites

- .NET 6.0+ SDK
- SQL Server (or another compatible database)
- Visual Studio 2022 or Visual Studio Code

### Installation

1. Clone the repository
   ```
   git clone https://github.com/Ozzy-ZY/api-template.git
   ```

2. Navigate to the project directory
   ```
   cd api-template
   ```

3. Update the connection string in `appsettings.json` or `appsettings.Development.json`
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

4. Run database migrations
   ```
   dotnet ef database update
   ```

5. Run the application
   ```
   dotnet run --project API
   ```

6. Access the Scalar UI at `https://localhost:7282/Scalar/v1`

## 📁 Project Structure

```
API_Template/
├── API/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── TestController.cs
│   │   └── UserController.cs
│   ├── wwwroot/
│   │   └── uploads/
│   │       └── Users/
│   ├── appsettings.json
│   └── Program.cs
│   
│   
│
├── Application/
│   ├── DTOs/
│   │   ├── AuthResult.cs
│   │   ├── ChangePasswordDto.cs
│   │   ├── LoginDto.cs
│   │   ├── PhotoOptions.cs
│   │   ├── RegisterDto.cs
│   │   ├── Result.cs
│   │   └── UserDto.cs
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── JwtTokenService.cs
│   │   ├── PhotoService.cs
│   │   └── UserService.cs
│   └── Validators/
│       └── DependencyInjection.cs
│
├── Domain/
│   ├── Models/
│   │   ├── AppUser.cs
│   │   └── RefreshToken.cs
│   └── ModelsConfig/
│       ├── AppUserConfig.cs
│       ├── RolesSeeding.cs
│       └── UserRolesSeeding.cs
│
└── Infrastructure/
    └── DataAccess/
        ├── Repositories/
        │   ├── AppDbContext.cs
        │   ├── AppDbContextFactory.cs
        │   ├── IUnitOfWork.cs
        │   └── UnitOfWork.cs
        └── Migrations/
            └── DependencyInjection.cs
```

## 🔐 Authentication Flow

1. **Register**: Create a new user account
   ```http
   POST /api/auth/RegisterUser
   ```

2. **Login**: Authenticate and receive JWT token
   ```http
   POST /api/auth/loginUser
   ```

3. **Refresh Token**: Get a new JWT using refresh token
   ```http
   POST /api/auth/RefreshToken
   ```

4. **Change Password**: Update user password
   ```http
   POST /api/auth/Logout
   ```

## 🛠️ Customization

### Adding New Controllers

1. Create a new controller class in the `API/Controllers` directory
2. Inherit from `ControllerBase` and add appropriate route attributes
3. Implement required endpoint methods

### Adding New Models

1. Define entity classes in the `Domain/Models` directory
2. Create configuration classes in the `Domain/ModelsConfig` directory
3. Register entities in the `AppDbContext`
4. Generate and apply migrations

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the project
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request
