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

**CRITICAL: Claude MUST follow these git practices for ALL work in this repository.**

### Branch-Based Workflow

**NEVER commit directly to `main`.** Every task, feature, bugfix, or change must be done in a dedicated branch.

#### Before Starting Any Task:

1. **Check current branch status:**
   ```bash
   git status
   git branch
   ```

2. **Ensure you're on main and it's up to date:**
   ```bash
   git checkout main
   git pull origin main
   ```

3. **Create a new branch for the task:**
   ```bash
   git checkout -b <branch-name>
   ```

#### Branch Naming Conventions:

Use descriptive, kebab-case names that reflect the work being done:

- **Features:** `feature/add-rate-limiting`, `feature/add-validation`
- **Bug fixes:** `fix/null-reference-error`, `fix/storage-timeout`
- **Refactoring:** `refactor/service-layer`, `refactor/dependency-injection`
- **Tests:** `test/add-storage-service-tests`, `test/increase-coverage`
- **Documentation:** `docs/update-readme`, `docs/add-api-examples`
- **Chores:** `chore/update-dependencies`, `chore/cleanup-warnings`

#### Multiple Tasks:

- Each distinct task should get its own branch
- If working on multiple unrelated changes, create separate branches
- Complete and merge one task before starting another when possible

### Commit Message Guidelines

1. **Review recent commit history first:**
   ```bash
   git log -n 5 --oneline
   ```

2. **Match the project's commit style:**
   - Write in imperative mood: "add feature" not "added feature"
   - Focus on **why** the change was made, not **what** was changed
   - Keep the first line under 72 characters
   - Use a blank line before detailed explanation if needed

3. **Good commit message examples:**
   ```
   add rate limiting to prevent abuse
   fix null reference when storage is unavailable
   refactor service registration for better testability
   update dependencies to address security vulnerabilities
   ```

4. **Bad commit message examples:**
   ```
   updated files
   changes
   fixed bug
   WIP
   ```

### Commit Creation Process

When creating commits, Claude should:

1. **Always include co-authorship:**
   ```
   <commit message>

   Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>
   ```

2. **Stage relevant files only:**
   ```bash
   git add <specific-files>
   ```
   Avoid `git add .` unless you've verified all changes are relevant.

3. **Review what will be committed:**
   ```bash
   git status
   git diff --staged
   ```

### Pushing to Remote

**DO NOT push to remote unless explicitly requested by the user.**

When pushing is requested:
```bash
# First time pushing a new branch
git push -u origin <branch-name>

# Subsequent pushes
git push
```

### Pull Request Guidelines

When the user requests a pull request:

1. **Ensure all changes are committed and pushed**
2. **Review the full diff from main:**
   ```bash
   git diff main...HEAD
   git log main..HEAD
   ```

3. **Create PR with descriptive title and body:**
   - Title: Clear, concise summary of changes
   - Body: Explain what changed and why, include testing notes
   - Always include: "Generated with Claude Code"

4. **PR body template:**
   ```markdown
   ## Summary
   - Brief bullet points of what changed
   - Why these changes were made

   ## Test Plan
   - [ ] Tests pass locally
   - [ ] Manual testing completed
   - [ ] No breaking changes introduced

   Generated with Claude Code
   ```

### Safety Checks

Before any git operation, Claude should:

1. **Verify current branch:**
   - Never assume you're on the right branch
   - Always check with `git branch` or `git status`

2. **Avoid destructive operations:**
   - Never use `--force` unless explicitly requested
   - Never use `git reset --hard` unless explicitly requested
   - Never amend commits that have been pushed to remote

3. **Respect git hooks:**
   - Never skip hooks with `--no-verify` unless explicitly requested
   - If a hook fails, fix the issue rather than bypassing it

### Example Workflow

```bash
# 1. Start new task
git checkout main
git pull origin main
git checkout -b feature/add-input-sanitization

# 2. Make changes, test, verify
dotnet build
dotnet test

# 3. Review and commit
git status
git diff
git add Kebabify.Api/Services/KebabService.cs Kebabify.Test/ServiceTests.cs
git commit -m "add input sanitization to prevent XSS

Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>"

# 4. Only push if user requests it
# git push -u origin feature/add-input-sanitization

# 5. Create PR if user requests it
# gh pr create --title "Add input sanitization" --body "..."
```

### Summary: Key Rules

1. **NEVER work directly on main** - always create a branch first
2. **One branch per task** - keep changes focused and isolated
3. **Descriptive branch names** - use conventions above
4. **Meaningful commits** - explain why, not what
5. **Don't push without permission** - wait for user request
6. **Review before committing** - always check status and diff
7. **Follow the project's commit style** - check git log first

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
