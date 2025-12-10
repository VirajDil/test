#!/bin/bash
set -e

echo "Waiting for MSSQL database to be ready..."
sleep 20

echo "MSSQL is ready - starting application..."
exec dotnet todoapp_backend.dll
