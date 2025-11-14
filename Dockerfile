# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

WORKDIR /src

# Copy project file
COPY ["IntervalsIcuMcp/IntervalsIcuMcp.csproj", "IntervalsIcuMcp/"]

# Restore dependencies
RUN dotnet restore "IntervalsIcuMcp/IntervalsIcuMcp.csproj"

# Copy the entire source
COPY . .

# Publish the application
RUN dotnet publish "IntervalsIcuMcp/IntervalsIcuMcp.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final

WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Expose port
EXPOSE 5000

# Set default environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:5000

# Run the application
ENTRYPOINT ["dotnet", "IntervalsIcuMcp.dll"]
