using System;

namespace Betty.Configuration
{
    /// <summary>
    /// Configuration profile model (Desktop or Mobile).
    /// Binds to the TestProfiles:{{activeProfile}} section in appsettings.json.
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Name of the active profile (e.g. "desktopChrome"). Set by ConfigurationLoader.
        /// </summary>
        public string ProfileName { get; set; } = string.Empty;

        // Browser settings
        public string BaseUrl { get; set; } = "https://www.spinberry.com";
        public string Browser { get; set; } = "chromium";
        public bool Headless { get; set; } = false;
        public string Device { get; set; } = string.Empty; // For mobile emulation

        /// <summary>
        /// Maximum time for Playwright to wait when searching for an element in milliseconds.
        /// </summary>
        public int ElementTimeoutMs { get; set; } = 15000; // 15sec

        /// <summary>
        /// Maximum timeout for Playwright when navigating in milliseconds.
        /// </summary>
        public int NavigationTimeoutMs { get; set; } = 30000; // 30 sec
    }
}
