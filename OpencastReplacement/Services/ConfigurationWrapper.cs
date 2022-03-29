namespace OpencastReplacement.Services
{
    public class ConfigurationWrapper
    {
        public ConfigurationManager ConfigurationManager { get; set; } = default!;

        public ConfigurationWrapper(ConfigurationManager config)
        {
            ConfigurationManager = config;
        }
    }
}
