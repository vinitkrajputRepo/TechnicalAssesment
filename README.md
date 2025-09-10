# Products API - Technical Assessment

A RESTful backend API solution built with .NET 8, ASP.NET Core Web API, and Entity Framework Core for performing CRUD operations on Products and Items.

## 🏗️ Architecture

This solution follows Clean Architecture principles with a layered approach:

- **Domain Layer**: Core business entities and logic
- **Application Layer**: Business services, DTOs, and validation
- **Infrastructure Layer**: Data access, external services, and configuration
- **API Layer**: Controllers, middleware, and HTTP handling

## 🚀 Tech Stack

- **Framework**: .NET 8 with C#
- **API Framework**: ASP.NET Core Web API
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT with refresh token strategy (ready for implementation)
- **Testing**: xUnit, Moq, and WebApplicationFactory
- **Documentation**: Swagger/OpenAPI with Swashbuckle
- **Containerization**: Docker and Docker Compose
- **Logging**: Serilog for structured logging
- **Validation**: FluentValidation
- **Mapping**: AutoMapper

## 📊 Database Structure

The solution implements the following database schema:

### Product Table
```sql
CREATE TABLE [dbo].[Products]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
    [ProductName] NVARCHAR(255) NOT NULL,
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [CreatedOn] DATETIME NOT NULL,
    [ModifiedBy] NVARCHAR(100) NULL,
    [ModifiedOn] DATETIME NULL
)
```

### Item Table
```sql
CREATE TABLE [dbo].[Item]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
    [ProductId] INT NOT NULL FOREIGN KEY REFERENCES Product(Id),
    [Quantity] INT NOT NULL
)
```

## 📁 Project Structure

```
Solution/
├── src/
│   ├── API/                  # ASP.NET Core Web API project
│   │   ├── Controllers/      # API controllers
│   │   ├── Filters/          # Action filters for cross-cutting concerns
│   │   ├── Middleware/       # Custom middleware components
│   │   ├── Extensions/       # Extension methods for DI and app configuration
│   │   ├── Program.cs        # Application entry point and configuration
│   │   └── appsettings.json  # Configuration files
│   ├── Application/          # Application logic layer
│   │   ├── DTOs/             # Data Transfer Objects
│   │   ├── Interfaces/       # Service interfaces
│   │   ├── Mapping/          # Object mapping profiles
│   │   ├── Services/         # Service implementations
│   │   └── Validators/       # Request validation rules
│   ├── Domain/               # Domain layer
│   │   ├── Entities/         # Domain models
│   │   ├── Enums/            # Enumeration types
│   │   ├── Events/           # Domain events
│   │   └── Exceptions/       # Custom domain exceptions
│   └── Infrastructure/       # Infrastructure layer
│       ├── Data/             # Data access components
│       │   ├── Configurations/  # Entity type configurations
│       │   ├── Repositories/    # Repository implementations
│       │   ├── ApplicationDbContext.cs  # EF Core DbContext
│       │   └── UnitOfWork.cs    # Unit of Work implementation
│       ├── Identity/          # Authentication and authorization
│       ├── Logging/           # Logging infrastructure
│       └── Services/          # External service integrations
├── tests/
│   ├── API.Tests/            # Integration tests for API
│   ├── Application.Tests/    # Unit tests for application layer
│   └── Infrastructure.Tests/ # Unit tests for infrastructure layer
└── docker-compose.yml        # Docker Compose configuration
```

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB, SQL Server Express, or SQL Server)
- Docker and Docker Compose (optional)

### Local Development

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Technical-Assessment
   ```

2. **Update connection string** in `src/API/appsettings.json`
   ```json
   "ConnectionStrings": {
  "DefaultConnection": "Server=DESKTOP-KNKB0H7;Database=ProductsDB;Trusted_Connection=true;TrustServerCertificate=True;MultipleActiveResultSets=true" //it is not prefered way to store connection string , we can use azure key valut or secret.json
}
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Run the application**
   ```bash
   dotnet run --project src/API
   ```

5. **Access the API**
   - API: https://localhost:7001
   - Swagger UI: https://localhost:7001

### Using Docker

1. **Build and run with Docker Compose**
   ```bash
   docker-compose up --build
   ```

2. **Access the application**
   - API: http://localhost:5000
   - Swagger UI: http://localhost:5000

## 📚 API Documentation

### Base URL
```
https://localhost:7001/api/products
```

### Endpoints

#### Get All Products
```http
GET /api/products?pageNumber=1&pageSize=10&searchTerm=electronics
```

**Query Parameters:**
- `pageNumber` (optional): Page number (default: 1)
- `pageSize` (optional): Page size (default: 10, max: 100)
- `searchTerm` (optional): Search in product name or created by

#### Get Product by ID
```http
GET /api/products/{id}
```

#### Create Product
```http
POST /api/products
Content-Type: application/json

{
  "productName": "Product Name",
  "createdBy": "John Doe"
}
```

#### Update Product
```http
PUT /api/products/{id}
Content-Type: application/json

{
  "productName": "Updated Product Name",
  "modifiedBy": "Jane Doe"
}
```

#### Delete Product
```http
DELETE /api/products/{id}
```

### Response Format

#### Success Response
```json
{
  "id": 1,
  "productName": "Product Name",
  "createdBy": "John Doe",
  "createdOn": "2024-01-01T00:00:00Z",
  "modifiedBy": "Jane Doe",
  "modifiedOn": "2024-01-01T12:00:00Z"
}
```

#### Error Response
```json
{
  "message": "Product with ID 999 not found.",
  "errorType": "DomainError",
  "statusCode": 404,
  "timestamp": "2024-01-01T00:00:00Z"
}
```

## 🧪 Testing

### Run Tests
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/API.Tests/
dotnet test tests/Application.Tests/
dotnet test tests/Infrastructure.Tests/
```

### Test Coverage
The solution includes:
- **Unit Tests**: Application and Infrastructure layers
- **Integration Tests**: API endpoints with mocked services
- **Test Data**: Sample data for testing scenarios

## 🔧 Configuration

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Application environment (Development/Staging/Production)
- `ConnectionStrings__DefaultConnection`: Database connection string

### Logging
- **Console Logging**: Development environment
- **File Logging**: Daily rolling log files in `Logs/` directory
- **Structured Logging**: Using Serilog with JSON formatting

## 🚀 Deployment

### Docker Deployment
```bash
# Build image
docker build -t products-api .

# Run container
docker run -p 5000:80 products-api
```

### Azure Deployment
The solution is ready for Azure deployment with:
- Azure App Service
- Azure SQL Database
- Azure Container Registry

## 🔒 Security Features

- **Input Validation**: FluentValidation for request validation
- **Exception Handling**: Custom middleware for consistent error responses
- **CORS**: Configurable Cross-Origin Resource Sharing
- **Authentication Ready**: JWT authentication infrastructure in place

## 📊 Performance Features

- **Pagination**: Efficient data retrieval with page-based results
- **Search & Filtering**: Optimized database queries with indexes
- **Async Operations**: Full async/await pattern throughout the stack
- **Unit of Work**: Transaction management for data consistency
- **Entity Relationships**: Proper foreign key relationships between Product and Item

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## 📄 License

This project is part of a technical assessment and is provided as-is.

## 🆘 Support

For questions or issues, please contact the development team or create an issue in the repository.
