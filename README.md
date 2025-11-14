# IntervalsIcuMcp HTTP MCP Server

This project is an HTTP MCP server designed to interact with the [intervals.icu](https://intervals.icu/) API. It provides endpoints for retrieving and manipulating athlete, workout, and activity data, supporting integration and automation scenarios for endurance sports training.

## Features
- ASP.NET Core Web API targeting .NET 10
- Endpoints for interacting with intervals.icu data
- Extensible service and plugin architecture
- Models for athlete profiles, workouts, activities, and wellness
- Caching and estimation services for efficient data handling

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
4. **Build and run the project:**
   ```sh
   dotnet build
   dotnet run --project IntervalsIcuMcp/IntervalsIcuMcp.csproj
   ```
5. **Access the API:**
   The default endpoint is `/McpTool`. You can extend functionality by adding controllers and services.

## Project Structure
- `Controllers/` - API endpoints
- `Services/` - Business logic and API integration
- `Models/` - Data models for intervals.icu entities
- `LlmPlugins/` - Extensible plugins for custom logic
- `Extensions/` - Helper extensions

## Contributing
Pull requests and issues are welcome. Please follow standard C# and .NET coding conventions.

## License
MIT