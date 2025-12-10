using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using todoapp_backend.DTOs;
using todoapp_backend.IntegrationTests.Fixtures;

namespace todoapp_backend.IntegrationTests.Controllers
{
    public class TasksControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private const string BaseUrl = "/api/tasks";

        public TasksControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllTasks_ShouldReturnEmptyList_WhenDatabaseIsEmpty()
        {
            // Act
            var response = await _client.GetAsync(BaseUrl);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(content);
        }

        [Fact]
        public async Task CreateTask_WithValidData_ShouldReturnCreatedStatusCode()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto
            {
                Title = "Integration Test Task",
                Description = "This is an integration test task"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(createTaskDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync(BaseUrl, content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseContent);
        }

        [Fact]
        public async Task CreateTask_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidTaskDto = new CreateTaskDto
            {
                Title = "", // Invalid: Empty title
                Description = "Missing title"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(invalidTaskDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync(BaseUrl, content);

            // Assert
            // Should return either BadRequest or validation error
            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task GetTaskById_WithValidId_ShouldReturnTask()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto
            {
                Title = "Task to Retrieve",
                Description = "This task will be retrieved"
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(createTaskDto),
                Encoding.UTF8,
                "application/json");

            var createResponse = await _client.PostAsync(BaseUrl, createContent);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var createdContent = await createResponse.Content.ReadAsStringAsync();
            var createdTask = JsonSerializer.Deserialize<TaskDto>(createdContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdTask);
            Assert.NotEqual(Guid.Empty, createdTask.Id);

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{createdTask.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            var task = JsonSerializer.Deserialize<TaskDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(task);
            Assert.Equal(createdTask.Id, task.Id);
        }

        [Fact]
        public async Task UpdateTask_WithValidData_ShouldReturnUpdatedTask()
        {
            // Arrange - Create a task first
            var createTaskDto = new CreateTaskDto
            {
                Title = "Original Title",
                Description = "Original Description"
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(createTaskDto),
                Encoding.UTF8,
                "application/json");

            var createResponse = await _client.PostAsync(BaseUrl, createContent);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var createdContent = await createResponse.Content.ReadAsStringAsync();
            var createdTask = JsonSerializer.Deserialize<TaskDto>(createdContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdTask);
            Assert.NotEqual(Guid.Empty, createdTask.Id);

            // Update the task
            var updateTaskDto = new UpdateTaskDto
            {
                Title = "Updated Title",
                Description = "Updated Description"
            };

            var updateContent = new StringContent(
                JsonSerializer.Serialize(updateTaskDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PutAsync($"{BaseUrl}/{createdTask.Id}", updateContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var updatedContent = await response.Content.ReadAsStringAsync();
            var updatedTask = JsonSerializer.Deserialize<TaskDto>(updatedContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(updatedTask);
            Assert.Equal("Updated Title", updatedTask.Title);
        }

        [Fact]
        public async Task CompleteTask_WithValidId_ShouldMarkTaskAsComplete()
        {
            // Arrange - Create a task first
            var createTaskDto = new CreateTaskDto
            {
                Title = "Task to Complete",
                Description = "This task will be completed"
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(createTaskDto),
                Encoding.UTF8,
                "application/json");

            var createResponse = await _client.PostAsync(BaseUrl, createContent);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var createdContent = await createResponse.Content.ReadAsStringAsync();
            var createdTask = JsonSerializer.Deserialize<TaskDto>(createdContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdTask);
            Assert.NotEqual(Guid.Empty, createdTask.Id);

            // Act - POST to /api/tasks/{id}/complete
            var response = await _client.PostAsync($"{BaseUrl}/{createdTask.Id}/complete", new StringContent("", Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var completedContent = await response.Content.ReadAsStringAsync();
            var completedTask = JsonSerializer.Deserialize<TaskDto>(completedContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(completedTask);
            Assert.True(completedTask.IsCompleted);
        }

        [Fact]
        public async Task DeleteTask_WithValidId_ShouldReturnNoContent()
        {
            // Arrange - Create a task first
            var createTaskDto = new CreateTaskDto
            {
                Title = "Task to Delete",
                Description = "This task will be deleted"
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(createTaskDto),
                Encoding.UTF8,
                "application/json");

            var createResponse = await _client.PostAsync(BaseUrl, createContent);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var createdContent = await createResponse.Content.ReadAsStringAsync();
            var createdTask = JsonSerializer.Deserialize<TaskDto>(createdContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdTask);
            Assert.NotEqual(Guid.Empty, createdTask.Id);

            // Act
            var response = await _client.DeleteAsync($"{BaseUrl}/{createdTask.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task GetTaskById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{invalidId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task MultipleOperations_CreateUpdateDelete_ShouldWorkSequentially()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto
            {
                Title = "Multi-Op Task",
                Description = "For multiple operations test"
            };

            // Act & Assert - Create
            var createContent = new StringContent(
                JsonSerializer.Serialize(createTaskDto),
                Encoding.UTF8,
                "application/json");

            var createResponse = await _client.PostAsync(BaseUrl, createContent);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdContent = await createResponse.Content.ReadAsStringAsync();
            var createdTask = JsonSerializer.Deserialize<TaskDto>(createdContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdTask);
            Assert.NotEqual(Guid.Empty, createdTask.Id);

            // Assert - Get Created Task
            var getResponse = await _client.GetAsync($"{BaseUrl}/{createdTask.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            // Assert - Update
            var updateTaskDto = new UpdateTaskDto
            {
                Title = "Updated Multi-Op Task",
                Description = "Updated for testing"
            };

            var updateContent = new StringContent(
                JsonSerializer.Serialize(updateTaskDto),
                Encoding.UTF8,
                "application/json");

            var updateResponse = await _client.PutAsync($"{BaseUrl}/{createdTask.Id}", updateContent);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            // Assert - Delete
            var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/{createdTask.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Assert - Verify Deleted
            var getDeletedResponse = await _client.GetAsync($"{BaseUrl}/{createdTask.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
        }
    }
}
