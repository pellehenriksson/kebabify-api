# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Kebabify API is a .NET 9 Azure Function that converts strings to kebab-case format. The API receives input strings via HTTP POST, converts them to kebab-case, persists the results to Azure Blob Storage, and returns the transformation.

**Solution structure:**
- `Kebabify.Api/` - Main Azure Function project containing endpoints and services
- `Kebabify.Test/` - xUnit test project with unit tests

**Technology stack:**
- .NET 9
- Azure Functions v4 (isolated worker model)
- Azure Blob Storage with Managed Identity
- Application Insights for telemetry
- xUnit, Moq, Shouldly for testing

## Common Commands

### Build and Test
```bash
# Build the solution
dotnet build

# Build for release
dotnet build --configuration Release --no-restore

# Run all tests
dotnet test

# Run tests with code coverage
dotnet test --configuration Release --collect:"XPlat Code Coverage" --verbosity normal

# Run a specific test
dotnet test --filter "FullyQualifiedName~Kebabify.Test.EndpointTests.MakeKebab_Input_Is_Valid_Should_Return_Result"
```

### Running Locally
```bash
# Run the Azure Function locally (from Kebabify.Api directory)
func start

# The API will be available at http://localhost:7071/api/kebab
```

### Publishing
```bash
# Publish the function app
dotnet publish Kebabify.Api/Kebabify.Api.csproj -c Release --output ./publish
```

## Architecture

### Dependency Injection Setup (Program.cs)
The application uses constructor-based dependency injection configured in `Program.cs:14-46`:
- **BlobServiceClient**: Registered via `AddAzureClients()` with environment-specific configuration
  - Development: Uses connection string from `local.settings.json`
  - Production: Uses Managed Identity with DefaultAzureCredential
- **Services**: Transient lifetime for `IKebabService` and `IStorageService`
- **Telemetry**: Application Insights configured for monitoring

### Service Layer Pattern
The codebase follows a clean separation of concerns:

1. **Endpoints.cs** - HTTP endpoint handlers
   - `MakeKebab` function: POST /api/kebab endpoint
   - Request validation using DataAnnotations (2-512 character limit)
   - Returns JSON response with input and kebab-case result

2. **KebabService** (Services/KebabService.cs) - Business logic
   - Converts input strings to kebab-case format
   - Algorithm: Split on spaces → Remove special characters → Join with hyphens → Lowercase
   - Uses source-generated regex for performance (`[GeneratedRegex]`)

3. **StorageService** (Services/StorageService.cs) - Azure Blob Storage persistence
   - Stores each conversion as a JSON file with GUID filename
   - Auto-creates container if not exists
   - Container name: "kebabify"

### Testing Pattern
Tests use a `Testable` inner class pattern (see `EndpointTests.cs:157-168`):
- Inherits from the class under test
- Exposes mock dependencies as properties
- Static `Create()` factory method for test setup
- Uses Moq for mocking, Shouldly for assertions

## CI/CD

GitHub Actions workflow (`.github/workflows/build.yml`) runs on push/PR to main:
1. Restore dependencies from `Kebabify.slnx`
2. Build in Release configuration
3. Run tests with code coverage
4. Extract coverage percentage and update GitHub badge
5. Deploy to Azure Functions on successful build

## Git Workflow Conventions

From GEMINI.md - the project follows these conventions:
1. Always work in a branch, never commit directly to `main`
2. Review `git log -n 5` to match commit message style
3. Write commit messages focusing on "why" rather than "what"
4. Do not push to remote unless explicitly requested

## Local Development Setup

You'll need a `local.settings.json` file in `Kebabify.Api/` with:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  },
  "Storage": {
    "ConnectionString": "UseDevelopmentStorage=true"
  }
}
```

For local development, use Azure Storage Emulator or Azurite.
