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

## Getting Started

### Prerequisites
- **.NET 10 SDK** (for local development)
- **Docker** (for containerized deployment)
- **Intervals.icu API Key** and **Athlete ID** (see [intervals.icu](https://intervals.icu/) for details)

### Environment Variables

The following environment variables are required to run the application:

| Variable | Description | Example |
|----------|-------------|---------|
| `INTERVALS_ICU_API_KEY` | Your Intervals.icu API key | `abc123def456ghi789` |
| `INTERVALS_ICU_ATHLETE_ID` | Your Intervals.icu athlete ID | `12345` |

### Local Development

1. **Clone the repository:**
   ```sh
   git clone https://github.com/colinnuk/IntervalsIcuMcp.git
   cd IntervalsIcuMcp
   ```

2. **Restore NuGet packages:**
   ```sh
   dotnet restore
   ```

3. **Set environment variables:**
   ```bash
   # Linux/macOS
   export INTERVALS_ICU_API_KEY="your-api-key"
   export INTERVALS_ICU_ATHLETE_ID="your-athlete-id"
   
   # Windows PowerShell
   $env:INTERVALS_ICU_API_KEY="your-api-key"
   $env:INTERVALS_ICU_ATHLETE_ID="your-athlete-id"
   ```

4. **Build and run the project:**
   ```sh
   dotnet build
   dotnet run --project IntervalsIcuMcp/IntervalsIcuMcp.csproj
   ```

5. **Access the API:**
   - REST endpoints: `http://localhost:5000/McpTool/*`
   - MCP server: `http://localhost:5000/api/mcp`
   - Swagger UI: `http://localhost:5000/swagger`

### Docker Setup

This project includes a multi-stage Dockerfile using Alpine Linux for minimal image size and cross-platform compatibility.

#### Build and Run with Docker

**Using a `.env` file:**

Create a `.env` file in the project root:
```bash
INTERVALS_ICU_API_KEY=your-api-key-here
INTERVALS_ICU_ATHLETE_ID=your-athlete-id-here
```

Build and run the Docker image:
```bash
# Build the image
docker build -t intervals-icu-mcp .

# Run the container with environment file
docker run --env-file .env -p 5000:5000 intervals-icu-mcp
```

**Using environment variables directly:**
```bash
docker run \
  -e INTERVALS_ICU_API_KEY="your-api-key" \
  -e INTERVALS_ICU_ATHLETE_ID="your-athlete-id" \
  -p 5000:5000 \
  intervals-icu-mcp
```

**Using Docker Compose:**

Create a `docker-compose.yml` file:
```yaml
version: '3.8'

services:
  intervals-icu-mcp:
    build: .
    container_name: intervals-icu-mcp
    ports:
      - "5000:5000"
    environment:
      INTERVALS_ICU_API_KEY: ${INTERVALS_ICU_API_KEY}
      INTERVALS_ICU_ATHLETE_ID: ${INTERVALS_ICU_ATHLETE_ID}
    restart: unless-stopped
```

Then run:
```bash
docker-compose up -d
```

## MCP Server

This project implements an MCP server that allows AI agents to interact with your Intervals.icu data using natural language. The MCP server exposes tools for:

- Querying athlete profiles, activities, wellness data, and upcoming events
- Generating structured workouts with proper zone targeting

**See [MCP_SERVER.md](MCP_SERVER.md) for detailed setup and usage instructions.**

## Project Structure
- `Controllers/` - API endpoints
- `McpServer/` - MCP tool implementations
- `Services/` - Business logic and API integration
- `Models/` - Data models for intervals.icu entities
- `LlmPlugins/` - Extensible plugins for custom logic
- `Extensions/` - Helper extensions

## Testing

This project includes focused unit tests using **NUnit**, targeting .NET 10.

### Test Framework: NUnit 4.3.2

The test suite provides essential coverage with **10 core tests** organized around key functionality:

**EstimateTss Tests (6 tests):**
- Cycling and running sport calculations
- Unsupported sport handling (returns null)
- Edge cases (empty/zero/negative durations)
- Missing athlete data fallback to defaults
- Multi-interval additivity

**EstimateIntensityFactor Tests (4 tests):**
- Cycling and running calculations with clamping
- Empty/unsupported sport edge cases
- Duration-weighted intensity calculations

**Real-World Scenarios (3 tests):**
- Typical endurance ride (90 minutes, steady Z2)
- High-intensity interval training (HIIT with Z6 efforts)
- Tempo run with sustained effort

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity detailed

# Run specific test class
dotnet test --filter "ClassName=WorkoutTssCalculatorTests"

# Run with code coverage
dotnet test /p:CollectCoverageMetrics=true
```

### Test Coverage Notes

The test suite consolidates related scenarios to maintain clarity while reducing redundancy:
- Multiple sport type tests are covered via parametrized tests (e.g., multiple unsupported sports in a single test with `[TestCase]`)
- Missing data scenarios (no FTP, no zones, zero values) are tested in a single comprehensive test
- Zone variations are tested through real-world scenarios rather than isolated zone tests
- All edge cases are validated with realistic data ranges

## Contributing
Pull requests and issues are welcome. Please follow standard C# and .NET coding conventions.

## License
MIT