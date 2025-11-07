#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SHARRandomizer;

public class RewardTranslations
{
    [JsonProperty]
    public List<RewardEntry> Entries { get; set; } = new List<RewardEntry>();

    public class RewardEntry
    {
        public string Name { get; set; }
        public string InternalName { get; set; }
        public List<string> Translations { get; set; } = new List<string>();
    }

    public static RewardTranslations LoadFromJson(string jsonFilePath)
    {
        try
        {
            string jsonString = File.ReadAllText(jsonFilePath);
            return new RewardTranslations
            {
                Entries = JsonConvert.DeserializeObject<List<RewardEntry>>(jsonString) ?? new List<RewardEntry>()
            };
        }
        catch (Exception ex)
        {
            Common.WriteLog($"Error loading JSON: {ex.Message}", "RewardTranslations::LoadFromJson");
            return new RewardTranslations();
        }
    }

    public string GetInternalName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        return Entries
            .FirstOrDefault(e =>
                !string.IsNullOrEmpty(e.Name) &&
                e.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?.InternalName;
    }

    public string GetInternalNameByTranslation(string translation)
    {
        if (string.IsNullOrEmpty(translation))
            return null;

        return Entries
            .FirstOrDefault(e =>
                e.Translations != null &&
                e.Translations.Any(t =>
                    !string.IsNullOrEmpty(t) &&
                    t.Equals(translation, StringComparison.OrdinalIgnoreCase)))
            ?.InternalName;
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

    public List<string> GetTranslationsByInternalName(string internalName)
    {
        if (string.IsNullOrEmpty(internalName))
            return new List<string>();

        return Entries
            .FirstOrDefault(e =>
                !string.IsNullOrEmpty(e.InternalName) &&
                e.InternalName.Equals(internalName, StringComparison.OrdinalIgnoreCase))
            ?.Translations ?? new List<string>();
    }

    public void PrintEntries()
    {
        if (Entries == null || !Entries.Any())
        {
            Common.WriteLog("No entries loaded.", "RewardTranslations::PrintEntries");
            return;
        }

        foreach (var entry in Entries)
        {
            Common.WriteLog($"Name: {entry.Name ?? "N/A"}", "RewardTranslations::PrintEntries");
            Common.WriteLog($"Internal Name: {entry.InternalName ?? "N/A"}", "RewardTranslations::PrintEntries");
            Common.WriteLog("Translations:", "RewardTranslations::PrintEntries");
            if (entry.Translations != null)
            {
                foreach (var translation in entry.Translations)
                {
                    Common.WriteLog($"  - {translation ?? "N/A"}", "RewardTranslations::PrintEntries");
                }
            }
            else
            {
                Common.WriteLog("  No translations", "RewardTranslations::PrintEntries");
            }
            Common.WriteLog("", "RewardTranslations::PrintEntries");
        }
    }
}