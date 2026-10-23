using Microsoft.Playwright;
using Betty.Configuration;
using Betty.Support;

namespace Betty.Pages;

public class IrishWildsPage : BasePage
{
    // The new TAB/PAGE where the game loads
    private IPage? _gamePage;

    // Upper limit for carousel clicks on mobile, so a missing game can't loop forever
    private const int MaxCarouselClicks = 30;

    // Delay time after a spin (simulates animation time)
    private const int SpinDelayMs = 3000;

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

    // --- MAIN PAGE LOCATORS (game carousel) ---
    private ILocator GamePlayLink(string gameName) => Page.Locator($"//span[@class=\"game-card__header\" and text()=\"Silent Fright\"]/following-sibling::div/a[contains(@class,\"btn-primary\")]");
    private ILocator PreviousGameButton => Page.Locator("#load-more-btn");
    private ILocator GameTitle => Page.Locator("//span[@class=\"game-card__header\"]");

    // --- GAME LOCATORS (elements inside the new tab) ---
    private ILocator GameStartPlayButton => GetGameLocator("button.button__slider-play");
    private ILocator BalanceDisplay => GetGameLocator(".display.balance span[class=\"amount\"]").First;
    private ILocator SpinButton => GetGameLocator(".main__controls button.arrows-spin-button");
    private ILocator DemoIndicator => GetGameLocator("div[class=\"demo-indicator\"] p");

    // DI: Injects its own dependencies
    public IrishWildsPage(IPage page, AppConfig config, ScenarioState state) : base(page, config, state)
    {
    }

    /// <summary>
    /// Retrieves the player's current balance from the game tab.
    /// </summary>
    public Task<decimal> GetCurrentBalanceAsync() => GetAmountFromLocatorAsync(BalanceDisplay);

    /// <summary>
    /// Performs a single spin and waits for the animation/loading to complete.
    /// </summary>
    public async Task PerformSpinAsync()
    {
        await SpinButton.ClickAsync();

        // Wait for animation/loading
        await Task.Delay(SpinDelayMs);
    }

    public async Task MakeSpinsAsync(int spinsCount)
    {
        for (int i = 0; i < spinsCount; i++)
        {
            await PerformSpinAsync();
            TestContext.Out.WriteLine($"Completed spin {i + 1} of {spinsCount}");
        }
    }

    public async Task OpenGameTileAndSwitchTabAsync(string neededGame)
    {
        // On mobile only one game is shown at a time, so scroll the carousel to it
        if (!string.IsNullOrEmpty(Config.Device))
        {
            await ScrollCarouselToGameAsync(neededGame);
        }

        var playLink = GamePlayLink(neededGame);
        await playLink.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        TestContext.Out.WriteLine($"Clicking '{neededGame}' link and waiting for new tab to open.");

        _gamePage = await Page.Context.RunAndWaitForPageAsync(() => playLink.ClickAsync());
        await _gamePage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task StartTheGameAsync()
    {
        await GameStartPlayButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        TestContext.Out.WriteLine("The start button is visible.");
        await GameStartPlayButton.ClickAsync();
        TestContext.Out.WriteLine($"Clicked the start game button: {_gamePage?.Url}");
    }

    public async Task<string> GetDemoIndicatorTextAsync()
    {
        return await DemoIndicator.TextContentAsync() ?? string.Empty;
    }

    private async Task ScrollCarouselToGameAsync(string neededGame)
    {
        for (int clicks = 0; clicks <= MaxCarouselClicks; clicks++)
        {
            var currentGame = (await GameTitle.TextContentAsync())?.Trim();
            if (string.Equals(neededGame, currentGame, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            await PreviousGameButton.ClickAsync();
        }

        throw new InvalidOperationException($"Game '{neededGame}' was not found in the carousel after {MaxCarouselClicks} clicks.");
    }

    // Builds an XPath string literal that is safe for names containing quotes
    private static string ToXPathLiteral(string value)
    {
        if (!value.Contains('\''))
        {
            return $"'{value}'";
        }
        if (!value.Contains('"'))
        {
            return $"\"{value}\"";
        }
        return $"concat('{value.Replace("'", "', \"'\", '")}')";
    }
}
