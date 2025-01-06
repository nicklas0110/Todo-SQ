import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Todo, Priority } from '../../models/todo.interface';
import { PrioritySelectorComponent } from '../priority-selector/priority-selector.component';

@Component({
  selector: 'app-todo-item',
  standalone: true,
  imports: [CommonModule, PrioritySelectorComponent],
  templateUrl: './todo-item.component.html',
  styleUrls: ['./todo-item.component.css']
})
export class TodoItemComponent {
  @Input() todo!: Todo;
  @Output() toggleTodo = new EventEmitter<Todo>();
  @Output() deleteTodo = new EventEmitter<number>();
  @Output() updatePriority = new EventEmitter<{id: number, priority: Priority}>();
  
  Priority = Priority;
  isEditingPriority = false;

  getPriorityClass(): string {
    switch (this.todo.priority) {
      case Priority.Low:
        return 'priority-low';
      case Priority.Medium:
        return 'priority-medium';
      case Priority.High:
        return 'priority-high';
      case Priority.Critical:
        return 'priority-critical';
      default:
        return 'priority-low';
    }
  }

  getPriorityLabel(): string {
    switch (this.todo.priority) {
      case Priority.Low:
        return 'Low';
      case Priority.Medium:
        return 'Medium';
      case Priority.High:
        return 'High';
      case Priority.Critical:
        return 'Critical';
      default:
        return 'Low';
    }
  }

  togglePriorityEdit(): void {
    this.isEditingPriority = !this.isEditingPriority;
  }

  onPriorityChange(newPriority: Priority) {
    if (this.todo.id) {
      this.updatePriority.emit({ id: this.todo.id, priority: newPriority });
      this.isEditingPriority = false; // Hide selector after selection
    }
  }

  onToggle(): void {
    this.toggleTodo.emit(this.todo);
  }

  onDelete(): void {
    this.deleteTodo.emit(this.todo.id);
  }
}
