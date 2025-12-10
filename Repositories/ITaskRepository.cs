using todoapp_backend.Models;

namespace todoapp_backend.Repositories;

public interface ITaskRepository
{    
    System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetRecentActiveTasks(int count = 5);

    System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetAllTasksAsync();

    System.Threading.Tasks.Task<Models.Task?> GetTaskByIdAsync(Guid id);

    System.Threading.Tasks.Task<Models.Task> CreateTaskAsync(Models.Task task);

    System.Threading.Tasks.Task SaveAsync();

    System.Threading.Tasks.Task<Models.Task> UpdateTaskAsync(Models.Task task);

    System.Threading.Tasks.Task DeleteTaskAsync(Guid id);
}
