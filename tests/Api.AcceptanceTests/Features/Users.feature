Feature: Mangae user data in the system

Scenario: Get user details successfully
	Given the query is correct
	When Called GetUserDetails
	Then the user details should be returned