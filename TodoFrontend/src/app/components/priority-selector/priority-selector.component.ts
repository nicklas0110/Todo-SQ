import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Priority } from '../../models/todo.interface';

@Component({
  selector: 'app-priority-selector',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './priority-selector.component.html',
  styleUrls: ['./priority-selector.component.css']
})
export class PrioritySelectorComponent {
  @Input() priority!: Priority;
  @Output() priorityChange = new EventEmitter<Priority>();

  Priority = Priority;

  onPriorityChange(value: Priority) {
    this.priorityChange.emit(value);
  }
} 