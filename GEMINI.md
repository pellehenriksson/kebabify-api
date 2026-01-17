# Kebabify API

This project is a .NET 9 Azure Function that converts strings to "kebab case".

## Project Overview

The solution contains two projects:

-   `Kebabify.Api`: The main Azure Function project.
-   `Kebabify.Test`: A project for testing the API.

The API exposes a single endpoint, `POST /api/kebab`, which accepts a JSON body with an `input` string. It converts the string to kebab case, persists the result to Azure Blob Storage, and returns the original and converted strings.

The project uses the following key technologies:

-   .NET 9
-   Azure Functions
-   Azure Blob Storage
-   xUnit
-   Moq
-   Shouldly

## Building and Running

### Prerequisites

-   .NET 9 SDK
-   Azure Functions Core Tools

### Building

To build the project, run the following command from the root directory:

```bash
dotnet build
```

### Running Locally

To run the project locally, you can use the Azure Functions Core Tools:

```bash
func start
```

This will start the function app, and you can send requests to `http://localhost:7071/api/kebab`.

### Testing

To run the tests, run the following command from the root directory:

```bash
dotnet test
```

## Development Conventions

### Coding Style

The project uses the default .NET coding style.

### Testing

The project uses xUnit for testing, Moq for mocking, and Shouldly for assertions. Tests are located in the `Kebabify.Test` project.

### Continuous Integration and Deployment

The project uses GitHub Actions for CI/CD. The workflow is defined in the `.github/workflows/build.yml` file. The workflow builds, tests, and deploys the project to Azure Functions on every push to the `main` branch.

### Git Conventions

When you are asked to make changes to the codebase, you should follow these guidelines:

1.  **Work in a branch:** Always create a new branch for your changes. Never commit directly to the `main` branch.
2.  **Check Status:** Before committing, run `git status` to ensure that all relevant files are staged.
3.  **Review History:** Look at the last few commit messages by running `git log -n 5`. This will help you to write a commit message that is in line with the project's conventions.
4.  **Propose Message:** Propose a descriptive and concise commit message that explains the "why" of the change, not just the "what".
5.  **Do Not Push:** Do not push changes to the remote repository unless you are explicitly asked to do so.
