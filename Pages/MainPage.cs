using Microsoft.Playwright;
using Betty.Configuration;
using Betty.Support;

namespace Betty.Pages;

/// <summary>
/// Handles interactions on the main entry page (https://www.spinberry.com/).
/// Responsibilities include accepting cookies and checking the page footer.
/// </summary>
public class MainPage : BasePage
{
    private const int CookieBannerTimeoutMs = 5000;

    private ILocator CookieButton => Page.Locator("#cookie-button");
    private ILocator SocialMediaLocator(string media) => Page.Locator($"img[alt*='{media}']").First;
    private readonly string _mainPageUrl;

    public MainPage(IPage page, AppConfig config, ScenarioState state) : base(page, config, state)
    {
        _mainPageUrl = config.BaseUrl ?? "https://www.spinberry.com/";
    }

    public Task NavigateToMainPageAsync() => base.NavigateAsync(_mainPageUrl);

    /// <summary>
    /// Accepts the cookie banner if it appears. A missing banner is not an error.
    /// </summary>
    public async Task AcceptCookiesAsync()
    {
        try
        {
            await CookieButton.ClickAsync(new LocatorClickOptions { Timeout = CookieBannerTimeoutMs });
            TestContext.Out.WriteLine("Cookie button clicked.");
        }
        catch (TimeoutException)
        {
            TestContext.Out.WriteLine("Cookie banner not shown, skipped.");
        }
    }

    /// <summary>
    /// Waits (up to the element timeout) for the social media icon to become visible.
    /// </summary>
    public async Task<bool> IsMediaIconVisibleAsync(string media)
    {
        try
        {
            await SocialMediaLocator(media).WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }
}
