import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Todo, Priority, stringToPriority } from '../models/todo.interface';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private apiUrl = 'http://localhost:5234/api/todos';

  constructor(private http: HttpClient) {}

  getTodos(): Observable<Todo[]> {
    return this.http.get<Todo[]>(this.apiUrl).pipe(
      map(todos => todos.map(todo => ({
        ...todo,
        priority: typeof todo.priority === 'string' ? stringToPriority(todo.priority as string) : todo.priority
      })))
    );
  }

  addTodo(title: string, description?: string): Observable<Todo> {
    const todo = {
      title,
      description,
      completed: false,
      priority: Priority.Low,
      createdAt: new Date()
    };
    
    return this.http.post<Todo>(this.apiUrl, todo).pipe(
      map(todo => ({
        ...todo,
        priority: typeof todo.priority === 'string' ? stringToPriority(todo.priority as string) : todo.priority
      }))
    );
  }

  toggleTodo(todo: Todo): Observable<Todo> {
    const updatedTodo = {
      ...todo,
      completed: !todo.completed
    };
    return this.http.put<Todo>(`${this.apiUrl}/${todo.id}`, updatedTodo).pipe(
      map(todo => ({
        ...todo,
        priority: typeof todo.priority === 'string' ? stringToPriority(todo.priority as string) : todo.priority
      }))
    );
  }

  updatePriority(id: number, priority: Priority): Observable<Todo> {
    return this.http.put<Todo>(`${this.apiUrl}/${id}/priority`, { priority }).pipe(
      map(todo => ({
        ...todo,
        priority: typeof todo.priority === 'string' ? stringToPriority(todo.priority as string) : todo.priority
      }))
    );
  }

  deleteTodo(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
