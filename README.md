# IntervalsIcuMcp HTTP MCP Server

This project is an HTTP MCP server designed to interact with the [intervals.icu](https://intervals.icu/) API. It provides endpoints for retrieving and manipulating athlete, workout, and activity data, supporting integration and automation scenarios for endurance sports training.

## Features
- **Model Context Protocol (MCP) Server**: Expose Intervals.icu functionality to AI agents like GitHub Copilot
- ASP.NET Core Web API targeting .NET 10
- Traditional REST API endpoints for intervals.icu data
- Extensible service and plugin architecture
- Models for athlete profiles, workouts, activities, and wellness
- Caching and estimation services for efficient data handling
- Workout generation with TSS calculation

## MCP Server

This project implements an MCP server that allows AI agents to interact with your Intervals.icu data using natural language. The MCP server exposes tools for:

- Querying athlete profiles, activities, wellness data, and upcoming events
- Generating structured workouts with proper zone targeting

**See [MCP_SERVER.md](MCP_SERVER.md) for detailed setup and usage instructions.**

Quick start:
```bash
# Set environment variables
export INTERVALS_ICU_API_KEY="your-api-key"
export INTERVALS_ICU_ATHLETE_ID="your-athlete-id"

# Run the server
dotnet run --project IntervalsIcuMcp/IntervalsIcuMcp.csproj

# MCP endpoint available at: http://localhost:5000/api/mcp
```

## Getting Started
1. **Clone the repository:**
   ```sh
   git clone https://github.com/colinnuk/IntervalsIcuMcp.git
   ```
2. **Open the solution in Visual Studio or VS Code.**
3. **Restore NuGet packages:**
   ```sh
   dotnet restore
   ```
4. **Set environment variables:**
   ```sh
   export INTERVALS_ICU_API_KEY="your-api-key"
   export INTERVALS_ICU_ATHLETE_ID="your-athlete-id"
   ```
5. **Build and run the project:**
   ```sh
   dotnet build
   dotnet run --project IntervalsIcuMcp/IntervalsIcuMcp.csproj
   ```
6. **Access the API:**
   - REST endpoints: `http://localhost:5000/McpTool/*`
   - MCP server: `http://localhost:5000/api/mcp`
   - Swagger UI: `http://localhost:5000/swagger`

## Project Structure
- `Controllers/` - API endpoints
- `McpServer/` - MCP tool implementations
- `Services/` - Business logic and API integration
- `Models/` - Data models for intervals.icu entities
- `LlmPlugins/` - Extensible plugins for custom logic
- `Extensions/` - Helper extensions

## Contributing
Pull requests and issues are welcome. Please follow standard C# and .NET coding conventions.

## License
MIT