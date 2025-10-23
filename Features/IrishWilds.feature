Feature: Irish Wilds Gameplay Validation
  As a casino operations tester
  I want to automate the Irish Wilds slot game launch and spins
  So that I can validate the core win/loss balance update functionality.

  Background: Go to Irish Wilds Game
    Given I am on the Main page
    When I accept the cookie policy
    And I open the "Irish Wilds" game in a new tab


    @desktop @mobile
    Scenario: Can open Irish Wilds game
        Then The demo indicator is available on the page

        @desktop @mobile
    Scenario: Can verify that the innitial amount is $2000
        Given I start the game from the loading screen
        Then The innitial amount is 2000

    @desktop @mobile
    Scenario: Verify that the amount has decreased after 10 spins
       Given I start the game from the loading screen
       And I Get the balance
       When I make 10 spins
       Then The ballance should be less than initial

    
