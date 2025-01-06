Feature: Todo Management
    As a user
    I want to manage my todos
    So that I can keep track of my tasks

    Scenario: Create a new todo
        Given I have a new todo with title "Test Todo"
        When I create the todo
        Then the todo should be created successfully
        And the todo should have title "Test Todo"

    Scenario: Update todo title
        Given I have an existing todo with title "Original Title"
        When I update the todo title to "Updated Title"
        Then the todo should be updated successfully
        And the todo should have title "Updated Title"

    Scenario: Delete todo
        Given I have an existing todo with title "To Delete"
        When I delete the todo
        Then the todo should be deleted successfully 