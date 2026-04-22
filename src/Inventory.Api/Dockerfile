FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Inventory.Shared/Inventory.Shared.csproj Inventory.Shared/
COPY src/Inventory.Api/Inventory.Api.csproj Inventory.Api/
RUN dotnet restore Inventory.Api/Inventory.Api.csproj

COPY src/Inventory.Shared/ Inventory.Shared/
COPY src/Inventory.Api/ Inventory.Api/
RUN dotnet publish Inventory.Api/Inventory.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "Inventory.Api.dll"]
