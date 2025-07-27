#!/bin/bash
# Run the REPM MCP Server

# Get the directory of this script
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Build the project
cd "$SCRIPT_DIR"
dotnet build

# Run from the output directory so appsettings.json is found
cd bin/Debug/net9.0

# Set environment variable for connection string (fallback)
export REPM_CONNECTION_STRING="Host=localhost;Port=5432;Database=realestatetest;Username=omar;Password=rootroot"

./REPM.MCP
