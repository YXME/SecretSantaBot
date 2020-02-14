using Newtonsoft.Json;
using System.IO;

namespace SecretSanta
{
    class Config
    {
        private const string configFolder = "Config";
        private const string configFile = "config.json";

        // Get all the necessary data to proceed connexion.

        public static BotConfig bot;
        static Config()
        {
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);

            if (!File.Exists(configFolder + "/" + configFile))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        public struct BotConfig
        {
            public string Token;
        }
    }
}
