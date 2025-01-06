import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { TodoService } from '../../services/todo.service';
import { TodoItemComponent } from '../todo-item/todo-item.component';
import { Todo, Priority } from '../../models/todo.interface';

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule, TodoItemComponent],
  templateUrl: './todo-list.component.html',
  styleUrls: ['./todo-list.component.css']
})
export class TodoListComponent implements OnInit {
  todos$ = this.todoService.getTodos();
  newTodoTitle = '';

  constructor(private todoService: TodoService) {}

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.todos$ = this.todoService.getTodos();
  }

  addTodo(): void {
    if (this.newTodoTitle.trim()) {
      this.todoService.addTodo(this.newTodoTitle).subscribe(() => {
        this.loadTodos();
        this.newTodoTitle = '';
      });
    }
  }

  onToggleTodo(todo: Todo): void {
    this.todoService.toggleTodo(todo).subscribe(() => {
      this.loadTodos();
    });
  }

  onDeleteTodo(id: number): void {
    this.todoService.deleteTodo(id).subscribe(() => {
      this.loadTodos();
    });
  }

  onUpdatePriority(event: {id: number, priority: Priority}): void {
    this.todoService.updatePriority(event.id, event.priority).subscribe(() => {
      this.loadTodos();
    });
  }

  onUpdateDeadline(event: {id: number, deadline?: Date}): void {
    this.todoService.updateDeadline(event.id, event.deadline).subscribe(() => {
      this.loadTodos();
    });
  }

  onUpdateTitle(event: {id: number, title: string}): void {
    this.todoService.updateTitle(event.id, event.title).subscribe(() => {
      this.loadTodos();
    });
  }
}
