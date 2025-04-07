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
        public int HitNRunReset { get; set; } = 0;
        public int Wrench { get; set; } = 0;
        public int Missions { get; set; } = 0;
        public int BonusMissions { get; set; } = 0;
        public int Wasps { get; set; } = 0;
        public int Cards { get; set; } = 0;
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

    public void SetMissions(int value)
    {
        Data.Missions = value; 
        Save();
    }

    public void SetBonusMissions(int value)
    {
        Data.BonusMissions = value;
        Save();
    }

    public void SetWasps(int value)
    {
        Data.Wasps = value;
        Save();
    }

    public void SetCards(int value)
    {
        Data.Cards = value;
        Save();
    }

    public int GetHitNRunReset() => Data.HitNRunReset;
    public int GetWrench() => Data.Wrench;
    public int GetMissions() => Data.Missions;
    public int GetBonusMissions() => Data.BonusMissions;
    public int GetWasps() => Data.Wasps;
    public int GetCards() => Data.Cards;
}
