namespace ChatApp.Json.Settings
{
    class SettingsRepository : JsonHandling<Settings>
    {
        private static readonly SettingsRepository instance = new SettingsRepository();

        private SettingsRepository() : base("settings.json")
        {
        }

        public static Settings GetSettings()
        {
            return instance.JsonObject;
        }
    }
}
