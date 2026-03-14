# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ProductApi/*.csproj ProductApi/
COPY ProductApi.Application/*.csproj ProductApi.Application/
COPY ProductApi.Infrastructure/*.csproj ProductApi.Infrastructure/
COPY ProductApi.Models/*.csproj ProductApi.Models/
COPY ../ECommerce.Data/ECommerce.Data.csproj ECommerce.Data/
COPY ../ECommerce.Models/ECommerce.Models.csproj ECommerce.Models/

# Restore dependencies
RUN dotnet restore ProductApi/ProductApi.csproj

# Copy everything
COPY . .

# Publish
RUN dotnet publish ProductApi/ProductApi.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "ProductApi.dll"]
