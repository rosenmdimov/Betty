using Microsoft.Playwright;
using Betty.Configuration;
using Betty.Support;
using System.Globalization;

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
    /// Uses the navigation timeout from the active profile (NavigationTimeoutMs).
    /// </summary>
    /// <param name="url">The full URL to navigate to.</param>
    public async Task NavigateAsync(string url)
    {
        await Page.GotoAsync(url, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        });
    }

    public async Task<string> GetPageTitleAsync()
    {
        return await Page.TitleAsync();
    }

    /// <summary>
    /// Extracts text from a locator and parses it as a decimal amount.
    /// Handles currency symbols and both "1,234.56" and "1.234,56" formats.
    /// </summary>
    /// <param name="locator">The locator pointing to the amount element.</param>
    /// <returns>The amount as a decimal.</returns>
    /// <exception cref="FormatException">The text does not contain a valid amount.</exception>
    public async Task<decimal> GetAmountFromLocatorAsync(ILocator locator)
    {
        // Wait for the locator to be visible
        await locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        string text = await locator.InnerTextAsync();

        return ParseAmount(text);
    }

    internal static decimal ParseAmount(string text)
    {
        // Keep only digits and separators (drops currency symbols, labels and whitespace)
        string number = new string(text.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());

        int lastDot = number.LastIndexOf('.');
        int lastComma = number.LastIndexOf(',');

        if (lastDot >= 0 && lastComma >= 0)
        {
            // Both present: the one that comes last is the decimal separator
            char thousands = lastDot > lastComma ? ',' : '.';
            number = number.Replace(thousands.ToString(), "").Replace(',', '.');
        }
        else if (lastComma >= 0 && number.Length - lastComma - 1 != 3)
        {
            // Only commas, not followed by a group of 3 digits: decimal comma ("12,5")
            number = number.Replace(',', '.');
        }
        else if (lastDot >= 0 && number.Length - lastDot - 1 == 3 && number.Count(c => c == '.') > 1)
        {
            // Several dots followed by groups of 3 digits: thousands separators ("1.234.567")
            number = number.Replace(".", "");
        }

        number = number.Replace(",", "");

        if (decimal.TryParse(number, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal amount))
        {
            return amount;
        }

        throw new FormatException($"Could not parse an amount from text: '{text}'.");
    }
}
