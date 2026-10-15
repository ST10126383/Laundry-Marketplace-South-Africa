# Laundry Marketplace South Africa & True People's Dignity Foundation

**WIL Task 2 – Full System Implementation**  
Student Number: ST10126383  
Region: Gauteng Province, South Africa

## Overview

Commercial hybrid platform that connects customers with laundromats while automatically allocating **10% of platform commission** to the **True People's Dignity Foundation** for clothing donations and bulk uniform procurement.

## Features Implemented

- Multi-role authentication (Customer, Laundromat, Driver, NPO Admin, System Admin)
- Customer laundry booking + optional clothing donation
- Automatic 10% NPO fund allocation on order creation
- Order tracking with status progress bar
- Laundromat order management (accept + update status)
- NPO Admin dashboard (balance, ledger, impact metrics)
- Uniform procurement with fund deduction
- Public Transparency / Impact Dashboard on home page
- Responsive Bootstrap 5 UI (desktop + mobile)
- Global exception handling middleware
- Clean Architecture + Service layer
- Cloud-ready database configuration

## Solution Structure

```
LaundryMarketplace/
├── src/
│   ├── LaundryMarketplace.Core/          # Domain entities, enums, interfaces
│   ├── LaundryMarketplace.Infrastructure/# EF Core, Identity, Services, Seeding
│   └── LaundryMarketplace.Web/           # MVC UI, Controllers, Views, Middleware
├── tests/
├── docs/Azure_SQL_Setup.md
└── README.md
```

## Quick Start

1. Install .NET 8 SDK
2. Open terminal in the project root
3. Run:

```bash
cd src/LaundryMarketplace.Web
dotnet restore
dotnet ef migrations add InitialCreate --project ../LaundryMarketplace.Infrastructure --startup-project .
dotnet ef database update --project ../LaundryMarketplace.Infrastructure --startup-project .
dotnet run
```

4. Open https://localhost:5xxx

### Default Seeded Accounts

| Role          | Email                    | Password       |
|---------------|--------------------------|----------------|
| System Admin  | admin@dignity.sa         | Admin@123!     |
| NPO Admin     | npo@dignity.sa           | Npo@123!       |
| Customer      | customer@example.com     | Customer@123!  |
| Laundromat    | laundromat@example.com   | Laundry@123!   |

## Cloud Database (Required for full marks)

See `docs/Azure_SQL_Setup.md` for step-by-step Azure SQL instructions.

Replace the connection string in `appsettings.json` with your Azure SQL string.

## Security Features

- ASP.NET Core Identity (hashed passwords + lockout)
- Role-based authorization on every controller
- Anti-forgery tokens on all forms
- Parameterized queries via EF Core
- Input validation + ModelState
- Global exception middleware
- HTTPS redirection

## Next Steps for Submission

1. Push this code to your GitHub repo: https://github.com/ST10126383/Laundry-Marketplace-South-Africa
2. Add your lecturer as collaborator
3. Configure Azure SQL (or Neon) and update connection string
4. Deploy to Azure App Service (optional but recommended)
5. Test all roles thoroughly

## Academic Alignment

This implementation follows the high-level design, corrected UML, ERD, Clean Architecture, and 10% allocation business rule documented in Task 1.
