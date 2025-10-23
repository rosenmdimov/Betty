using Reqnroll;
using Betty.Pages;
using Betty.Support;

namespace Betty.Steps;

[Binding]
public class GameplaySteps
{
    private readonly MainPage _mainPage;
    private readonly IrishWildsPage _irishWildsPage;
    private readonly ScenarioState _state;

    // DI: Reqnroll injects all dependencies
    public GameplaySteps(MainPage mainPage, IrishWildsPage irishWildsPage, ScenarioState state)
    {
        _irishWildsPage = irishWildsPage;
        _mainPage = mainPage;
        _state = state;
    }

    [When("I open the {string} game in a new tab")]
    public async Task WhenIOpenIrishWildsGameInNewTab(string neededGame)
    {
        await _irishWildsPage.OpenGameTileAndSwitchTabAsync(neededGame);
    }

    [Given("I start the game from the loading screen")]
    public async Task GivenIStartTheGameFromTheLoadingScreen()
      {
        await _irishWildsPage.StartTheGameAsync();

        TestContext.Out.WriteLine($"Launch the Game: {_irishWildsPage.Page.Url}");
    }

    [Then("The demo indicator is available on the page")]
    public async Task ThenTheDemoIndicatorIsAvailableOnThePage()
    {
        string demoIndicator = await _irishWildsPage.GetDemoIndicatorTextAsync();
        Assert.That(demoIndicator, Is.Not.Empty);
    }

    [Then("The innitial amount is {int}")]
    public async Task ThenTheInnitialAmountIsAsync(int wantedAmount)
    {
          await _irishWildsPage.GetBalance(); 
        Assert.That(_state.InitialBalance, Is.EqualTo(wantedAmount));
    }

    [Given("I Get the balance")]
    public async Task GivenIGetTheInitialBalance()
    {
        await _irishWildsPage.GetBalance();
    }

    [When("I make {int} spins")]
    public async Task WhenIMakeSpins(int spinsCount)
    {
        await _irishWildsPage.MakeSpins(spinsCount);
    }
    [Then("The ballance should be less than initial")]
    public async Task ThenTheBallanceShouldBeLessThanInitial()
    {
        var initialState = _state.InitialBalance;
        await _irishWildsPage.GetBalance();
        var currentBalance = _state.InitialBalance;

        Assert.That( currentBalance, Is.AtMost(initialState));
    }
}

