FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CatalogoDeProdutos.API/CatalogoDeProdutos.API.csproj", "CatalogoDeProdutos.API/"]
COPY ["CatalogoDeProdutos.Application/CatalogoDeProdutos.Application.csproj", "CatalogoDeProdutos.Application/"]
COPY ["CatalogoDeProdutos.Domain/CatalogoDeProdutos.Domain.csproj", "CatalogoDeProdutos.Domain/"]
COPY ["CatalogoDeProdutos.Infrastructure/CatalogoDeProdutos.Infrastructure.csproj", "CatalogoDeProdutos.Infrastructure/"]
COPY ["CatalogoDeProdutos.Tests/CatalogoDeProdutos.Tests.csproj", "CatalogoDeProdutos.Tests/"]

RUN dotnet restore "CatalogoDeProdutos.API/CatalogoDeProdutos.API.csproj"

COPY . .

WORKDIR "/src/CatalogoDeProdutos.API"
RUN dotnet build "CatalogoDeProdutos.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CatalogoDeProdutos.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogoDeProdutos.API.dll"]