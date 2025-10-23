using Microsoft.Playwright;
using Betty.Configuration;
using Betty.Support;

namespace Betty.Pages;
/// <summary>
/// Handles interactions on the main entry page (https://www.spinberry.com/).
/// Responsibilities include accepting cookies and navigating to the products section.
/// </summary>
public class MainPage : BasePage
{
    private ILocator CookieButton => Page.Locator("#cookie-button");
    private readonly string _mainPageUrl;
    public MainPage(IPage page, AppConfig config, ScenarioState state) : base(page, config, state)
    {
        _mainPageUrl = config.BaseUrl ?? "https://www.spinberry.com/";
    }
    public Task NavigateToMainPageAsync() => base.NavigateAsync(_mainPageUrl);
    private ILocator SocialMediaLocator(string media) => Page.Locator($"a[class*='{media}']");

    public async Task AcceptCookiesAsync()
    {
        await CookieButton.ClickAsync(new LocatorClickOptions { Timeout = 5000 });
        TestContext.Out.WriteLine("Cookie button clicked or skipped.");
    }

    public async Task<bool> IsMediaIconVisible(string media)
    {
        return await SocialMediaLocator(media).IsVisibleAsync();
    }
}