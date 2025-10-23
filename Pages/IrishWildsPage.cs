using Microsoft.Playwright;
using Betty.Configuration;
using Betty.Support;

namespace Betty.Pages;

public class IrishWildsPage : BasePage
{
    // Reference to the main page object to simplify navigation access.
    private readonly MainPage _mainPage;

    // The new TAB/PAGE where the game loads
    private IPage? _gamePage;

    // Private Helper for locators. Ensures interaction happens inside the new tab (_gamePage).
    private ILocator GetGameLocator(string selector)
    {
        // Ensures _gamePage is initialized before use
        if (_gamePage == null)
        {
            throw new InvalidOperationException("Game page context is not initialized. Call OpenGameTileAndSwitchTabAsync() first.");
        }
        return _gamePage.Locator(selector);
    }

    // --- GAME LOCATORS (elements inside the new tab) ---
    private ILocator GameStartPlayButton => GetGameLocator("button.button__slider-play");
    public ILocator IrishWildsPlayLink => Page.Locator("//h4[text()='Irish Wilds']/parent::div/following-sibling::a").First;
    // Gameplay Locators
    public ILocator BalanceDisplay => GetGameLocator(".display.balance span[class=\"amount\"]").First;
    public ILocator SpinButton => GetGameLocator(".main__controls button.arrows-spin-button");
    public ILocator BetDisplay => GetGameLocator(".bet-label, .bet-amount").First;
    public ILocator WinDisplay => GetGameLocator(".win-label, .total-win-amount").First;
    public ILocator StakePlusButton => GetGameLocator(".main__controls button disabled").Nth(3);
    public ILocator PreviousGameButton => Page.Locator("//div[@class=\"col-4 center current\"]//button[@class=\"prev\"]");
    public ILocator GameTitle => Page.Locator("//div[@class=\"col-4 center current\"]//h4");
    public ILocator DemoIndicator => GetGameLocator("div[class=\"demo-indicator\"] p");

    // Delay time after a spin (simulates animation time)
    private const int SpinDelayMs = 3000;

    // DI: Injects MainPage and its own dependencies
    public IrishWildsPage(IPage page, AppConfig config, MainPage mainPage, ScenarioState state) : base(page, config, state)
    {
        _mainPage = mainPage;
        _gamePage = null;
    }

    /// <summary>
    /// Retrieves the player's current balance from the game tab.
    /// </summary>
    public async Task<decimal> GetCurrentBalanceAsync()
    {
        // Uses GetBalanceFromLocatorAsync from BasePage
        return await base.GetBalanceFromLocatorAsync(BalanceDisplay);
    }

    /// <summary>
    /// Retrieves the current bet amount from the game tab.
    /// </summary>
    public async Task<decimal> GetCurrentBetAsync()
    {
        return await base.GetBalanceFromLocatorAsync(BetDisplay);
    }

    /// <summary>
    /// Performs a single spin and waits for the animation/loading to complete.
    /// </summary>
    public async Task PerformSpinAsync()
    {
        await SpinButton.ClickAsync();

        // Wait for animation/loading
        await Task.Delay(SpinDelayMs);
    }

    /// <summary>
    /// Executes N spins and performs an initial validation of the balance update.
    /// </summary>
    public async Task SpinAndValidateBalanceUpdateAsync(int count)
    {
        decimal initialBalance = await GetCurrentBalanceAsync();
        decimal betAmount = await GetCurrentBetAsync();

        if (betAmount <= 0)
        {
            TestContext.Out.WriteLine("WARNING: Bet amount is 0. Please check the locator for BetDisplay.");
            return;
        }

        TestContext.Out.WriteLine($"Initial Balance: {initialBalance}, Bet: {betAmount}");

        for (int i = 0; i < count; i++)
        {
            await PerformSpinAsync();
            TestContext.Out.WriteLine($"Spin {i + 1} completed.");
        }

        // Minimal check inside the page object
        decimal finalBalance = await GetCurrentBalanceAsync();
        decimal actualLossOrWin = finalBalance - initialBalance;

        // Assertion: checks that the balance change is not absurdly large, suggesting a major bug.
        Assert.That(Math.Abs(actualLossOrWin), Is.LessThanOrEqualTo(initialBalance * 10), "Balance change seems illogical (potential runaway win or loss).");

        TestContext.Out.WriteLine($"Final Balance: {finalBalance}. Total Change: {actualLossOrWin}");
    }
    public async Task OpenGameTileAndSwitchTabAsync(string neededGame)
    {
        bool needsForceClick = !string.IsNullOrEmpty(Config.Device);

        if (needsForceClick)
        {
            var currentGame = await GameTitle.TextContentAsync();
            while (!neededGame.Equals(currentGame))
            {
                await PreviousGameButton.ClickAsync();

                currentGame = await GameTitle.TextContentAsync();
            }
        }
        await IrishWildsPlayLink
            .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        TestContext.Out.WriteLine("Clicking Irish Wilds link and waiting for new tab to open.");

        var newPageTask = Page.Context.WaitForPageAsync();
        await IrishWildsPlayLink.ClickAsync();

        if (newPageTask == null)
        {
            throw new Exception("Failed to open the game in a new tab after clicking the 'Play game' link.");
        }
        _gamePage = await newPageTask;
        await _gamePage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
    public async Task StartTheGameAsync()
    {
        await GameStartPlayButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        TestContext.Out.WriteLine("The start button is visible.");
        await GameStartPlayButton.ClickAsync();
        TestContext.Out.WriteLine("Clicked the strt game button.");
    }

    internal IPage? Get_gamePage()
    {
        return _gamePage;
    }

    internal async Task<string> GetGamePageTitleAsync(IPage? _gamePage)
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        return await _gamePage.TitleAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    internal async Task GetBalance()
    {
        State.InitialBalance = Convert.ToDecimal(await base.GetBalanceFromLocatorAsync(BalanceDisplay));
    }

    internal async Task MakeSpins(int spinsCount)
    {
        for (int i = 0; i < spinsCount; i++)
        {
            await PerformSpinAsync();
            TestContext.Out.WriteLine($"Completed spin {i + 1} of {spinsCount}");
        }
    }
    internal async Task<string> GetDemoIndicatorTextAsync()
    {
#pragma warning disable CS8603 // Possible null reference return.
        return await DemoIndicator.TextContentAsync();
#pragma warning restore CS8603 // Possible null reference return.
    }
}