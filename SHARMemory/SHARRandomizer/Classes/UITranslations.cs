#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SHARRandomizer;

public class UITranslations
{
    [JsonProperty]
    public List<RewardEntry> Entries { get; set; } = new List<RewardEntry>();

    public class RewardEntry
    {
        public string Name { get; set; }
        public List<string> Translations { get; set; } = new List<string>();
    }

    public static UITranslations LoadFromJson(string jsonFilePath)
    {
        try
        {
            string jsonString = File.ReadAllText(jsonFilePath);
            return new UITranslations
            {
                Entries = JsonConvert.DeserializeObject<List<RewardEntry>>(jsonString) ?? new List<RewardEntry>()
            };
        }
        catch (Exception ex)
        {
            Common.WriteLog($"Error loading JSON: {ex.Message}", "UITranslations::LoadFromJson");
            return new UITranslations();
        }
    }

    public List<string> GetTranslationsByName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return new List<string>();

        return Entries
            .FirstOrDefault(e =>
                !string.IsNullOrEmpty(e.Name) &&
                e.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?.Translations ?? new List<string>();
    }

    public string GetUITranslation(string name, uint language = 0)
    {
        return GetTranslationsByName(name)[(int)language];
    }


    public void PrintEntries()
    {
        if (Entries == null || !Entries.Any())
        {
            Common.WriteLog("No entries loaded.", "UITranslations::PrintEntries");
            return;
        }

        foreach (var entry in Entries)
        {
            Common.WriteLog($"Name: {entry.Name ?? "N/A"}", "UITranslations::PrintEntries");
            Common.WriteLog("Translations:", "UITranslations::PrintEntries");
            if (entry.Translations != null)
            {
                foreach (var translation in entry.Translations)
                {
                    Common.WriteLog($"  - {translation ?? "N/A"}", "UITranslations::PrintEntries");
                }
            }
            else
            {
                Common.WriteLog("  No translations", "UITranslations::PrintEntries");
            }
            Common.WriteLog("", "UITranslations::PrintEntries");
        }
    }
}