# DemoShop Web API

A **.NET Core** Web API solution implementing a simple e-commerce system with **Authentication**, **Product Management**, **Order Management**, **Shopping Cart**, **Admin Dashboard** capabilities, **Payment Gateway simulation**, **Logging**, **Error Handling**, and **Testing**.

## Overview

This project demonstrates:

1. **User Management**:  
   - JWT-based authentication and authorization.  
   - Role-based endpoints (`Admin` vs. `User`).  
   - Password hashing (replace sample hashing with a secure method in production).

2. **Product Management**:  
   - CRUD operations, categories, and stock management.  
   - Optional caching (Redis) for performance.  
   - Search & filter endpoints.

3. **Shopping Cart**:  
   - Users can add products to their cart, view, modify, clear, then place orders.

4. **Order Management**:  
   - Placing orders (with stock reduction).  
   - Order status updates (Admin-only).  
   - Payment simulation or integration with real gateways.

5. **Admin Dashboard**:  
   - Orders overview, user info, and inventory monitoring.

6. **Logging & Error Handling**:  
   - Structured logging via Serilog.  
   - Global exception handling with consistent `ProblemDetails` responses.

7. **Unit & Integration Testing**:  
   - xUnit + Moq for business logic tests.  
   - `TestServer` or `WebApplicationFactory` for endpoint-level integration tests.

8. **Asynchronous / Background Jobs**:  
   - Example of background services for sending confirmation emails or periodic tasks (using Hangfire or a hosted service).

## Technologies

- **.NET Core**  
- **Entity Framework Core** with SQL Server  
- **JWT** Authentication (using `System.IdentityModel.Tokens.Jwt`)  
- **Serilog** for structured logging  
- **Swagger** for API documentation  
- **xUnit** + **Moq** for testing  
- **Hangfire** for background tasks

## Getting Started

### Prerequisites

- **.NET 8 SDK** or higher  
- **SQL Server** (local or remote) or any configured DB provider  

### Installation

1. **Clone** this repository:
   ```bash
   git clone https://github.com/aashimasharma24/DemoShop.git
   cd DemoShop
   ```

2. **Set up** your database connection in `appsettings.json` (or `appsettings.Development.json`):
   ```jsonc
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=DemoShopDB;Trusted_Connection=True;"
     },
     "Jwt": {
       "Key": "YOUR_SECRET_KEY_HERE"
     },
     "Serilog": {
       "Using": [ "Serilog.Sinks.Console" ],
       "MinimumLevel": "Information",
       "WriteTo": [
         {
           "Name": "Console"
         }
       ]
     }
   }
   ```

3. **Restore** packages:
   ```bash
   dotnet restore
   ```

4. **Apply migrations** (from the `DemoShop.API` project directory or specifying the .csproj paths):
   ```bash
   dotnet ef database update  --startup-project ./DemoShop.API.csproj --project ..\DemoShop.Manager\DemoShop.Manager.csproj
   ```

5. **Run** the application:
   ```bash
   dotnet run --project src/DemoShop.API/DemoShop.API.csproj
   ```
   The API is now available (by default) at `https://localhost:5001` or `http://localhost:5000`.

### Usage

- **Swagger UI**: Navigate to `https://localhost:5001/swagger` to explore and test endpoints interactively.  
- **Authentication**: Obtain a **JWT** token from `/api/v1/auth/login` or `/api/v1/auth/register`, then include it in `Authorization: Bearer <token>` headers for protected endpoints.  
- **Admin Endpoints**: Create an Admin user (by role or manually setting `Role="Admin"` in the DB) to access `/api/v1/admin/...`.
