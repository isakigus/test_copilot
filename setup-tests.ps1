# KafkaWorkerService Test Project Setup Script
# Run this script from C:\repos\test_copilot

Write-Host "================================"
Write-Host "Kafka Worker Service Test Setup"
Write-Host "================================"
Write-Host ""

# Step 1: Create and switch to testing branch
Write-Host "Step 1: Creating testing branch..."
try {
    git checkout -b testing 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        git checkout testing 2>&1 | Out-Null
    }
    Write-Host "  ✓ Testing branch created/checked out" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Failed to create/checkout testing branch: $_" -ForegroundColor Red
}

# Step 2: Create test project directory
Write-Host "Step 2: Creating test project..."
try {
    dotnet new xunit -n KafkaWorkerService.Tests -o KafkaWorkerService.Tests --force
    Write-Host "  ✓ Test project created" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Failed to create test project: $_" -ForegroundColor Red
    exit 1
}

# Step 3: Add NuGet packages
Write-Host "Step 3: Adding NuGet packages..."
Set-Location KafkaWorkerService.Tests
try {
    dotnet add package Moq --version 4.20.72
    dotnet add package KafkaFlow.UnitTests --version 3.0.11
    dotnet add package Microsoft.Extensions.Configuration --version 10.0.2
    dotnet add package Microsoft.Extensions.Logging.Abstractions --version 10.0.2
    Write-Host "  ✓ NuGet packages added" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Failed to add NuGet packages: $_" -ForegroundColor Red
}

# Step 4: Add project reference
Write-Host "Step 4: Adding project reference..."
try {
    dotnet add reference ..\KafkaWorkerService\KafkaWorkerService.csproj
    Write-Host "  ✓ Project reference added" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Failed to add project reference: $_" -ForegroundColor Red
}

# Step 5: Add to solution
Write-Host "Step 5: Adding to solution..."
Set-Location ..
try {
    dotnet sln add KafkaWorkerService.Tests\KafkaWorkerService.Tests.csproj
    Write-Host "  ✓ Added to solution" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Failed to add to solution: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "================================"
Write-Host "Setup completed!"
Write-Host "================================"
Write-Host ""
Write-Host "Next steps:"
Write-Host "1. The test files will be created automatically"
Write-Host "2. Run 'dotnet build' to verify the project compiles"
Write-Host "3. Run 'dotnet test' to execute the unit tests"
Write-Host ""
