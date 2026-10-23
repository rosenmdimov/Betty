@allure.epic:Website
Feature: Testing www.spinberry.com

	@desktop @mobile @minor @allure.story:SocialMedia
	Scenario Outline: Verify the social media icons are displayed
		Given I am on the Main page
		When I accept the cookie policy
		Then The social media "<media>" is displayed

		Examples:
			| media     |
			| Facebook  |
			| Instagram |
			| LinkedIn  |
