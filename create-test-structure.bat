@echo off
REM Create test files for KafkaWorkerService
echo Creating test files...

REM Create directory
if not exist "KafkaWorkerService.Tests" mkdir "KafkaWorkerService.Tests"
cd KafkaWorkerService.Tests

echo Done! Test project structure created.
echo.
echo Run the PowerShell script setup-tests.ps1 to complete the setup.
