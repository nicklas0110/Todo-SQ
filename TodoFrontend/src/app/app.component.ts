import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TodoListComponent } from './components/todo-list/todo-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, TodoListComponent],
  template: `
    <div class="container">
      <app-todo-list></app-todo-list>
    </div>
  `,
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'TodoFrontend';
}
