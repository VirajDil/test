using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using todoapp_backend.Data;
using todoapp_backend.Models;
using todoapp_backend.Repositories;
using TaskModel = todoapp_backend.Models.Task;

namespace todoapp_backend.IntegrationTests.Repositories
{
    public class TaskRepositoryIntegrationTests : IAsyncLifetime
    {
        private readonly TodoDbContext _dbContext;
        private readonly TaskRepository _taskRepository;
        private readonly string _dbName = "RepositoryTest_" + Guid.NewGuid();

        public TaskRepositoryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: _dbName)
                .Options;

            _dbContext = new TodoDbContext(options);
            _taskRepository = new TaskRepository(_dbContext);
        }

        public async global::System.Threading.Tasks.Task InitializeAsync()
        {
            // Setup code - can be used to seed initial data if needed
            await _dbContext.Database.EnsureCreatedAsync();
        }

        public async global::System.Threading.Tasks.Task DisposeAsync()
        {
            // Cleanup code
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.DisposeAsync();
        }

        [Fact]
        public async global::System.Threading.Tasks.Task CreateTaskAsync_WithValidTask_ShouldCreateAndReturnTask()
        {
            // Arrange
            var newTask = new TaskModel
            {
                Id = Guid.NewGuid(),
                Title = "Integration Test Task",
                Description = "Test Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _taskRepository.CreateTaskAsync(newTask);
            await _taskRepository.SaveAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newTask.Id, result.Id);
            Assert.Equal(newTask.Title, result.Title);
            Assert.Single(await _dbContext.Tasks.ToListAsync());
        }

        [Fact]
        public async global::System.Threading.Tasks.Task GetTaskByIdAsync_WithExistingTask_ShouldReturnTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskModel
            {
                Id = taskId,
                Title = "Task to Retrieve",
                Description = "Test Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _taskRepository.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskId, result.Id);
            Assert.Equal("Task to Retrieve", result.Title);
        }

        [Fact]
        public async global::System.Threading.Tasks.Task GetTaskByIdAsync_WithNonExistentTask_ShouldReturnNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _taskRepository.GetTaskByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async global::System.Threading.Tasks.Task GetAllTasksAsync_ShouldReturnAllTasks()
        {
            // Arrange
            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = Guid.NewGuid(), Title = "Task 1", Description = "Desc 1", IsCompleted = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new TaskModel { Id = Guid.NewGuid(), Title = "Task 2", Description = "Desc 2", IsCompleted = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new TaskModel { Id = Guid.NewGuid(), Title = "Task 3", Description = "Desc 3", IsCompleted = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            _dbContext.Tasks.AddRange(tasks);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _taskRepository.GetAllTasksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async global::System.Threading.Tasks.Task UpdateTaskAsync_WithValidTask_ShouldUpdateTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskModel
            {
                Id = taskId,
                Title = "Original Title",
                Description = "Original Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            task.Title = "Updated Title";
            task.Description = "Updated Description";
            task.UpdatedAt = DateTime.UtcNow;

            // Act
            var result = await _taskRepository.UpdateTaskAsync(task);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Title", result.Title);
            Assert.Equal("Updated Description", result.Description);

            var updatedTaskInDb = await _dbContext.Tasks.FirstAsync(t => t.Id == taskId);
            Assert.Equal("Updated Title", updatedTaskInDb.Title);
        }

        [Fact]
        public async global::System.Threading.Tasks.Task DeleteTaskAsync_WithExistingTask_ShouldDeleteTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskModel
            {
                Id = taskId,
                Title = "Task to Delete",
                Description = "Will be deleted",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            // Act
            await _taskRepository.DeleteTaskAsync(taskId);

            // Assert
            var deletedTask = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
            Assert.Null(deletedTask);
        }

        [Fact]
        public async global::System.Threading.Tasks.Task GetRecentActiveTasks_ShouldReturnRecentIncompleteTasks()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = Guid.NewGuid(), Title = "Recent Task 1", IsCompleted = false, CreatedAt = now, UpdatedAt = now },
                new TaskModel { Id = Guid.NewGuid(), Title = "Recent Task 2", IsCompleted = false, CreatedAt = now.AddDays(-1), UpdatedAt = now.AddDays(-1) },
                new TaskModel { Id = Guid.NewGuid(), Title = "Completed Task", IsCompleted = true, CreatedAt = now, UpdatedAt = now },
                new TaskModel { Id = Guid.NewGuid(), Title = "Old Task", IsCompleted = false, CreatedAt = now.AddDays(-10), UpdatedAt = now.AddDays(-10) }
            };

            _dbContext.Tasks.AddRange(tasks);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _taskRepository.GetRecentActiveTasks(5);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.DoesNotContain(resultList, t => t.IsCompleted); // All should be active (not completed)
        }

        [Fact]
        public async global::System.Threading.Tasks.Task SaveAsync_ShouldPersistChanges()
        {
            // Arrange
            var task = new TaskModel
            {
                Id = Guid.NewGuid(),
                Title = "Save Test Task",
                Description = "Testing Save",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Tasks.Add(task);

            // Act
            await _taskRepository.SaveAsync();

            // Assert
            var savedTask = await _dbContext.Tasks.FirstAsync(t => t.Id == task.Id);
            Assert.NotNull(savedTask);
            Assert.Equal("Save Test Task", savedTask.Title);
        }

        [Fact]
        public async global::System.Threading.Tasks.Task BulkOperations_CreateMultipleUpdateAndDelete()
        {
            // Arrange - Create multiple tasks
            var tasksToCreate = new List<TaskModel>
            {
                new TaskModel
                {
                    Title = "Bulk Task 1",
                    Description = "Bulk operation test",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TaskModel
                {
                    Title = "Bulk Task 2",
                    Description = "Bulk operation test",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TaskModel
                {
                    Title = "Bulk Task 3",
                    Description = "Bulk operation test",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            // Act - Create
            var createdTasks = new List<TaskModel>();
            foreach (var task in tasksToCreate)
            {
                var created = await _taskRepository.CreateTaskAsync(task);
                createdTasks.Add(created);
            }
            await _taskRepository.SaveAsync();

            var allTasks = await _taskRepository.GetAllTasksAsync();
            Assert.Equal(3, allTasks.Count());

            // Act - Update first task
            var firstTask = createdTasks[0];
            firstTask.Title = "Updated Bulk Task";
            firstTask.IsCompleted = true;
            await _taskRepository.UpdateTaskAsync(firstTask);
            await _taskRepository.SaveAsync();

            // Act - Delete second task
            await _taskRepository.DeleteTaskAsync(createdTasks[1].Id);

            // Assert
            var remainingTasks = await _taskRepository.GetAllTasksAsync();
            Assert.Equal(2, remainingTasks.Count());

            var updated = await _taskRepository.GetTaskByIdAsync(createdTasks[0].Id);
            Assert.NotNull(updated);
            Assert.True(updated.IsCompleted);
            Assert.Equal("Updated Bulk Task", updated.Title);

            var deleted = await _taskRepository.GetTaskByIdAsync(createdTasks[1].Id);
            Assert.Null(deleted);
        }
    }
}
