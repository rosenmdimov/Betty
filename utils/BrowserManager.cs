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
        // The timeouts (_config.ElementTimeoutMs and _config.NavigationTimeoutMs) are not part of
        // BrowserNewContextOptions; DIHooks applies them to the context after it is created.
        var contextOptions = new BrowserNewContextOptions();

        if (!string.IsNullOrEmpty(_config.Device))
        {
            if (!playwright.Devices.TryGetValue(_config.Device, out var device))
            {
                throw new InvalidOperationException($"Unknown Playwright device '{_config.Device}'. Check the 'Device' value of the active profile.");
            }

            // Implementing mobile emulation
            contextOptions.ViewportSize = device.ViewportSize;
            contextOptions.ScreenSize = device.ScreenSize;
            contextOptions.DeviceScaleFactor = device.DeviceScaleFactor;
            contextOptions.UserAgent = device.UserAgent;
            contextOptions.IsMobile = device.IsMobile;
            contextOptions.HasTouch = device.HasTouch;
        }

        return contextOptions;
    }
}
