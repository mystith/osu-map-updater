﻿﻿﻿using System.IO;
using Serilog;
using YamlDotNet.Serialization;

namespace osumapupdate
{
    public class ConfigLoader
    {
        private Serializer serialize;
        private Deserializer deserialize;

        public ConfigLoader()
        {
            serialize = new Serializer();
            deserialize = new Deserializer();
        }
        public Config Load()
        {
            Config result;
            
            //Check if config.yaml exists
            if (!File.Exists("config.yaml"))
            {
                Log.Error("Config file not found, using default settings.");
                
                result = new Config();

                Save(result);
                
                return result;
            }

            string config = File.ReadAllText("config.yaml");

            result = deserialize.Deserialize<Config>(config);
            Log.Debug("Deserialized config file");
            
            return result;
        }
        
        public void Save(Config config)
        {
            string cfgtext = serialize.Serialize(config);
            File.WriteAllText("config.yaml", cfgtext);
            
            Log.Debug("Saved config file");
        }
    }
}