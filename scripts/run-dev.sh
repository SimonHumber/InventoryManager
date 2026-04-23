#!/usr/bin/env bash
# Runs the Inventory microservice and the MVC web app together for local dev.
# Requires: .NET 10 SDK. Press Ctrl+C to stop both.

set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

export PATH="$PATH:$HOME/.dotnet/tools"

API_URL="http://localhost:5163"
WEB_URL="http://localhost:5100"

cleanup() {
    echo "Shutting down..."
    kill "${API_PID:-}" "${WEB_PID:-}" 2>/dev/null || true
    wait 2>/dev/null || true
}
trap cleanup EXIT INT TERM

echo "Building solution once (avoids parallel-build file lock on Inventory.Shared)..."
dotnet build InventoryManagement.slnx -c Debug -v minimal

echo "Starting Inventory.Api on $API_URL..."
ASPNETCORE_ENVIRONMENT=Development \
    dotnet run --project src/Inventory.Api/Inventory.Api.csproj \
    --no-build --no-launch-profile --urls "$API_URL" &
API_PID=$!

sleep 3

echo "Starting Inventory.Web on $WEB_URL..."
ASPNETCORE_ENVIRONMENT=Development \
    InventoryApi__BaseUrl="$API_URL/" \
    dotnet run --project src/Inventory.Web/Inventory.Web.csproj \
    --no-build --no-launch-profile --urls "$WEB_URL" &
WEB_PID=$!

echo ""
echo "======================================================"
echo "  Inventory Manager is running"
echo "  Web:  $WEB_URL"
echo "  API:  $API_URL"
echo ""
echo "  Default accounts:"
echo "    Admin:  admin@inventory.local / Admin#12345"
echo "    User:   user@inventory.local  / User#12345"
echo "======================================================"
echo ""

wait
