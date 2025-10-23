using Betty.Configuration;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Betty.utils;

public class BrowserManager
{
    private readonly AppConfig _config;
    // DI: AppConfig is injected by DIHooks
    public BrowserManager(AppConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Launches IBrowser based on the configuration profile.
    /// </summary>
    public async Task<IBrowser> LaunchBrowserAsync(IPlaywright playwright)
    {
        string browserType = (_config.Browser ?? string.Empty).ToLowerInvariant();

        return browserType switch
        {
            "firefox" => await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = _config.Headless
            }),
            "webkit" => await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = _config.Headless
            }),
            _ => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = _config.Headless
            }),
        };
    }

    /// <summary>
    /// Returns context options, including device emulation if set.
    /// </summary>
    public BrowserNewContextOptions GetContextOptions(IPlaywright playwright)
    {
        // Initialize context options. The global timeouts
        // for page operations (_config.ElementTimeoutMs and _config.NavigationTimeoutMs)
        // will be implemented later in DIHooks.cs, as they are not part of BrowserNewContextOptions.
        var contextOptions = new BrowserNewContextOptions();

        if (!string.IsNullOrEmpty(_config.Device))
        {
            var device = playwright.Devices[_config.Device];

            if (device != null)
            {
                // Implementing mobile emulation
                contextOptions.ViewportSize = device.ViewportSize;
                contextOptions.UserAgent = device.UserAgent;
                contextOptions.IsMobile = device.IsMobile;
                contextOptions.HasTouch = device.HasTouch;
            }
        }

        // **IMPORTANT:** Playwright.NET requires that we set a DefaultTimeout for the page,
        // which is best done in DIHooks.

        return contextOptions;
    }
}
