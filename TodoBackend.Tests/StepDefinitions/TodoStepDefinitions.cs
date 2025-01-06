using TechTalk.SpecFlow;
using TodoBackend.Models;
using TodoBackend.Services;
using TodoBackend.Data;
using TodoBackend.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace TodoBackend.Tests.StepDefinitions
{
    [Binding]
    public class TodoStepDefinitions
    {
        private readonly TodoService _service;
        private Todo? _todo;
        private Todo? _result;

        public TodoStepDefinitions()
        {
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new TodoDbContext(options);
            var repository = new TodoRepository(context);
            _service = new TodoService(repository);
        }

        [Given(@"I have a new todo with title ""(.*)""")]
        public void GivenIHaveANewTodoWithTitle(string title)
        {
            _todo = new Todo { Title = title };
        }

        [Given(@"I have an existing todo with title ""(.*)""")]
        public async Task GivenIHaveAnExistingTodoWithTitle(string title)
        {
            _todo = await _service.CreateTodoAsync(new Todo { Title = title });
        }

        [When(@"I create the todo")]
        public async Task WhenICreateTheTodo()
        {
            _result = await _service.CreateTodoAsync(_todo!);
        }

        [When(@"I update the todo title to ""(.*)""")]
        public async Task WhenIUpdateTheTodoTitleTo(string newTitle)
        {
            _result = await _service.UpdateTodoTitleAsync(_todo!.Id, newTitle);
        }

        [When(@"I delete the todo")]
        public async Task WhenIDeleteTheTodo()
        {
            await _service.DeleteTodoAsync(_todo!.Id);
        }

        [Then(@"the todo should be created successfully")]
        public void ThenTheTodoShouldBeCreatedSuccessfully()
        {
            _result.Should().NotBeNull();
        }

        [Then(@"the todo should have title ""(.*)""")]
        public void ThenTheTodoShouldHaveTitle(string expectedTitle)
        {
            _result!.Title.Should().Be(expectedTitle);
        }

        [Then(@"the todo should be deleted successfully")]
        public async Task ThenTheTodoShouldBeDeletedSuccessfully()
        {
            var todo = await _service.GetTodoByIdAsync(_todo!.Id);
            todo.Should().BeNull();
        }

        [Then(@"the todo should be updated successfully")]
        public void ThenTheTodoShouldBeUpdatedSuccessfully()
        {
            _result.Should().NotBeNull();
            _result!.UpdatedAt.Should().NotBe(default(DateTime));
        }
    }
} 