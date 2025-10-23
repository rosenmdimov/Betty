namespace Betty.Support
{
    /// <summary>
    /// The class that serves as the World/Context and passes state between steps within a scenario.
    /// </summary>
    public class ScenarioState
    {
        public decimal InitialBalance { get; set; } = 0m;
        public decimal CurrentBalance { get; internal set; }
    }
}
