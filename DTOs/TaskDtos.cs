namespace todoapp_backend.DTOs;


public class TaskDto
{    
    public Guid Id { get; set; }
        
    public string Title { get; set; } = string.Empty;
        
    public string Description { get; set; } = string.Empty;
        
    public bool IsCompleted { get; set; }
        
    public DateTime CreatedAt { get; set; }
}

public class CreateTaskDto
{    
    public string Title { get; set; } = string.Empty;
        
    public string Description { get; set; } = string.Empty;
}

public class UpdateTaskDto
{    
    public string? Title { get; set; }
        
    public string? Description { get; set; }
        
    public bool? IsCompleted { get; set; }
}

