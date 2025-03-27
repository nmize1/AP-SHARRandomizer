using Newtonsoft.Json;
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
            Console.WriteLine($"Error loading JSON: {ex.Message}");
            return null;
        }
    }

    public string getMissionName(int index, int level, int language = 0)
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
                Console.WriteLine($"Sending {item.apid}");
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
        Console.WriteLine("Level 1 Data:");
        PrintLevelData(level1);

        Console.WriteLine("\nLevel 2 Data:");
        PrintLevelData(level2);

        Console.WriteLine("\nLevel 3 Data:");
        PrintLevelData(level3);

        Console.WriteLine("\nLevel 4 Data:");
        PrintLevelData(level4);

        Console.WriteLine("\nLevel 5 Data:");
        PrintLevelData(level5);

        Console.WriteLine("\nLevel 6 Data:");
        PrintLevelData(level6);

        Console.WriteLine("\nLevel 7 Data:");
        PrintLevelData(level7);
    }

    private void PrintLevelData(LevelData levelData)
    {
        Console.WriteLine("  Missions:");
        foreach (var mission in levelData.missions)
        {
            Console.WriteLine($"    - Name: {mission.name}, APID: {mission.apid}, Index: {mission.index}");
        }

        Console.WriteLine("  Bonus Missions:");
        foreach (var bonusMission in levelData.bonus_missions)
        {
            Console.WriteLine($"    - Name: {bonusMission.name}, APID: {bonusMission.apid}");
        }

        Console.WriteLine("  Wasps:");
        foreach (var wasp in levelData.wasps)
        {
            Console.WriteLine($"    - Name: {wasp.name}, ID: {wasp.id}, APID: {wasp.apid}");
        }

        Console.WriteLine("  Cards:");
        foreach (var card in levelData.cards)
        {
            Console.WriteLine($"    - Name: {card.name}, ID: {card.id}, APID: {card.apid}");
        }

        Console.WriteLine("  Gags:");
        foreach (var gag in levelData.gags)
        {
            Console.WriteLine($"    - Name: {gag.name}, APID: {gag.apid}");
        }

        Console.WriteLine("  Shops:");
        foreach (var shop in levelData.shops)
        {
            Console.WriteLine($"    - Name: {shop.name}, APID: {shop.apid}");
        }
    }
}
