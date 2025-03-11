using SHARRandomizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class SaveData
{
    private string SAVEFILE;

    public class SaveFileData
    {
        public int HitNRunReset { get; set; }
        public int Wrench { get; set; }
        public Dictionary<int, int> ShopCosts { get; set; } = new Dictionary<int, int>();
    }

    private SaveFileData Data = new SaveFileData();

    public SaveData()
    {
        SAVEFILE = $"..\\Saves\\{ArchipelagoClient.SaveName}.json";
    }

    public void Load()
    {
        if (File.Exists(SAVEFILE))
        {
            string json = File.ReadAllText(SAVEFILE);
            Data = JsonSerializer.Deserialize<SaveFileData>(json) ?? new SaveFileData();
        }
        else
        {
            Data = new SaveFileData(); 
        }
    }

    public void Save()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(Data, options);
        Directory.CreateDirectory(Path.GetDirectoryName(SAVEFILE)!); 
        File.WriteAllText(SAVEFILE, json);
    }

    public void SetHitNRunReset(int value)
    {
        Data.HitNRunReset = value;
        Save();
    }

    public void SetWrench(int value)
    {
        Data.Wrench = value;
        Save();
    }

    public void SetShopCosts(Dictionary<int, int> shopCosts)
    {
        Data.ShopCosts = shopCosts;
        Save();
    }

    public int GetHitNRunReset() => Data.HitNRunReset;
    public int GetWrench() => Data.Wrench;
    public Dictionary<int, int> GetShopCosts() => Data.ShopCosts;

    public Dictionary<int, int> GetOrCreateShopCosts(Dictionary<int, int> defaultValues)
    {
        if (Data.ShopCosts != null && Data.ShopCosts.Count > 0)
        {
            return Data.ShopCosts;
        }

        Data.ShopCosts = new Dictionary<int, int>(defaultValues);
        Save();
        return Data.ShopCosts;
    }
}
