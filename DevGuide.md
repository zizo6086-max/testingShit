
# Developer Guide: UserZone API Project

## Table of Contents
1. [Project Overview](#project-overview)
2. [Architecture & Structure](#architecture--structure)
3. [Adding New Features](#adding-new-features)
4. [Configuration Management](#configuration-management)
5. [Service Layer Development](#service-layer-development)
6. [API Controllers](#api-controllers)
7. [Validation & Error Handling](#validation--error-handling)
8. [Database & Repositories](#database--repositories)
9. [Authentication & Authorization](#authentication--authorization)
10. [Testing & Deployment](#testing--deployment)
11. [Common Patterns & Best Practices](#common-patterns--best-practices)

---

## Project Overview

**UserZone API** is an ASP.NET Core 9.0 application built with Clean Architecture principles. It provides user authentication, profile management, and file upload capabilities with JWT-based security.

### Key Features
- JWT-based authentication with refresh tokens
- Email verification system
- File upload/download management
- Role-based authorization (User, Admin, Seller)
- Google OAuth integration
- Comprehensive logging and error handling

---

## Architecture & Structure

### Solution Structure
```
UserZone.sln
├── API/                    # Presentation Layer
├── Application/           # Business Logic Layer
├── Domain/                # Domain Models & Constants
└── Infrastructure/        # Data Access & External Services
```

### Layer Responsibilities

#### **Domain Layer** (`Domain/`)
- **Models**: Business entities (`AppUser`, `RefreshToken`)
- **Constants**: Application constants (`AuthConstants`, `ValidationConstants`)
- **Interfaces**: Domain contracts

#### **Application Layer** (`Application/`)
- **Services**: Business logic implementation
- **Interfaces**: Service contracts
- **DTOs**: Data transfer objects
- **Validators**: Input validation using FluentValidation
- **Extensions**: Utility methods and extensions

#### **Infrastructure Layer** (`Infrastructure/`)
- **Data**: Database context, repositories, configurations
- **Services**: External service implementations
- **Configuration**: Strongly-typed configuration classes
- **Email**: Email templates and services

#### **API Layer** (`API/`)
- **Controllers**: HTTP endpoint handlers
- **Middleware**: Cross-cutting concerns
- **Filters**: Request/response processing

---

## Adding New Features

### 1. Create Feature Structure

When adding a new feature (e.g., "Products"), follow this structure:

```
Application/
├── Features/
│   └── Products/
│       ├── DTOs/
│       │   ├── CreateProductDto.cs
│       │   ├── UpdateProductDto.cs
│       │   └── ProductDto.cs
│       ├── Interfaces/
│       │   └── IProductService.cs
│       ├── Services/
│       │   └── ProductService.cs
│       └── Validators/
│           ├── CreateProductDtoValidator.cs
│           └── UpdateProductDtoValidator.cs

Infrastructure/
├── Data/
│   ├── Configurations/
│   │   └── ProductConfiguration.cs
│   └── Repositories/
│       ├── IProductRepository.cs
│       └── ProductRepository.cs
```

### 2. Create Domain Model

```csharp
// Domain/Models/Product.cs
namespace Domain.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
```

### 3. Create DTOs

```csharp
// Application/Features/Products/DTOs/CreateProductDto.cs
namespace Application.Features.Products.DTOs;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// Application/Features/Products/DTOs/ProductDto.cs
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
```

### 4. Create Service Interface

```csharp
// Application/Features/Products/Interfaces/IProductService.cs
namespace Application.Features.Products.Interfaces;

public interface IProductService
{
    Task<ProductDto> GetByIdAsync(int id);
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto> CreateAsync(CreateProductDto createDto);
    Task<ProductDto> UpdateAsync(int id, UpdateProductDto updateDto);
    Task<bool> DeleteAsync(int id);
}
```

### 5. Create Service Implementation

```csharp
// Application/Features/Products/Services/ProductService.cs
namespace Application.Features.Products.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        logger.LogServiceOperation("Get product by ID", id.ToString());
        
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new NotFoundException($"Product with ID {id} not found");

        return MapToDto(product);
    }

    // Implement other methods...
}
```

### 6. Create Validators

```csharp
// Application/Features/Products/Validators/CreateProductDtoValidator.cs
namespace Application.Features.Products.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Product name is required and must not exceed 100 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");
    }
}
```

### 7. Create Repository Interface

```csharp
// Infrastructure/Data/Repositories/IProductRepository.cs
namespace Infrastructure.Data.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<List<Product>> GetActiveProductsAsync();
    Task<Product?> GetByNameAsync(string name);
}
```

### 8. Create Repository Implementation

```csharp
// Infrastructure/Data/Repositories/ProductRepository.cs
namespace Infrastructure.Data.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Product>> GetActiveProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .ToListAsync();
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Name == name);
    }
}
```

### 9. Create Controller

```csharp
// API/Controllers/ProductsController.cs
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Seller")]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = await _productService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Seller")]
    public async Task<ActionResult<ProductDto>> Update(int id, UpdateProductDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = await _productService.UpdateAsync(id, updateDto);
        return Ok(product);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _productService.DeleteAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
```

### 10. Register Dependencies

```csharp
// Application/DependencyInjection.cs
public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
{
    // Existing registrations...
    
    // Add new service
    services.AddScoped<IProductService, ProductService>();
    
    return services;
}

// Infrastructure/DependencyInjection.cs
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
{
    // Existing registrations...
    
    // Add new repository
    services.AddScoped<IProductRepository, ProductRepository>();
    
    return services;
}
```

---

## Configuration Management

### 1. Create Configuration Class

```csharp
// Infrastructure/Configuration/ProductSettings.cs
namespace Infrastructure.Configuration;

public class ProductSettings
{
    public const string SectionName = "Products";
    
    public int MaxProductsPerPage { get; set; } = 20;
    public bool EnableCaching { get; set; } = true;
    public string DefaultImagePath { get; set; } = "/images/products/default.jpg";
}
```

### 2. Register Configuration

```csharp
// Infrastructure/DependencyInjection.cs
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
{
    // Existing configurations...
    services.Configure<ProductSettings>(configuration.GetSection(ProductSettings.SectionName));
    
    return services;
}
```

### 3. Use Configuration in Service

```csharp
public class ProductService : IProductService
{
    private readonly ProductSettings _settings;
    
    public ProductService(IOptions<ProductSettings> settings)
    {
        _settings = settings.Value;
    }
    
    public async Task<List<ProductDto>> GetPaginatedAsync(int page, int pageSize)
    {
        var maxPageSize = _settings.MaxProductsPerPage;
        var actualPageSize = Math.Min(pageSize, maxPageSize);
        
        // Implementation...
    }
}
```

### 4. Add to appsettings.json

```json
{
  "Products": {
    "MaxProductsPerPage": 25,
    "EnableCaching": true,
    "DefaultImagePath": "/images/products/default.jpg"
  }
}
```

---

## Service Layer Development

### Service Interface Pattern

```csharp
public interface IExampleService
{
    // Query methods
    Task<ExampleDto> GetByIdAsync(int id);
    Task<List<ExampleDto>> GetAllAsync();
    
    // Command methods
    Task<ExampleDto> CreateAsync(CreateExampleDto createDto);
    Task<ExampleDto> UpdateAsync(int id, UpdateExampleDto updateDto);
    Task<bool> DeleteAsync(int id);
    
    // Business logic methods
    Task<bool> ValidateBusinessRuleAsync(int id);
}
```

### Service Implementation Pattern

```csharp
public class ExampleService : IExampleService
{
    private readonly IExampleRepository _repository;
    private readonly ILogger<ExampleService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ExampleService(
        IExampleRepository repository,
        ILogger<ExampleService> logger,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<ExampleDto> CreateAsync(CreateExampleDto createDto)
    {
        try
        {
            logger.LogServiceOperation("Creating example", createDto.Name);
            
            // Business logic validation
            if (await _repository.ExistsAsync(createDto.Name))
                throw new BusinessRuleException("Example with this name already exists");
            
            // Create entity
            var entity = new Example
            {
                Name = createDto.Name,
                CreatedAt = DateTime.UtcNow
            };
            
            // Save to database
            await _repository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            
            logger.LogUserAction("Created example", entity.Id.ToString());
            
            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create example: {Name}", createDto.Name);
            throw;
        }
    }
}
```

---

## API Controllers

### Controller Pattern

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExamplesController : ControllerBase
{
    private readonly IExampleService _service;
    private readonly ILogger<ExamplesController> _logger;

    public ExamplesController(
        IExampleService service,
        ILogger<ExamplesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // GET endpoints
    [HttpGet]
    public async Task<ActionResult<List<ExampleDto>>> GetAll()
    {
        var examples = await _service.GetAllAsync();
        return Ok(examples);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExampleDto>> GetById(int id)
    {
        var example = await _service.GetByIdAsync(id);
        return Ok(example);
    }

    // POST endpoints
    [HttpPost]
    public async Task<ActionResult<ExampleDto>> Create(CreateExampleDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var example = await _service.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = example.Id }, example);
    }

    // PUT endpoints
    [HttpPut("{id}")]
    public async Task<ActionResult<ExampleDto>> Update(int id, UpdateExampleDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var example = await _service.UpdateAsync(id, updateDto);
        return Ok(example);
    }

    // DELETE endpoints
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
```

### Authorization Patterns

```csharp
// Role-based authorization
[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public async Task<ActionResult> Delete(int id) { }

// Policy-based authorization
[Authorize(Policy = "CanManageProducts")]
[HttpPut("{id}")]
public async Task<ActionResult> Update(int id, UpdateDto dto) { }

// Anonymous endpoints
[AllowAnonymous]
[HttpGet("public")]
public ActionResult GetPublicData() { }
```

---

## Validation & Error Handling

### FluentValidation Pattern

```csharp
public class CreateExampleDtoValidator : AbstractValidator<CreateExampleDto>
{
    public CreateExampleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Name is required and must not exceed 100 characters");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Please provide a valid email address");

        RuleFor(x => x.Age)
            .InclusiveBetween(18, 100)
            .WithMessage("Age must be between 18 and 100");
    }
}
```

### Custom Validation Rules

```csharp
public class CreateExampleDtoValidator : AbstractValidator<CreateExampleDto>
{
    public CreateExampleDtoValidator(IExampleRepository repository)
    {
        RuleFor(x => x.Name)
            .MustAsync(async (name, cancellation) =>
            {
                var exists = await repository.ExistsAsync(name);
                return !exists;
            })
            .WithMessage("Name must be unique");
    }
}
```

### Error Handling Pattern

```csharp
public async Task<ExampleDto> GetByIdAsync(int id)
{
    try
    {
        var example = await _repository.GetByIdAsync(id);
        if (example == null)
            throw new NotFoundException($"Example with ID {id} not found");

        return MapToDto(example);
    }
    catch (NotFoundException)
    {
        // Re-throw business exceptions
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to get example by ID: {Id}", id);
        throw new ApplicationException("An error occurred while retrieving the example");
    }
}
```

---

## Database & Repositories

### Entity Configuration

```csharp
// Infrastructure/Data/Configurations/ExampleConfiguration.cs
namespace Infrastructure.Data.Configurations;

public class ExampleConfiguration : IEntityTypeConfiguration<Example>
{
    public void Configure(EntityTypeBuilder<Example> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        
        builder.HasIndex(e => e.Name)
            .IsUnique();
        
        builder.HasQueryFilter(e => e.IsActive);
    }
}
```

### Repository Pattern

```csharp
public interface IExampleRepository : IGenericRepository<Example>
{
    Task<Example?> GetByNameAsync(string name);
    Task<List<Example>> GetActiveAsync();
    Task<bool> ExistsAsync(string name);
}

public class ExampleRepository : GenericRepository<Example>, IExampleRepository
{
    public ExampleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Example?> GetByNameAsync(string name)
    {
        return await _context.Examples
            .FirstOrDefaultAsync(e => e.Name == name);
    }

    public async Task<List<Example>> GetActiveAsync()
    {
        return await _context.Examples
            .Where(e => e.IsActive)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(string name)
    {
        return await _context.Examples
            .AnyAsync(e => e.Name == name);
    }
}
```

### Unit of Work Pattern

```csharp
public interface IUnitOfWork
{
    IExampleRepository Examples { get; }
    // Other repositories...
    
    Task<int> CommitAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Examples = new ExampleRepository(context);
    }
    
    public IExampleRepository Examples { get; }
    
    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}
```

---

## Authentication & Authorization

### JWT Configuration

```csharp
// Infrastructure/DependencyInjection.cs
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
    };
});
```

### Custom Authorization Policies

```csharp
// Infrastructure/DependencyInjection.cs
services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageProducts", policy =>
        policy.RequireRole("Admin", "Seller"));
    
    options.AddPolicy("CanDeleteProducts", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("CanViewAnalytics", policy =>
        policy.RequireRole("Admin", "Manager"));
});
```

### Role-Based Access Control

```csharp
// Domain/Constants/AuthConstants.cs
public static class AuthConstants
{
    public static class Roles
    {
        public const string User = "User";
        public const string Admin = "Admin";
        public const string Seller = "Seller";
        public const string Manager = "Manager";
    }
}
```

---

## Testing & Deployment

### Unit Testing Pattern

```csharp
// Tests/Application/Features/Products/Services/ProductServiceTests.cs
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<ProductService>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        
        _service = new ProductService(
            _mockRepository.Object,
            _mockLogger.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        var productId = 1;
        var product = new Product { Id = productId, Name = "Test Product" };
        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Test Product", result.Name);
    }
}
```

### Integration Testing

```csharp
// Tests/API/Controllers/ProductsControllerTests.cs
public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
```

---

## Common Patterns & Best Practices

### 1. Naming Conventions

- **Controllers**: `{Feature}Controller` (e.g., `ProductsController`)
- **Services**: `{Feature}Service` (e.g., `ProductService`)
- **Interfaces**: `I{Feature}Service` (e.g., `IProductService`)
- **DTOs**: `{Action}{Feature}Dto` (e.g., `CreateProductDto`, `UpdateProductDto`)
- **Validators**: `{Action}{Feature}DtoValidator` (e.g., `CreateProductDtoValidator`)

### 2. Error Handling

```csharp
// Application/Common/Exceptions/NotFoundException.cs
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

// Application/Common/Exceptions/BusinessRuleException.cs
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}
```

### 3. Logging Patterns

```csharp
// Use structured logging
logger.LogInformation("User {UserId} performed {Action} on {Resource}", 
    userId, action, resource);

// Log errors with context
logger.LogError(ex, "Failed to {Action} {Resource} for user {UserId}", 
    action, resource, userId);
```

### 4. Async/Await Best Practices

```csharp
// Always use async/await
public async Task<ExampleDto> GetByIdAsync(int id)
{
    var example = await _repository.GetByIdAsync(id);
    return MapToDto(example);
}

// Use ConfigureAwait(false) for library code
public async Task<ExampleDto> GetByIdAsync(int id)
{
    var example = await _repository.GetByIdAsync(id).ConfigureAwait(false);
    return MapToDto(example);
}
```

### 5. Dependency Injection

```csharp
// Register services with appropriate lifetime
services.AddScoped<IProductService, ProductService>();        // Per request
services.AddTransient<IValidator, Validator>();               // Per injection
services.AddSingleton<ICacheService, CacheService>();         // Per application
```

---

## Quick Reference

### Adding New Feature Checklist

- [ ] Create domain model in `Domain/Models/`
- [ ] Create DTOs in `Application/Features/{Feature}/DTOs/`
- [ ] Create service interface in `Application/Features/{Feature}/Interfaces/`
- [ ] Create service implementation in `Application/Features/{Feature}/Services/`
- [ ] Create validators in `Application/Features/{Feature}/Validators/`
- [ ] Create repository interface in `Infrastructure/Data/Repositories/`
- [ ] Create repository implementation in `Infrastructure/Data/Repositories/`
- [ ] Create controller in `API/Controllers/`
- [ ] Register dependencies in `DependencyInjection.cs`
- [ ] Add configuration if needed
- [ ] Write tests
- [ ] Update documentation

### Common Commands

```bash
# Build solution
dotnet build

# Run tests
dotnet test

# Run API
dotnet run --project API

# Add new package
dotnet add package PackageName

# Add project reference
dotnet add reference ../ProjectPath/Project.csproj
```

---

This guide covers the essential patterns and practices for extending the UserZone API project. Follow these patterns to maintain consistency and code quality as the project grows.

```plaintext
UserZone.sln
├── API/                    # Presentation Layer
├── Application/           # Business Logic Layer
├── Domain/                # Domain Models & Constants
└── Infrastructure/        # Data Access & External Services
```

```plaintext
Application/
├── Features/
│   └── Products/
│       ├── DTOs/
│       │   ├── CreateProductDto.cs
│       │   ├── UpdateProductDto.cs
│       │   └── ProductDto.cs
│       ├── Interfaces/
│       │   └── IProductService.cs
│       ├── Services/
│       │   └── ProductService.cs
│       └── Validators/
│           ├── CreateProductDtoValidator.cs
│           └── UpdateProductDtoValidator.cs

Infrastructure/
├── Data/
│   ├── Configurations/
│   │   └── ProductConfiguration.cs
│   └── Repositories/
│       ├── IProductRepository.cs
│       └── ProductRepository.cs
```

```csharp
// Domain/Models/Product.cs
namespace Domain.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
```

```csharp
// Application/Features/Products/DTOs/CreateProductDto.cs
namespace Application.Features.Products.DTOs;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// Application/Features/Products/DTOs/ProductDto.cs
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
```

```csharp
// Application/Features/Products/Interfaces/IProductService.cs
namespace Application.Features.Products.Interfaces;

public interface IProductService
{
    Task<ProductDto> GetByIdAsync(int id);
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto> CreateAsync(CreateProductDto createDto);
    Task<ProductDto> UpdateAsync(int id, UpdateProductDto updateDto);
    Task<bool> DeleteAsync(int id);
}
```

```csharp
// Application/Features/Products/Services/ProductService.cs
namespace Application.Features.Products.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        logger.LogServiceOperation("Get product by ID", id.ToString());
        
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new NotFoundException($"Product with ID {id} not found");

        return MapToDto(product);
    }

    // Implement other methods...
}
```

```csharp
// Application/Features/Products/Validators/CreateProductDtoValidator.cs
namespace Application.Features.Products.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Product name is required and must not exceed 100 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");
    }
}
```

```csharp
// Infrastructure/Data/Repositories/IProductRepository.cs
namespace Infrastructure.Data.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<List<Product>> GetActiveProductsAsync();
    Task<Product?> GetByNameAsync(string name);
}
```

```csharp
// Infrastructure/Data/Repositories/ProductRepository.cs
namespace Infrastructure.Data.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Product>> GetActiveProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .ToListAsync();
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Name == name);
    }
}
```

```csharp
// API/Controllers/ProductsController.cs
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Seller")]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = await _productService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Seller")]
    public async Task<ActionResult<ProductDto>> Update(int id, UpdateProductDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = await _productService.UpdateAsync(id, updateDto);
        return Ok(product);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _productService.DeleteAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
```

```csharp
// Application/DependencyInjection.cs
public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
{
    // Existing registrations...
    
    // Add new service
    services.AddScoped<IProductService, ProductService>();
    
    return services;
}

// Infrastructure/DependencyInjection.cs
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
{
    // Existing registrations...
    
    // Add new repository
    services.AddScoped<IProductRepository, ProductRepository>();
    
    return services;
}
```

```csharp
// Infrastructure/Configuration/ProductSettings.cs
namespace Infrastructure.Configuration;

public class ProductSettings
{
    public const string SectionName = "Products";
    
    public int MaxProductsPerPage { get; set; } = 20;
    public bool EnableCaching { get; set; } = true;
    public string DefaultImagePath { get; set; } = "/images/products/default.jpg";
}
```

```csharp
// Infrastructure/DependencyInjection.cs
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
{
    // Existing configurations...
    services.Configure<ProductSettings>(configuration.GetSection(ProductSettings.SectionName));
    
    return services;
}
```

```csharp
public class ProductService : IProductService
{
    private readonly ProductSettings _settings;
    
    public ProductService(IOptions<ProductSettings> settings)
    {
        _settings = settings.Value;
    }
    
    public async Task<List<ProductDto>> GetPaginatedAsync(int page, int pageSize)
    {
        var maxPageSize = _settings.MaxProductsPerPage;
        var actualPageSize = Math.Min(pageSize, maxPageSize);
        
        // Implementation...
    }
}
```

```json
{
  "Products": {
    "MaxProductsPerPage": 25,
    "EnableCaching": true,
    "DefaultImagePath": "/images/products/default.jpg"
  }
}
```

```csharp
public interface IExampleService
{
    // Query methods
    Task<ExampleDto> GetByIdAsync(int id);
    Task<List<ExampleDto>> GetAllAsync();
    
    // Command methods
    Task<ExampleDto> CreateAsync(CreateExampleDto createDto);
    Task<ExampleDto> UpdateAsync(int id, UpdateExampleDto updateDto);
    Task<bool> DeleteAsync(int id);
    
    // Business logic methods
    Task<bool> ValidateBusinessRuleAsync(int id);
}
```

```csharp
public class ExampleService : IExampleService
{
    private readonly IExampleRepository _repository;
    private readonly ILogger<ExampleService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ExampleService(
        IExampleRepository repository,
        ILogger<ExampleService> logger,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<ExampleDto> CreateAsync(CreateExampleDto createDto)
    {
        try
        {
            logger.LogServiceOperation("Creating example", createDto.Name);
            
            // Business logic validation
            if (await _repository.ExistsAsync(createDto.Name))
                throw new BusinessRuleException("Example with this name already exists");
            
            // Create entity
            var entity = new Example
            {
                Name = createDto.Name,
                CreatedAt = DateTime.UtcNow
            };
            
            // Save to database
            await _repository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            
            logger.LogUserAction("Created example", entity.Id.ToString());
            
            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create example: {Name}", createDto.Name);
            throw;
        }
    }
}
```

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExamplesController : ControllerBase
{
    private readonly IExampleService _service;
    private readonly ILogger<ExamplesController> _logger;

    public ExamplesController(
        IExampleService service,
        ILogger<ExamplesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // GET endpoints
    [HttpGet]
    public async Task<ActionResult<List<ExampleDto>>> GetAll()
    {
        var examples = await _service.GetAllAsync();
        return Ok(examples);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExampleDto>> GetById(int id)
    {
        var example = await _service.GetByIdAsync(id);
        return Ok(example);
    }

    // POST endpoints
    [HttpPost]
    public async Task<ActionResult<ExampleDto>> Create(CreateExampleDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var example = await _service.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = example.Id }, example);
    }

    // PUT endpoints
    [HttpPut("{id}")]
    public async Task<ActionResult<ExampleDto>> Update(int id, UpdateExampleDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var example = await _service.UpdateAsync(id, updateDto);
        return Ok(example);
    }

    // DELETE endpoints
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
```

```csharp
// Role-based authorization
[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public async Task<ActionResult> Delete(int id) { }

// Policy-based authorization
[Authorize(Policy = "CanManageProducts")]
[HttpPut("{id}")]
public async Task<ActionResult> Update(int id, UpdateDto dto) { }

// Anonymous endpoints
[AllowAnonymous]
[HttpGet("public")]
public ActionResult GetPublicData() { }
```

```csharp
public class CreateExampleDtoValidator : AbstractValidator<CreateExampleDto>
{
    public CreateExampleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Name is required and must not exceed 100 characters");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Please provide a valid email address");

        RuleFor(x => x.Age)
            .InclusiveBetween(18, 100)
            .WithMessage("Age must be between 18 and 100");
    }
}
```

```csharp
public class CreateExampleDtoValidator : AbstractValidator<CreateExampleDto>
{
    public CreateExampleDtoValidator(IExampleRepository repository)
    {
        RuleFor(x => x.Name)
            .MustAsync(async (name, cancellation) =>
            {
                var exists = await repository.ExistsAsync(name);
                return !exists;
            })
            .WithMessage("Name must be unique");
    }
}
```

```csharp
public async Task<ExampleDto> GetByIdAsync(int id)
{
    try
    {
        var example = await _repository.GetByIdAsync(id);
        if (example == null)
            throw new NotFoundException($"Example with ID {id} not found");

        return MapToDto(example);
    }
    catch (NotFoundException)
    {
        // Re-throw business exceptions
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to get example by ID: {Id}", id);
        throw new ApplicationException("An error occurred while retrieving the example");
    }
}
```

```csharp
// Infrastructure/Data/Configurations/ExampleConfiguration.cs
namespace Infrastructure.Data.Configurations;

public class ExampleConfiguration : IEntityTypeConfiguration<Example>
{
    public void Configure(EntityTypeBuilder<Example> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        
        builder.HasIndex(e => e.Name)
            .IsUnique();
        
        builder.HasQueryFilter(e => e.IsActive);
    }
}
```

```csharp
public interface IExampleRepository : IGenericRepository<Example>
{
    Task<Example?> GetByNameAsync(string name);
    Task<List<Example>> GetActiveAsync();
    Task<bool> ExistsAsync(string name);
}

public class ExampleRepository : GenericRepository<Example>, IExampleRepository
{
    public ExampleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Example?> GetByNameAsync(string name)
    {
        return await _context.Examples
            .FirstOrDefaultAsync(e => e.Name == name);
    }

    public async Task<List<Example>> GetActiveAsync()
    {
        return await _context.Examples
            .Where(e => e.IsActive)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(string name)
    {
        return await _context.Examples
            .AnyAsync(e => e.Name == name);
    }
}
```

```csharp
public interface IUnitOfWork
{
    IExampleRepository Examples { get; }
    // Other repositories...
    
    Task<int> CommitAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Examples = new ExampleRepository(context);
    }
    
    public IExampleRepository Examples { get; }
    
    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}
```

```csharp
// Infrastructure/DependencyInjection.cs
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
    };
});
```

```csharp
// Infrastructure/DependencyInjection.cs
services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageProducts", policy =>
        policy.RequireRole("Admin", "Seller"));
    
    options.AddPolicy("CanDeleteProducts", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("CanViewAnalytics", policy =>
        policy.RequireRole("Admin", "Manager"));
});
```

```csharp
// Domain/Constants/AuthConstants.cs
public static class AuthConstants
{
    public static class Roles
    {
        public const string User = "User";
        public const string Admin = "Admin";
        public const string Seller = "Seller";
        public const string Manager = "Manager";
    }
}
```

```csharp
// Tests/Application/Features/Products/Services/ProductServiceTests.cs
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<ProductService>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        
        _service = new ProductService(
            _mockRepository.Object,
            _mockLogger.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        var productId = 1;
        var product = new Product { Id = productId, Name = "Test Product" };
        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Test Product", result.Name);
    }
}
```

```csharp
// Tests/API/Controllers/ProductsControllerTests.cs
public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
```

```csharp
// Application/Common/Exceptions/NotFoundException.cs
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

// Application/Common/Exceptions/BusinessRuleException.cs
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}
```

```csharp
// Use structured logging
logger.LogInformation("User {UserId} performed {Action} on {Resource}", 
    userId, action, resource);

// Log errors with context
logger.LogError(ex, "Failed to {Action} {Resource} for user {UserId}", 
    action, resource, userId);
```

```csharp
// Always use async/await
public async Task<ExampleDto> GetByIdAsync(int id)
{
    var example = await _repository.GetByIdAsync(id);
    return MapToDto(example);
}

// Use ConfigureAwait(false) for library code
public async Task<ExampleDto> GetByIdAsync(int id)
{
    var example = await _repository.GetByIdAsync(id).ConfigureAwait(false);
    return MapToDto(example);
}
```

```csharp
// Register services with appropriate lifetime
services.AddScoped<IProductService, ProductService>();        // Per request
services.AddTransient<IValidator, Validator>();               // Per injection
services.AddSingleton<ICacheService, CacheService>();         // Per application
```

```shellscript
# Build solution
dotnet build

# Run tests
dotnet test

# Run API
dotnet run --project API

# Add new package
dotnet add package PackageName

# Add project reference
dotnet add reference ../ProjectPath/Project.csproj
```
