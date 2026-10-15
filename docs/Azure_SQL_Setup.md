# Azure SQL / Cloud Database Setup Instructions

This project is designed to work with a cloud-based database (required for Task 2 marks).

## Option 1 – Azure SQL Database (Recommended)

1. Go to [portal.azure.com](https://portal.azure.com) and sign in.
2. Create a new resource → **SQL Database**.
3. Choose:
   - Database name: `LaundryMarketplaceDb`
   - Server: create a new server (e.g. `laundry-marketplace-server`)
   - Pricing tier: Basic (cheapest for student work) or free tier if available.
4. After creation, go to the database → **Connection strings** → copy the ADO.NET connection string.
5. Replace the password placeholder with the server admin password you set.
6. Open `src/LaundryMarketplace.Web/appsettings.json` and paste:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=tcp:YOURSERVER.database.windows.net,1433;Initial Catalog=LaundryMarketplaceDb;Persist Security Info=False;User ID=YOURADMIN;Password=YOURPASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}
```

7. In Azure portal → your SQL Server → **Networking** → add your client IP (or temporarily allow Azure services).
8. Run the application – the seeder will create tables automatically on first run (or use migrations).

### Creating the migration (once)

```bash
cd src/LaundryMarketplace.Web
dotnet ef migrations add InitialCreate --project ../LaundryMarketplace.Infrastructure --startup-project .
dotnet ef database update --project ../LaundryMarketplace.Infrastructure --startup-project .
```

## Option 2 – Free Neon PostgreSQL (Alternative)

1. Go to [neon.tech](https://neon.tech) and create a free project.
2. Copy the connection string.
3. Change the EF Core provider in `Infrastructure.csproj` and `Program.cs` to use `Npgsql.EntityFrameworkCore.PostgreSQL`.
4. Update the connection string in `appsettings.json`.

## Option 3 – LocalDB (Development only)

The default connection string already uses LocalDB:

```
Server=(localdb)\\mssqllocaldb;Database=LaundryMarketplaceDb;Trusted_Connection=True;...
```

This is fine for local testing but **will not score full marks** for the “cloud-based database” criterion. Use Azure SQL or Neon for the final submission.

## Security Notes

- Never commit real passwords to GitHub.
- Use User Secrets or Azure App Settings for production:
  ```bash
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-azure-string"
  ```
- Always keep `Encrypt=True` for Azure SQL.
