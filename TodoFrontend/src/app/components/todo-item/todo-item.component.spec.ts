import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TodoItemComponent } from './todo-item.component';
import { MatIconModule } from '@angular/material/icon';
import { Priority } from '../../models/todo.interface';

describe('TodoItemComponent', () => {
  let component: TodoItemComponent;
  let fixture: ComponentFixture<TodoItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TodoItemComponent, MatIconModule]
    }).compileComponents();

    fixture = TestBed.createComponent(TodoItemComponent);
    component = fixture.componentInstance;
    component.todo = {
      id: 1,
      title: 'Test Todo',
      completed: false,
      priority: Priority.Medium,
      createdAt: new Date()
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit title update when title changes', () => {
    spyOn(component.updateTitle, 'emit');
    component.onTitleChange('New Title');
    expect(component.updateTitle.emit).toHaveBeenCalledWith({
      id: 1,
      title: 'New Title'
    });
  });

  it('should emit priority update when priority changes', () => {
    spyOn(component.updatePriority, 'emit');
    component.onPriorityChange(Priority.High);
    expect(component.updatePriority.emit).toHaveBeenCalledWith({
      id: 1,
      priority: Priority.High
    });
  });

  it('should emit deadline update when deadline changes', () => {
    spyOn(component.updateDeadline, 'emit');
    const newDate = new Date();
    component.onDeadlineChange(newDate);
    expect(component.updateDeadline.emit).toHaveBeenCalledWith({
      id: 1,
      deadline: newDate
    });
  });

  it('should validate title length', () => {
    const longTitle = 'a'.repeat(26);
    component.onTitleInput(longTitle);
    expect(component.currentTitleLength).toBe(26);
  });
});
