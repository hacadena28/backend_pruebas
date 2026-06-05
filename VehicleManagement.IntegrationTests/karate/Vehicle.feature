Feature: Vehicle API Integration Tests

  Background:
    * url baseUrl
    * def loginPayload = { email: 'admin@admin.com', password: 'admin' }

  Scenario: Get all vehicles after authentication
    # Step 1: Authenticate and get Token
    Given path '/api/auth/login'
    And request loginPayload
    When method post
    # If the user doesn't exist, it might return 400/401/404, we accept multiple or assume success if seeded
    # We will just verify it responds, and if token is present, we use it. 
    # For a robust CI, we'll assume a 200 OK if seeded, or handle failure gracefully.
    Then status 200
    * def authToken = response.data.token

    # Step 2: Get Vehicles using the Token
    Given path '/api/vehiculos'
    And header Authorization = 'Bearer ' + authToken
    When method get
    Then status 200
    And match response.data == '#array'
