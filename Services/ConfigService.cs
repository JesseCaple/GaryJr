﻿namespace GaryJr.Services
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Read-only config file that supports model additions.
    /// </summary>
    public class ConfigService
    {
        private readonly string path;

        public ConfigService()
        {
            if (!Directory.Exists(AppContext.BaseDirectory))
            {
                Directory.CreateDirectory(AppContext.BaseDirectory);
            }

            this.path = Path.Combine(AppContext.BaseDirectory, "config.txt");
            if (File.Exists(this.path))
            {
                var str = File.ReadAllText(this.path);
                if (!str.EndsWith("\r\n"))
                {
                    File.AppendAllText(this.path, "\r\n");
                    str = str + "\r\n";
                }

                foreach (var prop in this.GetType().GetProperties())
                {
                    var regex = new Regex($"{prop.Name}\\s*=(.*?)\r");
                    var match = regex.Match(str);
                    if (match.Success)
                    {
                        var value = match.Groups[1].Value.Trim();
                        prop.SetValue(this, value);
                    }
                    else
                    {
                        File.AppendAllText(this.path, $"{prop.Name} = {prop.GetValue(this)}\r\n");
                    }
                }
            }
            else
            {
                using (var stream = File.CreateText(this.path))
                {
                    foreach (var prop in this.GetType().GetProperties())
                    {
                        stream.WriteLine($"{prop.Name} = {prop.GetValue(this)}");
                    }
                }

                Console.WriteLine($"Config file created -> {this.path}");
            }
        }

        public string VoiceChannel { get; private set; }

        public string HeartbeatChannel { get; private set; }

        public string DiscordKey { get; private set; }

        public string BeamUsername { get; private set; }

        public string BeamPassword { get; private set; }

        public string BeamChannel { get; private set; }

        public string GoogleUsername { get; private set; }

        public string GooglePassword { get; private set; }

        public string GoogleKey { get; private set; }

        public string GoogleSearchCx { get; private set; }

        public string GiphyKey { get; private set; }

        public string MusixmatchKey { get; private set; }

        public string CatKey { get; private set; }

        public string MashapeKey { get; private set; }

        public string ImgurKey { get; private set; }
    }
}
