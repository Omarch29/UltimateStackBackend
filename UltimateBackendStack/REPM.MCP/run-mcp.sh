#!/bin/bash
# Run the REPM MCP Server

# Build the project
cd /Users/omar/Desktop/dev/omar/UltimateStackBackend/UltimateBackendStack/REPM.MCP
dotnet build

# Run from the output directory so appsettings.json is found
cd bin/Debug/net9.0
./REPM.MCP
