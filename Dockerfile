# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["LaundryMarketplace.sln", "."]
COPY ["src/LaundryMarketplace.Core/LaundryMarketplace.Core.csproj", "src/LaundryMarketplace.Core/"]
COPY ["src/LaundryMarketplace.Infrastructure/LaundryMarketplace.Infrastructure.csproj", "src/LaundryMarketplace.Infrastructure/"]
COPY ["src/LaundryMarketplace.Web/LaundryMarketplace.Web.csproj", "src/LaundryMarketplace.Web/"]

# Restore
RUN dotnet restore "src/LaundryMarketplace.Web/LaundryMarketplace.Web.csproj"

# Copy everything else
COPY . .
WORKDIR "/src/src/LaundryMarketplace.Web"
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "LaundryMarketplace.Web.dll"]
