import { Component, Input, Output, EventEmitter, ElementRef, ViewChild, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { Todo, Priority } from '../../models/todo.interface';
import { PrioritySelectorComponent } from '../priority-selector/priority-selector.component';
import { DeadlineSelectorComponent } from '../deadline-selector/deadline-selector.component';

@Component({
  selector: 'app-todo-item',
  standalone: true,
  imports: [CommonModule, MatIconModule, PrioritySelectorComponent, DeadlineSelectorComponent],
  templateUrl: './todo-item.component.html',
  styleUrls: ['./todo-item.component.css']
})
export class TodoItemComponent implements AfterViewChecked {
  @Input() todo!: Todo;
  @Output() toggleTodo = new EventEmitter<Todo>();
  @Output() deleteTodo = new EventEmitter<number>();
  @Output() updatePriority = new EventEmitter<{id: number, priority: Priority}>();
  @Output() updateDeadline = new EventEmitter<{id: number, deadline?: Date}>();
  @Output() updateTitle = new EventEmitter<{id: number, title: string}>();
  
  @ViewChild('titleInput') titleInput?: ElementRef;
  
  Priority = Priority;
  isEditingPriority = false;
  isEditingTitle = false;
  currentTitleLength = 0;

  ngAfterViewChecked() {
    if (this.isEditingTitle && this.titleInput) {
      this.titleInput.nativeElement.focus();
    }
  }

  onTitleInput(value: string): void {
    this.currentTitleLength = value.length;
  }

  toggleTitleEdit(): void {
    this.isEditingTitle = !this.isEditingTitle;
    if (this.isEditingTitle) {
      this.currentTitleLength = this.todo.title.length;
    } else {
      this.currentTitleLength = 0;
    }
  }

  onTitleChange(newTitle: string): void {
    if (this.todo.id && newTitle.trim() && newTitle !== this.todo.title) {
      this.updateTitle.emit({ id: this.todo.id, title: newTitle.trim() });
    }
    this.isEditingTitle = false;
  }

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
      this.isEditingPriority = false;
    }
  }

  onDeadlineChange(newDeadline?: Date) {
    if (this.todo.id) {
      this.updateDeadline.emit({ id: this.todo.id, deadline: newDeadline });
    }
  }

  onToggle(): void {
    this.toggleTodo.emit(this.todo);
  }

  onDelete(): void {
    this.deleteTodo.emit(this.todo.id);
  }
}
