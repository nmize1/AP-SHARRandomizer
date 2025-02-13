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

    public string getMissionName(int index, int level)
    {
        List<LevelData> Levels = new List<LevelData> { level1, level2, level3, level4, level5, level6, level7 };
        return Levels[level].missions[index].name;    
    }

    public long getAPID(string id, string type)
    {
        List<LevelData> Levels = new List<LevelData>{ level1, level2, level3, level4, level5, level6, level7 };
        foreach (var level in Levels)
        {
            switch (type.ToLower())
            {
                case "mission":
                    foreach (var mission in level.missions)
                    {
                        if (mission.id == id)
                            return mission.apid;
                    }
                    break;

                case "bonus_mission":
                    foreach (var bonusMission in level.bonus_missions)
                    {
                        if (bonusMission.id == id)
                            return bonusMission.apid;
                    }
                    break;

                case "wasp":
                    foreach (var wasp in level.wasps)
                    {
                        if (wasp.id == id)
                            return wasp.apid;
                    }
                    break;

                case "card":
                    foreach (var card in level.cards)
                    {

                        if (card.id == id)
                            return card.apid;

                    }
                    break;

                case "gag":
                    foreach (var gag in level.gags)
                    {
                        if (gag.id == id)
                            return gag.apid;
                    }
                    break;

                case "shop":
                    foreach (var store in level.shops)
                    {
                        if (store.id == id)
                            return store.apid;
                    }
                    break;

                default:
                    throw new ArgumentException("Invalid type specified.");
            }
        }

        return -1;
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
