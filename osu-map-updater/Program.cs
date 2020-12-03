﻿﻿using System;
 using System.Collections.Generic;
 using System.Diagnostics;
 using System.IO;
 using System.Linq;
 using System.Net.Http;
 using System.Threading;
 using System.Threading.Tasks;
using CircleHelper.Data;
using CircleHelper.Parsing;
 using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using PuppeteerSharp;
  using PuppeteerSharp.Input;
  using Serilog;
using Serilog.Core;

 namespace osumapupdate
{
    class Program
    {
        static void Main(string[] args)
        { 
            Task.WaitAll(MainAsync());
        }

        static async Task MainAsync()
        {
            LoggingLevelSwitch lls = new LoggingLevelSwitch();
            
            //Initialize logger with logginglevelswitch to enable runtime configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(lls)
                .WriteTo.Console()
                .WriteTo.File("logs/logfile.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            
            ConfigLoader cl = new ConfigLoader();
            Config c = cl.Load();
            
            lls.MinimumLevel = c.LogDepth;
            
            //Download Chromium for login and download
            Log.Information("Please wait, downloading chromium");
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            Log.Information("Finished downloading chromium");
            
            if (string.IsNullOrWhiteSpace(c.DatabasePath))
            {
                Log.Error("osu! database path null, please set the path in the config.yaml file!");
                return;
            }

            //Load .db file
            Database dm = DatabaseParser.Parse(c.DatabasePath);

            if (dm == null)
            {
                Log.Error("Database data null.");
                return;
            }

            Browser mainBrowser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
                Timeout = 0,
                UserDataDir = "./user_data"
            });
            
            Page p = (await mainBrowser.PagesAsync()).First();
            await p.GoToAsync("https://osu.ppy.sh/home");

            //Force login
            bool loggedIn = (await p.GetContentAsync()).Contains("/users/");

            if (!loggedIn)
                Log.Information("Please log in to the osu website.");

            while (!loggedIn)
            {
                Thread.Sleep(500);
                loggedIn = (await p.GetContentAsync()).Contains("/users/");
            }

            //Begin searching maps
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://osu.ppy.sh/beatmapsets/search");
            response.EnsureSuccessStatusCode();
                
            string responseBody = await response.Content.ReadAsStringAsync();

            IEnumerable<JToken> jt = JsonConvert.DeserializeObject<JToken>(responseBody)["beatmapsets"].Where(a => //reduce downloads
                a["beatmaps"].Any(b => //make sure at least one of the beatmaps meet criteria
                    b["difficulty_rating"].Value<double>() >= c.MinimumDifficulty //difficulty criteria
                    && c.Modes.Any(b["mode"].Value<string>().Equals) //mode criteria
                    && dm.Beatmaps.All(d => d.BeatmapID != b["id"].Value<int>()) //make sure map doesn't already exist
                )
            );

            int i = 1;
            //Download maps
            foreach (JToken bs in jt)
            {
                Log.Information("Downloading [ {0} ] by {1}  [ {2} / {3} ]", bs["title"].Value<string>(), bs["creator"].Value<string>(), i, jt.Count());
                Page page = await mainBrowser.NewPageAsync();

                try
                {
                    await page.GoToAsync($"https://osu.ppy.sh/beatmapsets/{bs["id"].Value<string>()}/");
                    await page.ClickAsync(".js-beatmapset-download-link");
                }
                catch (Exception e)
                {
                    if(!e.Message.Contains("ERR_ABORTED")) Log.Error(e.ToString());
                }

                //Handle too many requests error
                while ((await page.GetContentAsync()).Contains("429 Too Many Requests"))
                {
                    Log.Information("429 Too Many Requests error, waiting one minute...");
                    Thread.Sleep(60000);
                    await page.ReloadAsync();
                }
                
                await page.CloseAsync();

                i++;
                
                Thread.Sleep(3000);
            }

            //Get downloads folder
            DirectoryInfo downloads = new DirectoryInfo($@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\Downloads");

            //Wait for files to download
            while (downloads.GetFiles().Any(o => o.Extension == ".crdownload")) ;

            //Close browser
            await mainBrowser.CloseAsync();
        }
    }
}