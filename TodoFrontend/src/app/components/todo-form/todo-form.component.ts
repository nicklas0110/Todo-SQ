import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-todo-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './todo-form.component.html',
  styleUrls: ['./todo-form.component.css']
})
export class TodoFormComponent {
  @Output() addTodo = new EventEmitter<string>();
  title = '';

  onSubmit() {
    if (this.title.trim()) {
      this.addTodo.emit(this.title.trim());
      this.title = '';
    }
  }
} 