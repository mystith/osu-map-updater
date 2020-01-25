﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace osumapupdate
{
    public class Covers
    {
        public string cover { get; set; }
        [JsonProperty("cover@2x")]public string cover2x { get; set; }
        public string card { get; set; }
        [JsonProperty("card@2x")]public string card2x { get; set; }
        public string list { get; set; }
        [JsonProperty("list@2x")]public string list2x { get; set; }
        public string slimcover { get; set; }
        [JsonProperty("slimcover@2x")]public string slimcover2x { get; set; }
    }

    public class Availability
    {
        public bool download_disabled { get; set; }
        public object more_information { get; set; }
    }

    public class Hype
    {
        public int current { get; set; }
        public int required { get; set; }
    }

    public class Nominations
    {
        public int current { get; set; }
        public int required { get; set; }
    }

    public class Beatmap
    {
        public int id { get; set; }
        public int beatmapset_id { get; set; }
        public string mode { get; set; }
        public int mode_int { get; set; }
        public object convert { get; set; }
        public double difficulty_rating { get; set; }
        public string version { get; set; }
        public int total_length { get; set; }
        public int hit_length { get; set; }
        public double bpm { get; set; }
        public double cs { get; set; }
        public double drain { get; set; }
        public double accuracy { get; set; }
        public double ar { get; set; }
        public int playcount { get; set; }
        public int passcount { get; set; }
        public int count_circles { get; set; }
        public int count_sliders { get; set; }
        public int count_spinners { get; set; }
        public int count_total { get; set; }
        public DateTime last_updated { get; set; }
        public int ranked { get; set; }
        public string status { get; set; }
        public string url { get; set; }
        public object deleted_at { get; set; }
    }

    public class BeatmapSet
    {
        public int id { get; set; }
        public string title { get; set; }
        public string artist { get; set; }
        public int play_count { get; set; }
        public int favourite_count { get; set; }
        public bool has_favourited { get; set; }
        public DateTime submitted_date { get; set; }
        public DateTime last_updated { get; set; }
        public DateTime ranked_date { get; set; }
        public string creator { get; set; }
        public int user_id { get; set; }
        public double bpm { get; set; }
        public string source { get; set; }
        public Covers covers { get; set; }
        public string preview_url { get; set; }
        public string tags { get; set; }
        public bool video { get; set; }
        public bool storyboard { get; set; }
        public int ranked { get; set; }
        public string status { get; set; }
        public bool is_scoreable { get; set; }
        public bool discussion_enabled { get; set; }
        public bool discussion_locked { get; set; }
        public bool can_be_hyped { get; set; }
        public Availability availability { get; set; }
        public Hype hype { get; set; }
        public Nominations nominations { get; set; }
        public string legacy_thread_url { get; set; }
        public List<Beatmap> beatmaps { get; set; }
    }
    
    public class Cursor
    {
        public string approved_date { get; set; }
        public string _id { get; set; }
    }
    
    public class RootObject
    {
        public List<BeatmapSet> beatmapsets { get; set; }
        public Cursor cursor { get; set; }
        public object recommended_difficulty { get; set; }
        public object error { get; set; }
        public int total { get; set; }
    }
}