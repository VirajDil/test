using todoapp_backend.DTOs;

namespace todoapp_backend.ITaskServices
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetRecentTasksAsync(int count = 5);

        Task<IEnumerable<TaskDto>> GetAllTasksAsync();

        Task<TaskDto?> GetTaskByIdAsync(Guid id);

        Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto);

        Task<TaskDto> UpdateTaskAsync(Guid id, UpdateTaskDto updateTaskDto);

        Task<TaskDto> MarkTaskAsCompletedAsync(Guid id);

        System.Threading.Tasks.Task DeleteTaskAsync(Guid id);
    }
}
