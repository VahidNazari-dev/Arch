Feature: Create Usage01
  Scenario: Successful Usage01 creation
    Given the API is running
    When I create a Usage01 with title Test
    Then the response should be OK Created