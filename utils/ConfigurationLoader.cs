using Betty.Configuration;
using Microsoft.Extensions.Configuration;

namespace Betty.utils
{
    /// <summary>
    /// Responsible for loading, prioritizing, and binding the configuration profile.
    /// </summary>
    public static class ConfigurationLoader
    {
        public static AppConfig LoadConfig()
        {
            // Initialize the configuration
            var configuration = new ConfigurationBuilder()
                // We use BaseDirectory to find appsettings.json
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables() // Allows TEST_PROFILE to be set from the environment
                .Build();

            // Determining the active profile (Prioritization)
            // Reading TEST_PROFILE from Environment Variables
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string activeProfileName = configuration["TEST_PROFILE"]?.Trim()
                // Reading DefaultProfile from appsettings.json
                ?? configuration["DefaultProfile"]?.Trim();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

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

            return appConfig;
        }
    }
}
