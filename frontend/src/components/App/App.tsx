import { useState, useEffect } from 'react';

import './App.css';
import type { Task } from '../../types/task';
import { taskApiClient } from '../../services/taskApiClient';
import { TaskForm } from '../TaskForm/TaskForm';
import { TaskList } from '../TaskList/TaskList';

/**
 * Main application component
 */
function App() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // Load tasks on component mount
  useEffect(() => {
    loadTasks();
  }, []);

  const loadTasks = async () => {
    setLoading(true);
    setError('');
    try {
      const fetchedTasks = await taskApiClient.getRecentTasks(5);
      setTasks(fetchedTasks);
    } catch (err) {
      setError('Failed to load tasks. Please refresh the page.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateTask = async (title: string, description: string) => {
    try {
      const newTask = await taskApiClient.createTask({ title, description });
      setTasks([newTask, ...tasks.slice(0, 4)]);
    } catch (err) {
      throw err;
    }
  };

  const handleCompleteTask = async (id: string) => {
    try {
      await taskApiClient.completeTask(id);
      setTasks(tasks.filter((task) => task.id !== id));
    } catch (err) {
      setError('Failed to complete task. Please try again.');
      console.error(err);
      throw err;
    }
  };

  return (
    <div className="app">
      <header className="app-header">
        <h1>My Todo Tasks</h1>
        <p>Stay organized and get things done</p>
      </header>

      <main className="app-main">
        <div className="container">
          {error && (
            <div className="error-banner">
              {error}
              <button onClick={() => setError('')} className="close-btn">
                ×
              </button>
            </div>
          )}

          <div className="content-wrapper">
            <section className="form-section">
              <TaskForm onSubmit={handleCreateTask} isLoading={loading} />
            </section>

            <section className="list-section">
              <TaskList tasks={tasks} onTaskComplete={handleCompleteTask} isLoading={loading} />
            </section>
          </div>
        </div>
      </main>

      <footer className="app-footer">
        <p>&copy; 2024 Todo App. Stay productive!</p>
      </footer>
    </div>
  );
}

export default App;
