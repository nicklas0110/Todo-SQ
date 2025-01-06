import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-deadline-selector',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule],
  templateUrl: './deadline-selector.component.html',
  styleUrls: ['./deadline-selector.component.css']
})
export class DeadlineSelectorComponent {
  @Input() deadline?: Date;
  @Output() deadlineChange = new EventEmitter<Date | undefined>();
  
  isEditing = false;
  selectedDate?: string;

  getDeadlineClass(): string {
    if (!this.deadline) return '';
    
    const now = new Date();
    const deadlineDate = new Date(this.deadline);
    const timeDiff = deadlineDate.getTime() - now.getTime();
    const hoursDiff = timeDiff / (1000 * 60 * 60);

    if (timeDiff < 0) return 'deadline-overdue';
    if (hoursDiff <= 24) return 'deadline-soon';
    return 'deadline-set';
  }

  toggleEdit(): void {
    this.isEditing = !this.isEditing;
    if (this.isEditing && this.deadline) {
      this.selectedDate = this.formatDateForInput(this.deadline);
    }
  }

  onDateChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const newDate = input.value ? new Date(input.value) : undefined;
    this.deadlineChange.emit(newDate);
    this.isEditing = false;
  }

  clearDeadline(): void {
    this.deadlineChange.emit(undefined);
    this.isEditing = false;
  }

  private formatDateForInput(date: Date): string {
    return new Date(date).toISOString().split('T')[0];
  }
} 