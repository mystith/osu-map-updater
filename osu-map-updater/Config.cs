﻿using Serilog.Events;

namespace osumapupdate
{
    public class Config
    {
        public string DatabasePath { get; private set; }
        public double MinimumDifficulty { get; private set; }
        public string[] Modes { get; private set; }
        public LogEventLevel LogDepth { get; private set; }

        public Config()
        {
            DatabasePath = "";
            MinimumDifficulty = 4.99;
            Modes = new string[] { "osu" };
            LogDepth = LogEventLevel.Information;
        }
    }
}