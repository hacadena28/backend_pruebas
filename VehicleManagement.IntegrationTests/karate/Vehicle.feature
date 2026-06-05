Feature: Vehicle API Integration Tests

  Background:
    * url baseUrl

  Scenario: Get all vehicles
    Given path '/api/vehicles'
    When method get
    Then status 200
    And match response == '#array'
