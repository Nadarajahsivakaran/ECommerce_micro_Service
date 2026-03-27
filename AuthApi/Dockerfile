# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY AuthApi/AuthApi.csproj AuthApi/
COPY ECommerce.Data/ECommerce.Data.csproj ECommerce.Data/
COPY ECommerce.Models/ECommerce.Models.csproj ECommerce.Models/

RUN dotnet restore AuthApi/AuthApi.csproj

COPY . .
RUN dotnet publish AuthApi/AuthApi.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "AuthApi.dll"]
