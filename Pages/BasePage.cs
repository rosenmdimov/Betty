using Microsoft.Playwright;
using Betty.Configuration;
using Betty.Support;

namespace Betty.Pages;

/// <summary>
/// Base class for all Page Object Models (POMs) in the project.
/// Handles common operations like navigation and basic data extraction.
/// </summary>
public class BasePage
{
    // The main Playwright page instance
    public IPage Page { get; private set; }


    // Application configuration settings
    protected readonly AppConfig Config;


    // Exchange states between the steps
    protected readonly ScenarioState State;


    // DI: The constructor receives the IPage and AppConfig instances from the DI container.
    public BasePage(IPage page, AppConfig config, ScenarioState state)
    {
        Page = page;
        Config = config;
        State = state;
    }

    /// <summary>
    /// Navigates the browser to the specified URL.
    /// </summary>
    /// <param name="url">The full URL to navigate to.</param>
    public async Task NavigateAsync(string url)
    {
        await Page.GotoAsync(url, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 30000
        });
    }
    public async Task<string> GetPageTitleAsync()
    {
        return await Page.TitleAsync();
    }

    /// <summary>
    /// Extracts text from a locator and attempts to parse it as a decimal balance.
    /// Handles potential currency symbols and formatting.
    /// </summary>
    /// <param name="locator">The locator pointing to the balance element.</param>
    /// <returns>The balance as a decimal.</returns>
    public async Task<decimal> GetBalanceFromLocatorAsync(ILocator locator)
    {
        // Wait for the locator to be visible
        await locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        string text = await locator.InnerTextAsync();

        // Clean the text: remove currency symbols, commas, and trim whitespace
        text = text.Replace("$", "")
         .Replace("€", "")
         .Replace(",", "")
         .Replace("Balance", "") // Specific to some game UIs
                   .Trim();

        if (decimal.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal balance))
        {
            State.InitialBalance = balance;
            return balance;
        }

        TestContext.Out.WriteLine($"Warning: Could not parse balance from text: '{text}'. Returning 0.");
        return 0m;
    }
}