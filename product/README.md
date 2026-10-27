# ProductApi

A minimal ASP.NET Core Web API built with .NET 10, demonstrating CRUD operations with LINQ queries and RESTful endpoints.

## Overview

ProductApi is a sample project showcasing best practices for building a RESTful API in ASP.NET Core. It includes product management functionality with in-memory data storage, dependency injection, and Swagger/OpenAPI documentation.

## Features

- ✅ **CRUD Operations**: Create, Read, Update, Delete products
- ✅ **RESTful Endpoints**: Clean, standard HTTP methods and status codes
- ✅ **LINQ Queries**: Efficient data filtering and manipulation
- ✅ **Swagger UI**: Interactive API documentation and testing
- ✅ **Dependency Injection**: Service-based architecture
- ✅ **In-Memory Storage**: Singleton pattern for data persistence across requests
- ✅ **Input Validation**: Data annotations for product validation
- ✅ **Error Handling**: Proper HTTP response codes

## Technologies

- **.NET 10**
- **ASP.NET Core Web API**
- **Swagger/Swashbuckle** (API documentation)
- **LINQ** (data queries)
- **C# 13**

## Prerequisites

- **.NET SDK 10.0** or later ([download](https://dotnet.microsoft.com/download/dotnet))
- **Visual Studio 2026** (Community Edition or higher) or **Visual Studio Code**
- **Git** (for cloning the repo)

## Project Structure

```
ProductApi/
├── Controllers/
│   └── ProductsController.cs       # API endpoints
├── DTOs/
│   └── ProductDtos.cs              # Data Transfer Objects
├── Models/
│   └── Product.cs                  # Product model
├── Services/
│   ├── IProductService.cs          # Service interface
│   └── ProductService.cs           # Service implementation
├── Properties/
│   └── launchSettings.json         # Launch configuration
├── appsettings.json                # Configuration
├── appsettings.Development.json    # Dev configuration
├── Program.cs                      # Application entry point
├── ProductApi.csproj               # Project file
├── ProductApi.http                 # HTTP test file
└── README.md                       # This file
```

## Installation

### 1. Clone the repository
```bash
git clone https://github.com/SangitaZare/angular-trainning.git
cd angular-trainning/ProductApi
```

### 2. Restore dependencies
```bash
dotnet restore
```

### 3. Build the project
```bash
dotnet build
```

## Running the Project

### Option 1: Command Line
```bash
dotnet run
```

The API will start at `https://localhost:5001` (or the port in launchSettings.json).

### Option 2: Visual Studio
1. Open `ProductApi.slnx` in Visual Studio
2. Press `F5` or click **Run** (Debug) / **Start Without Debugging** (Release)
3. The browser will open to Swagger UI automatically

### Option 3: Visual Studio Code
```bash
dotnet run
```
Then open `https://localhost:5001/swagger` in your browser.

## API Endpoints

### Get All Products
```http
GET /api/products
```
**Response (200 OK):**
```json
[
  {
	"id": 1,
	"name": "Product 1",
	"category": "Electronics",
	"price": 99.99,
	"stock": 100
  }
]
```

### Get Product by ID
```http
GET /api/products/{id}
```
**Example:** `GET /api/products/1`

**Response (200 OK):**
```json
{
  "id": 1,
  "name": "Product 1",
  "category": "Electronics",
  "price": 99.99,
  "stock": 100
}
```

### Create Product
```http
POST /api/products
Content-Type: application/json

{
  "name": "New Product",
  "category": "Books",
  "price": 19.99,
  "stock": 50
}
```

**Response (201 Created):**
```json
{
  "id": 3,
  "name": "New Product",
  "category": "Books",
  "price": 19.99,
  "stock": 50
}
```

### Update Product
```http
PUT /api/products/{id}
Content-Type: application/json

{
  "name": "Updated Product",
  "category": "Books",
  "price": 24.99,
  "stock": 45
}
```

**Response (200 OK):** Updated product object

### Delete Product
```http
DELETE /api/products/{id}
```

**Example:** `DELETE /api/products/1`

**Response (204 No Content)** or **404 Not Found**

## Testing with Swagger UI

1. Run the project: `dotnet run`
2. Open browser: `https://localhost:5001/swagger`
3. Expand the endpoints and click **Try it out**
4. Enter parameters/body and click **Execute**

## Testing with REST Client (VS Code)

Use the included `ProductApi.http` file to test endpoints directly in Visual Studio Code with the [REST Client extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client).

## Project Models

### Product (Model)
```csharp
public class Product
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Category { get; set; }
	public decimal Price { get; set; }
	public int Stock { get; set; }
}
```

### ProductDtos
- **CreateProductDto**: Request DTO for creating a product
- **UpdateProductDto**: Request DTO for updating a product

## Key Classes

### ProductsController
Handles HTTP requests and responses for product operations.

### ProductService
Core business logic for product management, uses in-memory list with Singleton pattern.

### IProductService
Service interface defining contract for product operations.

## Configuration

Edit `appsettings.json` to customize:
- **Logging levels**
- **Allowed hosts**
- **CORS settings** (if added)
- **Other app settings**

## Development

### Hot Reload (Watch Mode)
```bash
dotnet watch run
```
The app will automatically restart when you save files.

### Run Tests
```bash
dotnet test
```
(If test projects are added to the solution)

## Future Enhancements

- [ ] Database integration (SQL Server, PostgreSQL, SQLite with Entity Framework Core)
- [ ] Authentication & Authorization (JWT)
- [ ] Pagination and filtering
- [ ] Unit and integration tests
- [ ] API versioning
- [ ] Logging and monitoring
- [ ] Docker containerization
- [ ] CI/CD pipelines

## Troubleshooting

### Port already in use
If `https://localhost:5001` is in use, check `launchSettings.json` and change the port.

### SSL certificate issues
Trust the dev certificate:
```bash
dotnet dev-certs https --trust
```

### Build errors
Clear build artifacts and restore:
```bash
dotnet clean
dotnet restore
dotnet build
```

## License

This project is part of a training initiative. Feel free to use it for learning purposes.

## Author

**Sangita Zare**  
GitHub: [@SangitaZare](https://github.com/SangitaZare)

## Links

- **Repository**: [angular-trainning/ProductApi](https://github.com/SangitaZare/angular-trainning/tree/main/ProductApi)
- **.NET Documentation**: [docs.microsoft.com/dotnet](https://docs.microsoft.com/dotnet)
- **ASP.NET Core Docs**: [docs.microsoft.com/aspnet/core](https://docs.microsoft.com/aspnet/core)
