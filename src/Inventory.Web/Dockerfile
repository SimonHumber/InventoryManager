FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Inventory.Shared/Inventory.Shared.csproj Inventory.Shared/
COPY src/Inventory.Web/Inventory.Web.csproj Inventory.Web/
RUN dotnet restore Inventory.Web/Inventory.Web.csproj

COPY src/Inventory.Shared/ Inventory.Shared/
COPY src/Inventory.Web/ Inventory.Web/
RUN dotnet publish Inventory.Web/Inventory.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "Inventory.Web.dll"]
