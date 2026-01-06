using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SHARRandomizerFrontend
{
    public class clsSettings
    {
        public string APPath { get; set; } = "";

        public string prevURL { get; set; } = "";
        public string prevPort { get; set; } = "";
        public string prevSlot { get; set; } = "";
        public string prvPass { get; set; } = "";
    }

    public static class SettingsManager
    {
        private static string SettingsFile =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public static void Save(clsSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(SettingsFile, json);
        }

        public static clsSettings Load()
        {
            if (!File.Exists(SettingsFile))
                return new clsSettings();

            var json = File.ReadAllText(SettingsFile);
            return JsonSerializer.Deserialize<clsSettings>(json) ?? new clsSettings();
        }
    }

}
