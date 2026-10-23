@allure.epic:Games
Feature: Irish Wilds Gameplay Validation
  As a casino operations tester
  I want to automate the Irish Wilds slot game launch and spins
  So that I can validate the core win/loss balance update functionality.

  Background: Go to Irish Wilds Game
    Given I am on the Main page
    And I accept the cookie policy
    And I open the "Irish Wilds" game in a new tab

  @desktop @mobile @critical @allure.story:GameLaunch
  Scenario: Can open Irish Wilds game
    Then The demo indicator is available on the page

  @desktop @mobile @normal @allure.story:Balance
  Scenario: Can verify that the initial amount is $2000
    Given I start the game from the loading screen
    Then The initial amount is 2000

  @desktop @mobile @critical @allure.story:Balance
  Scenario: Verify that the balance changes after 10 spins
    Given I start the game from the loading screen
    And I get the balance
    When I make 10 spins
    Then The balance should be different from the initial one
