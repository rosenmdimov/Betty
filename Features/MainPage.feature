Feature: Testing www.spinbery.com

	Scenario Outline: Verify the social media icons are displayed
		Given I am on the Main page
		When I accept the cookie policy
		Then The social media "<media>" is displayed

		Examples:
			| media     |
			| facebook  |
			| instagram |
			| linkedin  |
					