# Todo Application

A .NET Core Web API for managing todos with comprehensive test coverage.

## Prerequisites
- .NET 6.0 SDK
- dotnet CLI tools

## Running the Application

1. Clone the repository
2. Navigate to the project directory
3. Run the application:
bash
dotnet run --project TodoBackend
## Running Tests

### Basic Test Execution
bash
dotnet test
### Run Specific Test Project
bash
dotnet test TodoBackend.Tests
## Code Coverage

### Setup
1. Install the ReportGenerator tool:
bash
dotnet tool install -g dotnet-reportgenerator-globaltool
### Generate Coverage Report
1. Run tests with coverage collection:
bash
dotnet test --collect:"XPlat Code Coverage"
2. Generate HTML report:
bash
reportgenerator -reports:"/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

3. View the report by opening `coveragereport/index.html` in your browser

## Current Coverage Stats

The application maintains high test coverage across components:

### Core Components
- Program.cs: 100% line and branch coverage
- TodoService: 100% line coverage, 62.5% branch coverage
- TaskAnalyzer: 100% line coverage, 83.3% branch coverage
- TodoController: 71.6% line coverage, 57.1% branch coverage
- TodoRepository: 90.6% line coverage, 50% branch coverage

### Data Layer
- TodoDbContext: 100% line coverage
- Todo Model: 100% line coverage (8/8 lines)
- DTOs: 100% line coverage

## Coverage Requirements

Minimum coverage thresholds:
- Line coverage: 70%
- Branch coverage: 50%
- Method coverage: 80%

These thresholds help maintain code quality and prevent coverage regression.

## Project Structure

- `TodoBackend/` - Main API project
- - `TodoFrontend/` - Frontend for project
- `TodoBackend.Tests/` - Test project containing unit and integration tests
- `coveragereport/` - Generated coverage reports
