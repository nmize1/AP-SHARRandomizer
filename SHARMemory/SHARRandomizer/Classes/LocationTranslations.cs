using Newtonsoft.Json;
using SHARRandomizer;
using System;
using System.Collections.Generic;
using System.IO;

public class LocationTranslations
{
    public LevelData level1 { get; set; }
    public LevelData level2 { get; set; }
    public LevelData level3 { get; set; }
    public LevelData level4 { get; set; }
    public LevelData level5 { get; set; }
    public LevelData level6 { get; set; }
    public LevelData level7 { get; set; }


    public class LevelData
    {
        public List<ltMission> missions { get; set; }
        [JsonProperty("bonus missions")]
        public List<ltBonusMission> bonus_missions { get; set; }
        public List<ltWasp> wasps { get; set; }
        public List<ltCard> cards { get; set; }
        public List<ltGag> gags { get; set; }
        public List<ltShop> shops { get; set; }
    }

    public class ltMission
    {
        public string name { get; set; }
        public string id { get; set; }
        public long apid { get; set; }
        public int index { get; set; }
        public string[] translations { get; set; }
    }

    public class ltBonusMission
    {
        public string name { get; set; }
        public string id { get; set; }
        public long apid { get; set; }
    }

    public class ltWasp
    {
        public string name { get; set; }
        public string id { get; set; }
        public long apid { get; set; }
    }

    public class ltCard
    {
        public string name { get; set; }
        public string id { get; set; }
        public long apid { get; set; }
    }

    public class ltGag
    {
        public string name { get; set; }
        public string id { get; set; }
        public long apid { get; set; }
    }

    public class ltShop
    {
        public string name { get; set; }
        public string id { get; set; }
        public long apid { get; set; }
    }

    public static LocationTranslations LoadFromJson(string jsonFilePath)
    {
        try
        {
            string jsonString = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<LocationTranslations>(jsonString);
        }
        catch (Exception ex)
        {
            Common.WriteLog($"Error loading JSON: {ex}", "LocationTranslations::LoadFromJson");
            return null;
        }
    }

    public string getMissionName(int index, int level, uint language = 0)
    {
        List<LevelData> Levels = [level1, level2, level3, level4, level5, level6, level7];
        var mission = Levels[level].missions[index];
        return mission.translations.Length > language ? mission.translations[language] : mission.name;
    }

    public long getAPID(string id, string type)
    {
        List<LevelData> Levels = new List<LevelData> { level1, level2, level3, level4, level5, level6, level7 };

        var typeSelectors = new Dictionary<string, Func<LevelData, IEnumerable<dynamic>>>()
        {
            { "missions", l => l.missions },
            { "bonus missions", l => l.bonus_missions},
            { "wasp", l => l.wasps },
            { "card", l => l.cards },
            { "gag", l => l.gags },
            { "shop", l => l.shops }
        };

        if (!typeSelectors.TryGetValue(type.ToLower(), out var selector))
            throw new ArgumentException("Invalid type specified.");

        foreach (var level in Levels)
        {
            var collection = selector(level);
            var item = collection?.FirstOrDefault(e => e.id == id);
            if (item != null)
            {
                Common.WriteLog($"Sending {item.apid}", "LocationTranslations::getAPID");
                return item.apid;
            }
        }

        return -1;
    }

    public (string type, string name) getTypeAndNameByAPID(long apid)
    {
        List<LevelData> Levels = new List<LevelData> { level1, level2, level3, level4, level5, level6, level7 };

        var typeSelectors = new Dictionary<string, Func<LevelData, IEnumerable<dynamic>>>
        {
            { "mission", l => l.missions },
            { "bonus missions", l => l.bonus_missions},
            { "wasp", l => l.wasps },
            { "card", l => l.cards },
            { "gag", l => l.gags },
            { "shop", l => l.shops }
        };

        foreach (var level in Levels)
        {
            foreach (var kvp in typeSelectors)
            {
                string typeName = kvp.Key;
                var collection = kvp.Value(level);

                var item = collection?.FirstOrDefault(e => e.apid == apid);
                if (item != null)
                    return (typeName, item.name);
            }
        }

        return (null, null);
    }




    public void PrintData()
    {
        Common.WriteLog("Level 1 Data:", "PrintData");
        PrintLevelData(level1);

        Common.WriteLog("\nLevel 2 Data:", "PrintData");
        PrintLevelData(level2);

        Common.WriteLog("\nLevel 3 Data:", "PrintData");
        PrintLevelData(level3);

        Common.WriteLog("\nLevel 4 Data:", "PrintData");
        PrintLevelData(level4);

        Common.WriteLog("\nLevel 5 Data:", "PrintData");
        PrintLevelData(level5);

        Common.WriteLog("\nLevel 6 Data:", "PrintData");
        PrintLevelData(level6);

        Common.WriteLog("\nLevel 7 Data:", "PrintData");
        PrintLevelData(level7);
    }

    private void PrintLevelData(LevelData levelData)
    {
        Common.WriteLog("  Missions:", "PrintLevelData");
        foreach (var mission in levelData.missions)
        {
            Common.WriteLog($"    - Name: {mission.name}, APID: {mission.apid}, Index: {mission.index}", "PrintLevelData");
        }

        Common.WriteLog("  Bonus Missions:", "PrintLevelData");
        foreach (var bonusMission in levelData.bonus_missions)
        {
            Common.WriteLog($"    - Name: {bonusMission.name}, APID: {bonusMission.apid}", "PrintLevelData");
        }

        Common.WriteLog("  Wasps:", "PrintLevelData");
        foreach (var wasp in levelData.wasps)
        {
            Common.WriteLog($"    - Name: {wasp.name}, ID: {wasp.id}, APID: {wasp.apid}", "PrintLevelData");
        }

        Common.WriteLog("  Cards:", "PrintLevelData");
        foreach (var card in levelData.cards)
        {
            Common.WriteLog($"    - Name: {card.name}, ID: {card.id}, APID: {card.apid}", "PrintLevelData");
        }

        Common.WriteLog("  Gags:", "PrintLevelData");
        foreach (var gag in levelData.gags)
        {
            Common.WriteLog($"    - Name: {gag.name}, APID: {gag.apid}", "PrintLevelData");
        }

        Common.WriteLog("  Shops:", "PrintLevelData");
        foreach (var shop in levelData.shops)
        {
            Common.WriteLog($"    - Name: {shop.name}, APID: {shop.apid}", "PrintLevelData");
        }
    }
}
