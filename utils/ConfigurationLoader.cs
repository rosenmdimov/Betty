using Betty.Configuration;
using Microsoft.Extensions.Configuration;

namespace Betty.utils
{
    /// <summary>
    /// Responsible for loading, prioritizing, and binding the configuration profile.
    /// </summary>
    public static class ConfigurationLoader
    {
        // The profile is read once per test run and shared by all scenarios
        private static readonly Lazy<AppConfig> Config = new(Load);

        public static AppConfig LoadConfig() => Config.Value;

        private static AppConfig Load()
        {
            // Initialize the configuration
            var configuration = new ConfigurationBuilder()
                // We use BaseDirectory to find appsettings.json
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables() // Allows TEST_PROFILE to be set from the environment
                .Build();

            // Determining the active profile (Prioritization)
            // Reading TEST_PROFILE from Environment Variables
            string? activeProfileName = configuration["TEST_PROFILE"]?.Trim()
                // Reading DefaultProfile from appsettings.json
                ?? configuration["DefaultProfile"]?.Trim();

            if (string.IsNullOrEmpty(activeProfileName))
            {
                throw new InvalidOperationException("No test profile set. Please set 'TEST_PROFILE' (Env Var) or 'DefaultProfile' in appsettings.json.");
            }

            Console.WriteLine($"Selected profile for testing:{activeProfileName}");

            // Retrieve the configuration section (e.g. "TestProfiles:mobileWebkit")
            var profileSection = configuration.GetRequiredSection($"TestProfiles:{activeProfileName}");

            // Binding the section to AppConfig
            var appConfig = profileSection.Get<AppConfig>()
                                ?? throw new InvalidOperationException($"The profile '{activeProfileName}' cannot be loaded. Check if it exists in 'TestProfiles'.");
            appConfig.ProfileName = activeProfileName;

            return appConfig;
        }
    }
}
