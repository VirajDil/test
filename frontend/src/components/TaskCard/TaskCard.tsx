import React from 'react';
import './TaskCard.css';
import type { Task } from '../../types/task';

interface TaskCardProps {
  task: Task;
  onComplete: (id: string) => Promise<void>;
  isLoading?: boolean;
}


export const TaskCard: React.FC<TaskCardProps> = ({ task, onComplete, isLoading = false }) => {
  const handleComplete = async () => {  
      try {
      await onComplete(task.id);
    } catch (error) {
      console.error('Error completing task:', error);
    }
  };

  const formattedDate = new Date(task.createdAt).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });

  return (
    <div className="task-card">
      <div className="task-header">
        <h3 className="task-title">{task.title}</h3>
        <span className="task-date">{formattedDate}</span>
      </div>
      <p className="task-description">{task.description}</p>
      <div className="task-actions">
        <button
          className="done-btn"
          onClick={handleComplete}
          disabled={isLoading}
          title="Mark as completed"
        >
          {isLoading ? 'Completing...' : 'Done'}
        </button>
      </div>
    </div>
  );
};
