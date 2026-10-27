# CompanyApi

A RESTful API built with **ASP.NET Core** and **Entity Framework Core** for managing company departments and employees.

## Features

- **Department Management**: Create, read, update, and delete departments
- **Employee Management**: Create, read, update, and delete employees  
- **Entity Framework Core**: Database ORM with migrations
- **.NET 10**: Latest .NET framework
- **RESTful API**: Clean HTTP endpoints for CRUD operations
- **DTOs**: Data Transfer Objects for request/response handling
- **AutoMapper**: Automatic mapping between entities and DTOs

## Project Structure

```
CompanyApi/
├── Controllers/          # API endpoints
│   ├── DepartmentsController.cs
│   └── EmployeesController.cs
├── Models/               # Domain models
│   ├── Department.cs
│   └── Employee.cs
├── DTOs/                 # Data Transfer Objects
│   ├── DepartmentDtos.cs
│   └── EmployeeDtos.cs
├── Data/                 # Database context
│   └── AppDbContext.cs
├── Migrations/           # EF Core migrations
├── Mappings/             # AutoMapper profiles
├── appsettings.json      # Configuration
└── Program.cs            # Application startup
```

## Prerequisites

- .NET 10 SDK
- SQL Server or another supported database
- Visual Studio 2026 or VS Code

## Getting Started

### 1. Clone the repository
```bash
git clone https://github.com/SangitaZare/angular-trainning.git
cd CompanyApi/CompanyApi
```

### 2. Restore dependencies
```bash
dotnet restore
```

### 3. Update database
```bash
dotnet ef database update
```

### 4. Run the application
```bash
dotnet run
```

The API will be available at `https://localhost:5001`

## API Endpoints

### Departments
- `GET /api/departments` - Get all departments
- `GET /api/departments/{id}` - Get department by ID
- `POST /api/departments` - Create new department
- `PUT /api/departments/{id}` - Update department
- `DELETE /api/departments/{id}` - Delete department

### Employees
- `GET /api/employees` - Get all employees
- `GET /api/employees/{id}` - Get employee by ID
- `POST /api/employees` - Create new employee
- `PUT /api/employees/{id}` - Update employee
- `DELETE /api/employees/{id}` - Delete employee

## Configuration

Update `appsettings.json` with your database connection string:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=.;Database=CompanyDb;Trusted_Connection=true;"
  }
}
```

## Technologies

- ASP.NET Core 10
- Entity Framework Core 10
- AutoMapper
- SQL Server

## License

This project is part of the angular-trainning repository.

## Author

[SangitaZare](https://github.com/SangitaZare)
