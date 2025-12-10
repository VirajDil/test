# Testing Structure

This project uses a separated testing approach with **Unit Tests** and **Integration Tests**.

## Project Organization

### 1. Unit Tests (`todoapp_backend.Tests`)
Located in `todoapp_backend.Tests/` directory

**Purpose:** Test individual units (classes/methods) in isolation using mocks

**Structure:**
- `Services/` - Unit tests for business logic services
- `Repositories/` - Unit tests for repository interfaces
- `Controllers/` - Unit tests for API controllers

**Key Characteristics:**
- Use **Moq** for mocking dependencies
- Test business logic without external dependencies
- Fast execution
- No database interaction (in-memory mocks)
- Focus on method behavior

**Example Tests:**
- `TaskServiceTests.cs` - Tests service methods with mocked repositories
- `TaskRepositoryTests.cs` - Tests repository methods with mocked DbContext
- `TasksControllerTests.cs` - Tests controller endpoints with mocked services

---

### 2. Integration Tests (`todoapp_backend.IntegrationTests`)
Located in `todoapp_backend.IntegrationTests/` directory

**Purpose:** Test components working together with real or in-memory database

**Structure:**
- `Controllers/` - Integration tests for API endpoints (end-to-end)
- `Repositories/` - Integration tests for data access layer with real database context
- `Fixtures/` - Test infrastructure and setup

**Key Characteristics:**
- Use **WebApplicationFactory** for HTTP testing
- Use **In-Memory Database** for database operations
- Test actual component integration
- Slower execution (more comprehensive)
- Real database interactions
- Focus on workflow and data persistence

**Example Tests:**
- `TasksControllerIntegrationTests.cs` - Tests full HTTP requests/responses
- `TaskRepositoryIntegrationTests.cs` - Tests repository with in-memory database

---

## Running Tests

### Run All Tests
```powershell
dotnet test
```

### Run Only Unit Tests
```powershell
dotnet test todoapp_backend.Tests/todoapp_backend.Tests.csproj
```

### Run Only Integration Tests
```powershell
dotnet test todoapp_backend.IntegrationTests/todoapp_backend.IntegrationTests.csproj
```

### Run with Coverage
```powershell
dotnet test /p:CollectCoverage=true
```

### Run Specific Test Class
```powershell
dotnet test --filter ClassName=TaskServiceTests
```

### Run with Detailed Output
```powershell
dotnet test --verbosity normal
```

---

## Key Differences

| Aspect | Unit Tests | Integration Tests |
|--------|------------|-------------------|
| **Scope** | Single unit/method | Multiple components |
| **Dependencies** | Mocked | Real or In-Memory |
| **Database** | No (mocked) | In-Memory |
| **Speed** | Fast | Slower |
| **Isolation** | Complete | Partial |
| **Tools** | Moq, xUnit | WebApplicationFactory, xUnit |
| **Focus** | Business logic | Workflows & Integration |
| **Examples** | Service logic, Repository logic | API endpoints, Data persistence |

---

## Test Fixtures

### CustomWebApplicationFactory
Located in `Fixtures/CustomWebApplicationFactory.cs`

Provides:
- In-memory database for integration tests
- HTTP client for making requests
- Automatic database creation and seeding

**Usage:**
```csharp
public class TasksControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    public TasksControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }
}
```

---

## Best Practices

### Unit Tests
✅ Do:
- Test one thing per test
- Use descriptive test names
- Mock external dependencies
- Test both success and failure paths
- Keep tests fast

❌ Don't:
- Access database
- Make HTTP calls
- Test multiple components
- Depend on test execution order

### Integration Tests
✅ Do:
- Test real workflows
- Use in-memory database
- Test API endpoints end-to-end
- Include error scenarios
- Test data persistence

❌ Don't:
- Mock everything (defeats the purpose)
- Make external API calls
- Test only happy paths
- Ignore cleanup

---

## Test Naming Convention

### Unit Test Method Names
`[MethodName]_[Scenario]_[ExpectedResult]`

Examples:
- `CreateTaskAsync_WithValidDto_CreatesTask`
- `UpdateTaskAsync_WithInvalidData_ThrowsException`
- `GetTaskById_WhenTaskNotFound_ReturnsNull`

### Integration Test Method Names
`[Operation]_[Scenario]_[ExpectedResult]`

Examples:
- `CreateTask_WithValidData_ShouldReturnCreatedStatusCode`
- `GetTaskById_WithValidId_ShouldReturnTask`
- `DeleteTask_WithValidId_ShouldReturnNoContent`

---

## Project Files

### Unit Tests
- `todoapp_backend.Tests.csproj` - Project file
- `Services/TaskServiceTests.cs` - Service logic tests
- `Repositories/TaskRepositoryTests.cs` - Repository pattern tests
- `Controllers/TasksControllerTests.cs` - Controller logic tests

### Integration Tests
- `todoapp_backend.IntegrationTests.csproj` - Project file
- `Fixtures/CustomWebApplicationFactory.cs` - Test infrastructure
- `Controllers/TasksControllerIntegrationTests.cs` - API endpoint tests
- `Repositories/TaskRepositoryIntegrationTests.cs` - Data layer tests

---

## Continuous Integration

Add to your CI/CD pipeline:

```yaml
- name: Run Unit Tests
  run: dotnet test todoapp_backend.Tests

- name: Run Integration Tests
  run: dotnet test todoapp_backend.IntegrationTests

- name: Generate Coverage
  run: dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

---

## Troubleshooting

### Tests fail with "The module could not be loaded"
- Ensure all NuGet packages are restored
- Run: `dotnet restore`

### In-Memory Database is shared between tests
- Each test uses a unique database name: `"IntegrationTest_" + Guid.NewGuid()`
- Implement `IAsyncLifetime` for proper cleanup

### Integration tests timeout
- Increase timeout in test configuration
- Check database operations aren't blocking

---

**Last Updated:** December 11, 2025
