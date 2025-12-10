import { useState } from 'react';
import './TaskList.css';
import type { Task } from '../../types/task';
import { TaskCard } from '../TaskCard/TaskCard';

interface TaskListProps {
  tasks: Task[];
  onTaskComplete: (id: string) => Promise<void>;
  isLoading?: boolean;
}

export const TaskList: React.FC<TaskListProps> = ({ tasks, onTaskComplete, isLoading = false }) => {
  const [completingTaskId, setCompletingTaskId] = useState<string | null>(null);

  const handleComplete = async (id: string) => {
    setCompletingTaskId(id);
    try {
      await onTaskComplete(id);
    } finally {
      setCompletingTaskId(null);
    }
  };

  if (tasks.length === 0) {
    return (
      <div className="task-list empty">
        <p className="empty-message">No active tasks yet. Create one to get started!</p>
      </div>
    );
  }

  return (
    <div className="task-list">
      <h2 className="task-list-title">Active Tasks (Latest {tasks.length})</h2>
      <div className="tasks-container">
        {tasks.map((task) => (
          <TaskCard
            key={task.id}
            task={task}
            onComplete={handleComplete}
            isLoading={completingTaskId === task.id || isLoading}
          />
        ))}
      </div>
    </div>
  );
};
