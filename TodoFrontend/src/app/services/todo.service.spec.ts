import { TestBed } from '@angular/core/testing';

import { TodoService } from './todo.service';

describe('TodoService', () => {
  let service: TodoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TodoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should add a new todo', (done) => {
    service.getTodos().subscribe(todos => {
      expect(todos.length).toBe(0);
      done();
    });

    service.addTodo('Test Todo');
    
    service.getTodos().subscribe(todos => {
      expect(todos.length).toBe(1);
      expect(todos[0].title).toBe('Test Todo');
      done();
    });
  });
});
