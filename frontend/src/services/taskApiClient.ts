import axios, { type AxiosInstance } from 'axios';
import type { CreateTaskDto, Task } from '../types/task';

class TaskApiClient {
  private client: AxiosInstance;
  private baseURL: string;

  constructor() {
    // Use /api for Docker (proxied by nginx) or environment variable
    this.baseURL = 'http://localhost:5000/api/v1';

    this.client = axios.create({
      baseURL: this.baseURL,
      headers: {
        'Content-Type': 'application/json',
      },
    });
  }

    async getRecentTasks(count: number = 5): Promise<Task[]> {
    try {
      const response = await this.client.get<Task[]>('/tasks/GetRecentTasks', {
        params: { count },
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching tasks:', error);
      throw error;
    }
  }  

  async createTask(createTaskDto: CreateTaskDto): Promise<Task> {
    try {
      const response = await this.client.post<Task>('/tasks/CreateTask', createTaskDto);
      return response.data;
    } catch (error) {
      console.error('Error creating task:', error);
      throw error;
    }
  }

  async completeTask(id: string): Promise<Task> {
    try {
      const response = await this.client.post<Task>(`/tasks/CompleteTask/${id}/complete`);
      return response.data;
    } catch (error) {
      console.error(`Error completing task ${id}:`, error);
      throw error;
    }
  }
}

export const taskApiClient = new TaskApiClient();
