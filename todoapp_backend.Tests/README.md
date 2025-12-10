# XUnit Test Suite for TodoApp Backend

## Overview
This directory contains a comprehensive XUnit test suite for the TodoApp Backend ASP.NET Core application. The tests are organized into three main categories: Services, Controllers, and Repositories.

## Test Files Created

### 1. **Services/TaskServiceTests.cs**
Contains 16 unit tests for the `TaskService` class using mocks for the repository layer.

**Test Coverage:**
- `GetRecentTasksAsync` - 3 tests
  - Returns recent tasks with default count
  - Returns correct number of tasks with custom count
  - Returns empty collection when no tasks exist

- `GetTaskByIdAsync` - 2 tests
  - Returns task with valid ID
  - Returns null with invalid ID

- `CreateTaskAsync` - 5 tests
  - Creates and returns task with valid data
  - Throws exception when title is null/empty
  - Throws exception when description is null/empty
  - Trims whitespace from title and description

- `UpdateTaskAsync` - 3 tests
  - Updates task with valid data
  - Throws exception with invalid ID
  - Updates only provided fields (partial updates)
  - Trims whitespace from updated fields

- `MarkTaskAsCompletedAsync` - 2 tests
  - Marks task as completed with valid ID
  - Throws exception with invalid ID

**Key Features:**
- Uses `Mock<ITaskRepository>` for dependency injection
- Tests business logic validation
- Tests data transformation (DTO mapping)
- Covers edge cases and error scenarios

### 2. **Controllers/TasksControllerTests.cs**
Contains 14 unit tests for the `TasksController` class using mocks for the service layer.

**Test Coverage:**
- `GetRecentTasks` - 6 tests
  - Returns OK result with valid count
  - Returns OK result with custom count
  - Returns BadRequest for invalid counts (0, negative, >100)
  - Returns 500 InternalServerError on service exception

- `GetTaskById` - 3 tests
  - Returns OK result with valid ID
  - Returns NotFound with invalid ID
  - Returns 500 InternalServerError on service exception

- `CreateTask` - 5 tests
  - Returns CreatedAtAction with valid data
  - Returns BadRequest for empty title/description
  - Returns BadRequest when service throws ArgumentException
  - Returns 500 InternalServerError on general exception

- `UpdateTask` - 3 tests
  - Returns OK result with valid data
  - Returns NotFound with invalid ID
  - Returns 500 InternalServerError on service exception

- `CompleteTask` - 3 tests
  - Returns OK result with completed task
  - Returns NotFound with invalid ID
  - Returns 500 InternalServerError on service exception

**Key Features:**
- Tests HTTP response codes and types
- Tests error handling
- Tests action method routing and parameters
- Uses Moq for service mocking
- Verifies response content

### 3. **Repositories/TaskRepositoryTests.cs**
Contains 11 integration tests for the `TaskRepository` class using InMemoryDatabase.

**Test Coverage:**
- `GetRecentActiveTasks` - 4 tests
  - Returns recent incomplete tasks with default count
  - Returns tasks ordered by CreatedAt descending
  - Respects count limit
  - Returns empty list when no tasks exist

- `GetTaskByIdAsync` - 2 tests
  - Returns task with valid ID
  - Returns null with invalid ID

- `CreateTaskAsync` - 2 tests
  - Creates task with new ID
  - Sets CreatedAt and UpdatedAt timestamps

- `UpdateTaskAsync` - 2 tests
  - Updates task with new values
  - Updates UpdatedAt timestamp

- `SaveAsync` - 1 test
  - Persists changes to database

**Key Features:**
- Uses `InMemoryDatabase` for isolated testing
- Tests actual EF Core operations
- Tests database persistence
- Tests timestamp management
- Verifies data ordering

## Running the Tests

### Prerequisites
- .NET 8.0 SDK or higher
- Required NuGet packages:
  - xunit (2.4.0 or later)
  - Moq (4.20.72 or later)
  - Microsoft.EntityFrameworkCore.InMemory (8.0.0)

### Run All Tests
```bash
cd todoapp_backend.Tests
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~TaskServiceTests"
dotnet test --filter "FullyQualifiedName~TasksControllerTests"
dotnet test --filter "FullyQualifiedName~TaskRepositoryTests"
```

### Run Specific Test Method
```bash
dotnet test --filter "FullyQualifiedName~TaskServiceTests.CreateTaskAsync_WithValidData_CreatesAndReturnsTask"
```

### Run Tests with Verbose Output
```bash
dotnet test --verbosity normal
```

### Generate Coverage Report
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

## Test Architecture

### Layers Tested

```
Controllers (HTTP Layer)
    ↓ mocked
Services (Business Logic)
    ↓ actual/mocked
Repositories (Data Access)
    ↓ InMemory DB
Database
```

### Mocking Strategy
- **Service Layer Tests**: Mock ITaskRepository
- **Controller Layer Tests**: Mock ITaskService
- **Repository Layer Tests**: Use InMemoryDatabase for actual EF Core testing

### Naming Convention
- Test method names follow: `MethodName_Scenario_ExpectedResult`
- Example: `CreateTaskAsync_WithEmptyTitle_ThrowsArgumentException`

## Test Statistics

- **Total Tests**: 41
- **Service Tests**: 16
- **Controller Tests**: 14
- **Repository Tests**: 11
- **Expected Pass Rate**: 100% (if main code is correct)

## Key Test Scenarios Covered

### Validation Tests
- Null/empty input validation
- Boundary condition testing
- Range validation (e.g., count between 1-100)

### Error Handling Tests
- Exception propagation
- HTTP error code responses
- NotFound scenarios
- Internal server errors

### Business Logic Tests
- Data transformation (DTO mapping)
- Timestamp management
- Status updates
- Filtering and ordering

### Integration Tests
- End-to-end data flow
- Database persistence
- Transaction handling

## Best Practices Implemented

1. **AAA Pattern**: Arrange-Act-Assert structure in all tests
2. **Clear Test Names**: Descriptive names indicating what is being tested
3. **Isolated Tests**: Each test is independent and can run in any order
4. **Mock Usage**: Mocks injected via constructor
5. **Verification**: Moq `Verify()` used to ensure methods are called correctly
6. **Exception Testing**: Proper use of `Assert.ThrowsAsync()`
7. **In-Memory Database**: Isolated database per test using unique names

## Continuous Integration

These tests are suitable for CI/CD pipelines:
```yaml
- Run tests before each commit
- Generate coverage reports
- Fail build if tests don't pass
- Track coverage trends over time
```

## Maintenance Notes

- Update tests when business logic changes
- Keep mocked dependencies in sync with interfaces
- Review test coverage reports regularly
- Remove tests for deprecated features
- Add tests for new features before implementation (TDD)

## Troubleshooting

### Tests Won't Run
1. Ensure all NuGet packages are installed: `dotnet restore`
2. Check that .NET SDK version is 8.0 or higher
3. Clean solution: `dotnet clean`

### Mock-related Failures
1. Verify interfaces match between mock and implementation
2. Check Setup() calls are correct
3. Ensure Verify() expectations are reasonable

### Database Tests Fail
1. In-memory database is isolated per test
2. Check that connection string is correct for in-memory DB
3. Ensure DbContext is properly disposed

## Future Enhancements

- [ ] Add performance benchmarking tests
- [ ] Add concurrency/threading tests
- [ ] Add API integration tests using TestServer
- [ ] Add data annotation validation tests
- [ ] Add database migration tests
- [ ] Add authentication/authorization tests

## References

- [XUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/Moq/moq4/wiki/Quickstart)
- [EF Core Testing](https://docs.microsoft.com/en-us/ef/core/testing/)
- [ASP.NET Core Testing](https://docs.microsoft.com/en-us/aspnet/core/test/unit-tests-with-nunit)
