# MCP Server Configuration

This project now includes an MCP (Model Context Protocol) server that exposes Intervals.icu functionality to AI agents like GitHub Copilot.

## What is MCP?

The Model Context Protocol (MCP) is an open standard that allows AI agents to securely connect to external data sources and tools. By exposing your Intervals.icu endpoints through MCP, you enable AI assistants to:

- Query athlete profiles, activities, wellness data, and upcoming events
- Generate structured workouts with proper zone targeting
- Use pre-built training prompts to guide training analysis and planning
- Interact with your training data using natural language

## Available MCP Tools

The MCP server exposes the following tools:

### Intervals.icu Tools

1. **GetAthleteProfileAsync**: Gets the athlete's profile including FTP, weight, heart rate zones, and fitness metrics (cached)
2. **GetRecentActivitiesAsync**: Gets the last 6 weeks of activities including workouts, races, and training sessions
3. **GetWellnessAsync**: Gets wellness data for a specific date (yyyy-MM-dd) including fatigue, soreness, sleep quality, etc.
4. **GetUpcomingEventsAsync**: Gets upcoming calendar events (planned workouts/races) for the next N days

### Workout Generator Tools

1. **GenerateWorkoutAsync**: Generates a structured workout with intervals, estimates TSS, and formats for Intervals.icu

## Running the MCP Server

### Prerequisites

Set the required environment variables:

```bash
export INTERVALS_ICU_API_KEY="your-api-key"
export INTERVALS_ICU_ATHLETE_ID="your-athlete-id"
```

### Start the Server

```bash
dotnet run --project IntervalsIcuMcp/IntervalsIcuMcp.csproj
```

The MCP server will be available at: `http://localhost:5000/api/mcp`

## Connecting to GitHub Copilot

### In Visual Studio Code / Codespaces

1. Open Copilot Chat and select **Agent** mode
2. Click the **Tools** button ? **Add More Tools...**
3. Select **Add MCP Server**
4. Choose **HTTP (HTTP or Server-Sent Events)**
5. Enter Server URL: `http://localhost:5000/api/mcp`
6. Enter Server ID: `intervals-icu-mcp` (or any name you prefer)
7. Select **Workspace Settings**

### Configuration File

You can also add the server to your `.vscode/mcp.json` file:

```json
{
  "servers": {
    "intervals-icu-mcp": {
      "type": "http",
      "url": "http://localhost:5000/api/mcp"
    }
  }
}
```

## Example Interactions

Once connected, you can interact with your Intervals.icu data using natural language in Copilot Chat:

- "Show me my athlete profile"
- "What were my activities in the last week?"
- "Get my wellness data for 2024-01-15"
- "What workouts do I have coming up?"
- "Generate a cycling workout with 3x10 minute intervals at Z4"

## Security Considerations

The current implementation allows any origin (CORS configured with `AllowAnyOrigin`) for development purposes. For production:

1. **Restrict CORS origins** to trusted domains only
2. **Add authentication/authorization** to protect sensitive athlete data
3. **Use HTTPS** for all connections
4. **Implement rate limiting** to prevent abuse
5. **Validate and sanitize** all inputs
6. **Monitor and log** MCP endpoint access

Example production CORS configuration:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://your-trusted-domain.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

## Deployment to Azure App Service

To deploy your MCP server:

1. Deploy to Azure App Service (follow standard ASP.NET Core deployment)
2. Set environment variables in App Service Configuration:
   - `INTERVALS_ICU_API_KEY`
   - `INTERVALS_ICU_ATHLETE_ID`
3. Update your MCP client configuration to use: `https://your-app.azurewebsites.net/api/mcp`

## Technical Details

### Implementation

- **Framework**: ASP.NET Core with ModelContextProtocol.AspNetCore package
- **Transport**: HTTP with Server-Sent Events (SSE) for streaming
- **Tool Discovery**: Automatic via `[McpServerToolType]` and `[McpServerTool]` attributes
- **Descriptions**: Uses `[Description]` attributes to provide context to the AI agent

### Architecture

```
???????????????????
?  AI Agent       ?
? (GitHub Copilot)?
???????????????????
         ? MCP Protocol (JSON-RPC 2.0)
         ? HTTP/SSE
         ?
???????????????????????????
?  MCP Server             ?
?  /api/mcp endpoint      ?
???????????????????????????
?  IntervalsIcuMcpTool    ?
?  WorkoutGeneratorMcpTool?
???????????????????????????
         ?
         ?
???????????????????????????
?  Business Logic         ?
?  IntervalsIcuPlugin     ?
?  WorkoutGeneratorPlugin ?
???????????????????????????
         ?
         ?
???????????????????????????
?  Intervals.icu API      ?
???????????????????????????
```

## Troubleshooting

### CORS Errors

If you see CORS errors in the browser console:
- Ensure `app.UseCors()` is called in Program.cs
- Check that CORS middleware is configured before `app.MapMcp()`

### Connection Issues

- Verify the server is running: `curl http://localhost:5000/api/mcp`
- Check environment variables are set correctly
- Look at server logs for authentication errors with Intervals.icu

### Tool Not Found

- Ensure your MCP tool classes have `[McpServerToolType]` attribute
- Methods must have `[McpServerTool]` attribute
- Rebuild the project after adding new tools

## Resources

- [Model Context Protocol Specification](https://spec.modelcontextprotocol.io/)
- [MCP C# SDK GitHub](https://github.com/modelcontextprotocol/csharp-sdk)
- [MCP C# SDK Documentation](https://modelcontextprotocol.github.io/csharp-sdk/api/ModelContextProtocol.html)
- [Intervals.icu API Documentation](https://forum.intervals.icu/t/api-documentation/45)
