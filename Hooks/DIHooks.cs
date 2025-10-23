using Microsoft.Playwright;
using Reqnroll.BoDi;
using Reqnroll;
using Betty.Configuration;
using Betty.utils;
using System.Diagnostics;

namespace Betty.Hooks;

[Binding]
public class DIHooks(IObjectContainer objectContainer)
{
    private readonly IObjectContainer _objectContainer = objectContainer;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private AppConfig? _config;

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports", "TestResults.html");
        
        if (File.Exists(reportPath))
        {
            try
            {
                File.Delete(reportPath);
                TestContext.Out.WriteLine("✅ Reqnroll report Cleanup successfully invoked.");
            }
            catch (Exception ex)
            {
                TestContext.Error.WriteLine($"⚠️ Reqnroll Cleanup error: {ex.Message}");
            }
        }
    }

    [BeforeScenario(Order = 1)]
    public async Task BeforeScenario()
    {
        // Load configuration
        _config = ConfigurationLoader.LoadConfig();
        _objectContainer.RegisterInstanceAs(_config);

        _playwright = await Playwright.CreateAsync();
        var browserManager = new BrowserManager(_config);
        _browser = await browserManager.LaunchBrowserAsync(_playwright);

        var contextOptions = browserManager.GetContextOptions(_playwright);
        var context = await _browser.NewContextAsync(contextOptions);

        context.SetDefaultTimeout(_config.ElementTimeoutMs);
        var page = await context.NewPageAsync();
        page.SetDefaultNavigationTimeout(_config.NavigationTimeoutMs);

        _objectContainer.RegisterInstanceAs(page);
        _objectContainer.RegisterInstanceAs(context);
        _objectContainer.RegisterInstanceAs(_browser);
        _objectContainer.RegisterInstanceAs(_playwright);
    }

    [AfterStep]
    public async Task AfterStepAsync(ScenarioContext scenarioContext)
    {
        if (scenarioContext.TestError != null)
        {
            try
            {
                var page = _objectContainer.Resolve<IPage>();
                byte[] screenshotBytes = await page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true });
                string screenshotsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports", "screenshots");
                Directory.CreateDirectory(screenshotsDir);

                string screenshotPath = Path.Combine(screenshotsDir, $"fail_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                await File.WriteAllBytesAsync(screenshotPath, screenshotBytes);

                TestContext.Out.WriteLine($"⚠️ Screenshot saved to {screenshotPath}");
            }
            catch (Exception ex)
            {
                TestContext.Error.WriteLine($"⚠️ Error capturing screenshot: {ex.Message}");
            }
        }
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        try
        {
            if (_objectContainer.IsRegistered<IBrowserContext>())
            {
                var context = _objectContainer.Resolve<IBrowserContext>();
                await context.CloseAsync();
            }
            if (_objectContainer.IsRegistered<IBrowser>())
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                await _browser.CloseAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _playwright.Dispose();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            TestContext.Out.WriteLine("✅ Browser resources released.");
        }
        catch (Exception ex)
        {
            TestContext.Error.WriteLine($"⚠️ Error closing browser: {ex.Message}");
        }
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        try
        {
            string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports", "TestResults.html");
            if (File.Exists(reportPath))
            {
                TestContext.Out.WriteLine($"✅ Opening report: {reportPath}");
                Process.Start(new ProcessStartInfo
                {
                    FileName = reportPath,
                    UseShellExecute = true // Opens in default browser
                });
            }
            else
            {
                TestContext.Error.WriteLine("⚠️ Report file not found.");
            }
        }
        catch (Exception ex)
        {
            TestContext.Error.WriteLine($"⚠️ Could not open report automatically: {ex.Message}");
        }
    }
}
