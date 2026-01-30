# KafkaWorkerService Test Project Setup

This document provides complete instructions and all necessary files for setting up unit tests for the KafkaWorkerService project.

## Automated Setup (Recommended)

Run the following command from `C:\repos\test_copilot`:

```powershell
.\setup-tests.ps1
```

Then manually create the test files listed below.

## Manual Setup Instructions

### 1. Create and Switch to Testing Branch

```powershell
cd C:\repos\test_copilot
git checkout -b testing
```

### 2. Create Test Project

```powershell
dotnet new xunit -n KafkaWorkerService.Tests -o KafkaWorkerService.Tests
```

### 3. Add NuGet Packages

```powershell
cd KafkaWorkerService.Tests
dotnet add package Moq --version 4.20.72
dotnet add package KafkaFlow.UnitTests --version 3.0.11
dotnet add package Microsoft.Extensions.Configuration --version 10.0.2
dotnet add package Microsoft.Extensions.Logging.Abstractions --version 10.0.2
```

### 4. Add Project Reference

```powershell
dotnet add reference ..\KafkaWorkerService\KafkaWorkerService.csproj
```

### 5. Add to Solution

```powershell
cd ..
dotnet sln add KafkaWorkerService.Tests\KafkaWorkerService.Tests.csproj
```

### 6. Create Test Files

Replace the default `UnitTest1.cs` with the following test files:

#### KafkaWorkerService.Tests.csproj
Update the project file to include all dependencies (see below).

#### MessageHandlerTests.cs
Tests for the MessageHandler class including:
- Constructor validation
- Message handling
- Logging verification
- Interface implementation

#### KafkaMessageTests.cs
Tests for the KafkaMessage model including:
- Property initialization
- Value assignment
- Edge cases (empty content, large content)

#### KafkaSettingsTests.cs
Tests for KafkaSettings configuration class including:
- Property initialization
- Multiple bootstrap servers
- Property modification

#### MySQLSettingsTests.cs
Tests for MySQLSettings configuration class including:
- Connection string handling
- SSL connection strings
- Multiple instances independence

#### Usings.cs
Global using statements for cleaner test files.

### 7. Build and Test

```powershell
dotnet build
dotnet test
```

### 8. Commit Changes

```powershell
git add .
git commit -m "Add unit tests for KafkaWorkerService"
```

## Test Coverage

The test suite covers:

1. **MessageHandler Class**
   - Constructor validation with valid/invalid configuration
   - Message handling and logging
   - Error handling
   - Interface implementation

2. **KafkaMessage Model**
   - Default initialization
   - Property assignment
   - Edge cases

3. **KafkaSettings Class**
   - Configuration properties
   - Multiple server handling
   - Property modification

4. **MySQLSettings Class**
   - Connection string management
   - SSL configuration
   - Instance independence

## Running Tests

From the solution directory:

```powershell
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

## Test Framework and Tools

- **xUnit**: Test framework
- **Moq**: Mocking framework for dependencies
- **KafkaFlow.UnitTests**: KafkaFlow testing utilities
- **Microsoft.Extensions.Configuration**: Configuration testing
- **Microsoft.Extensions.Logging.Abstractions**: Logger mocking

## Notes

- Some MessageHandler tests may show database connection errors in isolation, which is expected behavior
- The tests focus on business logic and configuration validation
- Database integration tests would require additional setup (not included)

