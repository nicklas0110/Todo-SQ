export enum Priority {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3
}

export interface Todo {
  id?: number;
  title: string;
  description?: string;
  completed: boolean;
  createdAt: Date;
  updatedAt?: Date;
  deadline?: Date;
  priority: Priority;
}

// Helper function to convert string priority to enum
export function stringToPriority(priority: string): Priority {
  switch (priority.toLowerCase()) {
    case 'low': return Priority.Low;
    case 'medium': return Priority.Medium;
    case 'high': return Priority.High;
    case 'critical': return Priority.Critical;
    default: return Priority.Low;
  }
} 