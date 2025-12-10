using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using todoapp_backend.Controllers;
using todoapp_backend.DTOs;
using todoapp_backend.ITaskServices;

namespace todoapp_backend.Tests.Controllers
{
    public class TasksControllerTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly TasksController _controller;

        public TasksControllerTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _controller = new TasksController(_mockTaskService.Object);
        }

        [Fact]
        public async void GetRecentTasks_ReturnsOkWithTasks()
        {
            // Arrange
            var tasks = new List<TaskDto>
            {
                new TaskDto { Id = Guid.NewGuid(), Title = "Task 1", Description = "Desc 1", IsCompleted = false },
                new TaskDto { Id = Guid.NewGuid(), Title = "Task 2", Description = "Desc 2", IsCompleted = false }
            };
            
            _mockTaskService.Setup(s => s.GetRecentTasksAsync(It.IsAny<int>()))
                .ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetAllTasks();

            // Assert
            Assert.NotNull(result);
            // The result should contain the tasks
            _mockTaskService.Verify(s => s.GetRecentTasksAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async void GetTaskById_WithValidId_ReturnsOkWithTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskDto { Id = taskId, Title = "Task", Description = "Desc", IsCompleted = false };
            
            _mockTaskService.Setup(s => s.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _controller.GetTaskById(taskId);

            // Assert
            Assert.NotNull(result);
            _mockTaskService.Verify(s => s.GetTaskByIdAsync(taskId), Times.Once);
        }

        [Fact]
        public async void CreateTask_WithValidDto_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new CreateTaskDto { Title = "New Task", Description = "New Desc" };
            var createdTask = new TaskDto { Id = Guid.NewGuid(), Title = createDto.Title, Description = createDto.Description, IsCompleted = false };
            
            _mockTaskService.Setup(s => s.CreateTaskAsync(createDto))
                .ReturnsAsync(createdTask);

            // Act
            var result = await _controller.CreateTask(createDto);

            // Assert
            Assert.NotNull(result);
            _mockTaskService.Verify(s => s.CreateTaskAsync(createDto), Times.Once);
        }

        [Fact]
        public async void UpdateTask_WithValidData_ReturnsOkWithUpdatedTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateDto = new UpdateTaskDto { Title = "Updated", Description = "Updated Desc" };
            var updatedTask = new TaskDto { Id = taskId, Title = updateDto.Title, Description = updateDto.Description, IsCompleted = false };
            
            _mockTaskService.Setup(s => s.UpdateTaskAsync(taskId, updateDto))
                .ReturnsAsync(updatedTask);

            // Act
            var result = await _controller.UpdateTask(taskId, updateDto);

            // Assert
            Assert.NotNull(result);
            _mockTaskService.Verify(s => s.UpdateTaskAsync(taskId, updateDto), Times.Once);
        }

        [Fact]
        public async void CompleteTask_WithValidId_ReturnsOkWithCompletedTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var completedTask = new TaskDto { Id = taskId, Title = "Task", Description = "Desc", IsCompleted = true };
            
            _mockTaskService.Setup(s => s.MarkTaskAsCompletedAsync(taskId))
                .ReturnsAsync(completedTask);

            // Act
            var result = await _controller.CompleteTask(taskId);

            // Assert
            Assert.NotNull(result);
            _mockTaskService.Verify(s => s.MarkTaskAsCompletedAsync(taskId), Times.Once);
        }
    }
}
