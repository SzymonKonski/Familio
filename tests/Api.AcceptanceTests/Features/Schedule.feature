Feature: Schedule

Scenario: Gets list of events in group
	Given I am a user
	And I am in some group
	When I make a GET request to GetEvents endpoint
	Then the response status code is '200'
	And the response should be list of Events in the group
