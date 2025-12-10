using Microsoft.EntityFrameworkCore;
using todoapp_backend.Data;
using todoapp_backend.Models;

namespace todoapp_backend.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TodoDbContext _context;
        
    public TaskRepository(TodoDbContext context)
    {
        _context = context;
    }
    public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetRecentActiveTasks(int count = 5)
    {
        return await _context.Tasks
            .Where(t => !t.IsCompleted)
            .OrderByDescending(t => t.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetAllTasksAsync()
    {
        return await _context.Tasks.ToListAsync();
    }

    public async System.Threading.Tasks.Task<Models.Task?> GetTaskByIdAsync(Guid id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async System.Threading.Tasks.Task<Models.Task> CreateTaskAsync(Models.Task task)
    {
        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.Tasks.AddAsync(task);
        return task;
    }

    public async System.Threading.Tasks.Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task<Models.Task> UpdateTaskAsync(Models.Task task)
    {
        task.UpdatedAt = DateTime.UtcNow;
        _context.Tasks.Update(task);
        return await System.Threading.Tasks.Task.FromResult(task);
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
