using Reqnroll;
using Betty.Pages;
using Betty.Support;

namespace Betty.Steps;

[Binding]
public class GameplaySteps
{
    private readonly IrishWildsPage _irishWildsPage;
    private readonly ScenarioState _state;

    // DI: Reqnroll injects all dependencies
    public GameplaySteps(IrishWildsPage irishWildsPage, ScenarioState state)
    {
        _irishWildsPage = irishWildsPage;
        _state = state;
    }

    [Given("I open the {string} game in a new tab")]
    [When("I open the {string} game in a new tab")]
    public async Task WhenIOpenTheGameInNewTab(string neededGame)
    {
        await _irishWildsPage.OpenGameTileAndSwitchTabAsync(neededGame);
    }

    [Given("I start the game from the loading screen")]
    public async Task GivenIStartTheGameFromTheLoadingScreen()
    {
        await _irishWildsPage.StartTheGameAsync();
    }

    [Then("The demo indicator is available on the page")]
    public async Task ThenTheDemoIndicatorIsAvailableOnThePage()
    {
        string demoIndicator = await _irishWildsPage.GetDemoIndicatorTextAsync();
        Assert.That(demoIndicator, Is.Not.Empty);
    }

    [Then("The initial amount is {int}")]
    public async Task ThenTheInitialAmountIs(int wantedAmount)
    {
        _state.InitialBalance = await _irishWildsPage.GetCurrentBalanceAsync();
        Assert.That(_state.InitialBalance, Is.EqualTo(wantedAmount));
    }

    [Given("I get the balance")]
    public async Task GivenIGetTheInitialBalance()
    {
        _state.InitialBalance = await _irishWildsPage.GetCurrentBalanceAsync();
        TestContext.Out.WriteLine($"Initial balance: {_state.InitialBalance}");
    }

    [When("I make {int} spins")]
    public async Task WhenIMakeSpins(int spinsCount)
    {
        await _irishWildsPage.MakeSpinsAsync(spinsCount);
    }

    [Then("The balance should be different from the initial one")]
    public async Task ThenTheBalanceShouldBeDifferentFromTheInitialOne()
    {
        _state.CurrentBalance = await _irishWildsPage.GetCurrentBalanceAsync();
        TestContext.Out.WriteLine($"Initial balance: {_state.InitialBalance}, current balance: {_state.CurrentBalance}");

        // A net win over several spins is possible, so only an unchanged balance is a failure
        Assert.That(_state.CurrentBalance, Is.Not.EqualTo(_state.InitialBalance),
            "The balance did not change after the spins.");
    }
}
