using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using todoapp_backend.Data;
using todoapp_backend.Models;
using todoapp_backend.Repositories;

namespace todoapp_backend.Tests.Repositories
{
    public class TaskRepositoryTests
    {
        private TodoDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: $"TodoDb_{Guid.NewGuid()}")
                .Options;
            return new TodoDbContext(options);
        }

        [Fact]
        public async void GetRecentActiveTasks_ReturnsIncompleteTasks()
        {
            // Arrange
            var context = GetContext();
            var repository = new TaskRepository(context);
            var tasks = new List<Models.Task>
            {
                new Models.Task { Id = Guid.NewGuid(), Title = "Task 1", Description = "Desc 1", IsCompleted = false, CreatedAt = DateTime.UtcNow },
                new Models.Task { Id = Guid.NewGuid(), Title = "Task 2", Description = "Desc 2", IsCompleted = false, CreatedAt = DateTime.UtcNow },
                new Models.Task { Id = Guid.NewGuid(), Title = "Completed", Description = "Desc 3", IsCompleted = true, CreatedAt = DateTime.UtcNow }
            };

            context.Tasks.AddRange(tasks);
            context.SaveChanges();

            // Act
            var result = await repository.GetRecentActiveTasks(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, task => Assert.False(task.IsCompleted));
        }

        [Fact]
        public async void CreateTaskAsync_WithValidTask_CreatesTask()
        {
            // Arrange
            var context = GetContext();
            var repository = new TaskRepository(context);
            var task = new Models.Task { Id = Guid.NewGuid(), Title = "New Task", Description = "New Desc", IsCompleted = false, CreatedAt = DateTime.UtcNow };

            // Act
            var result = await repository.CreateTaskAsync(task);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Task", result.Title);
        }

        [Fact]
        public async void GetTaskByIdAsync_WithValidId_ReturnsTask()
        {
            // Arrange
            var context = GetContext();
            var repository = new TaskRepository(context);
            var taskId = Guid.NewGuid();
            var task = new Models.Task { Id = taskId, Title = "Task", Description = "Desc", IsCompleted = false, CreatedAt = DateTime.UtcNow };
            context.Tasks.Add(task);
            context.SaveChanges();

            // Act
            var result = await repository.GetTaskByIdAsync(task.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Task", result.Title);
        }

        [Fact]
        public async void UpdateTaskAsync_WithValidData_UpdatesTask()
        {
            // Arrange
            var context = GetContext();
            var repository = new TaskRepository(context);
            var taskId = Guid.NewGuid();
            var task = new Models.Task { Id = taskId, Title = "Original", Description = "Original Desc", IsCompleted = false, CreatedAt = DateTime.UtcNow };
            context.Tasks.Add(task);
            context.SaveChanges();
            context.Entry(task).State = EntityState.Detached;

            var updated = new Models.Task { Id = taskId, Title = "Updated", Description = "Updated Desc", IsCompleted = false, CreatedAt = task.CreatedAt };

            // Act
            var result = await repository.UpdateTaskAsync(updated);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.Title);
        }

        [Fact]
        public async void TaskRepository_HandlesMultipleTasks()
        {
            // Arrange
            var context = GetContext();
            var repository = new TaskRepository(context);
            var tasks = new List<Models.Task>
            {
                new Models.Task { Id = Guid.NewGuid(), Title = "Task A", Description = "Desc A", IsCompleted = false, CreatedAt = DateTime.UtcNow },
                new Models.Task { Id = Guid.NewGuid(), Title = "Task B", Description = "Desc B", IsCompleted = false, CreatedAt = DateTime.UtcNow }
            };

            // Act
            foreach (var task in tasks)
            {
                await repository.CreateTaskAsync(task);
            }
            await repository.SaveAsync();
            var result = await repository.GetRecentActiveTasks(10);

            // Assert
            Assert.Equal(2, result.Count());
        }
    }
}

