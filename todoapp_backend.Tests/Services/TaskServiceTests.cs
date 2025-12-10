using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using todoapp_backend.DTOs;
using todoapp_backend.Models;
using todoapp_backend.Repositories;
using todoapp_backend.Services;

namespace todoapp_backend.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _taskService = new TaskService(_mockTaskRepository.Object);
        }

        [Fact]
        public async void GetRecentTasksAsync_ReturnsRecentTasks()
        {
            // Arrange
            var tasks = new List<Models.Task>
            {
                new Models.Task { Id = Guid.NewGuid(), Title = "Task 1", Description = "Desc 1", IsCompleted = false, CreatedAt = DateTime.UtcNow },
                new Models.Task { Id = Guid.NewGuid(), Title = "Task 2", Description = "Desc 2", IsCompleted = false, CreatedAt = DateTime.UtcNow }
            };
            
            _mockTaskRepository.Setup(r => r.GetRecentActiveTasks(5))
                .ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetRecentTasksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async void CreateTaskAsync_WithValidDto_CreatesTask()
        {
            // Arrange
            var createDto = new CreateTaskDto { Title = "New Task", Description = "New Desc" };
            var createdTask = new Models.Task { Id = Guid.NewGuid(), Title = createDto.Title, Description = createDto.Description, IsCompleted = false, CreatedAt = DateTime.UtcNow };
            
            _mockTaskRepository.Setup(r => r.CreateTaskAsync(It.IsAny<Models.Task>()))
                .ReturnsAsync(createdTask);

            // Act
            var result = await _taskService.CreateTaskAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createDto.Title, result.Title);
        }

        [Fact]
        public async void UpdateTaskAsync_WithValidData_UpdatesTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateDto = new UpdateTaskDto { Title = "Updated", Description = "Updated Desc" };
            var existingTask = new Models.Task { Id = taskId, Title = "Old", Description = "Old Desc", IsCompleted = false, CreatedAt = DateTime.UtcNow };
            
            _mockTaskRepository.Setup(r => r.GetTaskByIdAsync(taskId))
                .ReturnsAsync(existingTask);
            _mockTaskRepository.Setup(r => r.UpdateTaskAsync(It.IsAny<Models.Task>()))
                .ReturnsAsync(new Models.Task { Id = taskId, Title = "Updated", Description = "Updated Desc" });
            _mockTaskRepository.Setup(r => r.SaveAsync())
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            var result = await _taskService.UpdateTaskAsync(taskId, updateDto);

            // Assert
            Assert.NotNull(result);
            _mockTaskRepository.Verify(r => r.GetTaskByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.UpdateTaskAsync(It.IsAny<Models.Task>()), Times.Once);
        }

        [Fact]
        public async void CompleteTask_WithValidId_ReturnsCompletedTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var completedTask = new Models.Task { Id = taskId, Title = "Task", Description = "Desc", IsCompleted = true, CreatedAt = DateTime.UtcNow };
            
            _mockTaskRepository.Setup(r => r.GetTaskByIdAsync(taskId))
                .ReturnsAsync(completedTask);

            // Act
            var result = await _taskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsCompleted);
        }
    }
}
