using SHARMemory.SHAR.Events;
using SHARMemory.SHAR.Events.GameFlow;
using SHARMemory.SHAR.Events.GameDataManager;
using System;
using System.Threading;
using System.Threading.Tasks;
using SHARMemory.SHAR.Events.CharacterSheet;
using SHARMemory.SHAR.Classes;
using SHARMemory.SHAR.Events.GameplayManager;
using SHARMemory.SHAR.Events.SoundManager;
using System.Linq;
using SHARMemory.SHAR.Events.CardGallery;
using System.Collections.Generic;
using SHARMemory.SHAR.Events.RewardsManager;

namespace SHARMemory.SHAR;

/// <summary>
/// A class to watch parts of SHAR memory and triggers events.
/// </summary>
public sealed class Watcher
{
    /// <summary>
    /// The polling interval between memory checks. Defaults to 500ms.
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// An event handler for when there's an error in the <see cref="Tick"/> loop.
    /// </summary>
    public event AsyncEventHandler<ErrorEventArgs> Error;

    /// <summary>
    /// An event handler for when the game's state changes.
    /// </summary>
    public event AsyncEventHandler<GameStateChangedEventArgs> GameStateChanged;

    /// <summary>
    /// An event handler for when a New Game is started.
    /// </summary>
    public event AsyncEventHandler<NewGameEventArgs> NewGame;
    /// <summary>
    /// An event handler for when a Save Game is loaded.
    /// </summary>
    public event AsyncEventHandler<LoadGameEventArgs> LoadGame;

    /// <summary>
    /// An event handler for when a Mission is completed.
    /// </summary>
    public event AsyncEventHandler<MissionCompleteEventArgs> MissionComplete;
    /// <summary>
    /// An event handler for when a Bonus Mission is completed.
    /// </summary>
    public event AsyncEventHandler<BonusMissionCompleteEventArgs> BonusMissionComplete;
    /// <summary>
    /// An event handler for when a Street Race is completed.
    /// </summary>
    public event AsyncEventHandler<StreetRaceCompleteEventArgs> StreetRaceComplete;
    /// <summary>
    /// An event handler for when a Gamble/Wager Race is completed.
    /// </summary>
    public event AsyncEventHandler<GambleRaceCompleteEventArgs> GambleRaceComplete;
    /// <summary>
    /// An event handler for when a Wasp is destroyed.
    /// </summary>
    public event AsyncEventHandler<WaspDestroyedEventArgs> WaspDestroyed;
    /// <summary>
    /// An event handler for when an FMV is watched.
    /// </summary>
    public event AsyncEventHandler<FMVWatchedEventArgs> FMVWatched;
    /// <summary>
    /// An event handler for when a Car is purchased.
    /// </summary>
    public event AsyncEventHandler<CarPurchasedEventArgs> CarPurchased;
    /// <summary>
    /// An event handler for when a Skin/Costume is purchased.
    /// </summary>
    public event AsyncEventHandler<SkinPurchasedEventArgs> SkinPurchased;
    /// <summary>
    /// An event handler for when the current Skin/Costume is changed.
    /// </summary>
    public event AsyncEventHandler<SkinChangedEventArgs> SkinChanged;
    /// <summary>
    /// An event handler for when a Gag is viewed.
    /// </summary>
    public event AsyncEventHandler<GagViewedEventArgs> GagViewed;
    /// <summary>
    /// An event handler for when the Coins value changes.
    /// </summary>
    public event AsyncEventHandler<CoinsChangedEventArgs> CoinsChanged;
    /// <summary>
    /// An event handler for when a Persistent Object is destroyed.
    /// </summary>
    public event AsyncEventHandler<PersistentObjectDestroyedEventArts> PersistentObjectDestroyed;

    /// <summary>
    /// An event handler for when a Collector Card is collected.
    /// </summary>
    public event AsyncEventHandler<CardCollectedEventArgs> CardCollected;

    /// <summary>
    /// An event handler for when the Mission Index changes.
    /// </summary>
    public event AsyncEventHandler<MissionIndexChangedEventArgs> MissionIndexChanged;
    /// <summary>
    /// An event handler for when the Current Mission changes.
    /// </summary>
    public event AsyncEventHandler<MissionChangedEventArgs> MissionChanged;
    /// <summary>
    /// An event handler for when the Current Stage changes.
    /// </summary>
    public event AsyncEventHandler<MissionStageChangedEventArgs> MissionStageChanged;
    /// <summary>
    /// An event handler for when the Current Vehicle changes.
    /// </summary>
    public event AsyncEventHandler<VehicleChangedEventArgs> VehicleChanged;

    /// <summary>
    /// An event handler for when a new Dialog starts playing.
    /// </summary>
    public event AsyncEventHandler<DialogPlaying> DialogPlaying;

    /// <summary>
    /// An event handler for when Merchandise is purchased.
    /// </summary>
    public event AsyncEventHandler<MerchandisePurchased> MerchandisePurchased;

    private readonly Memory Memory;

    private bool Running = false;

    internal Watcher(Memory memory)
    {
        Memory = memory;
    }

    /// <summary>
    /// Starts the polling loop.
    /// </summary>
    /// <returns>Returns <c>true</c> if the loop starts, <c>false</c> if the loop is already running.</returns>
    public bool Start()
    {
        if (Running)
            return false;

        Running = true;
        Task.Run(Tick);
        return true;
    }

    /// <summary>
    /// Stops the polling loop.
    /// </summary>
    /// <returns>Returns <c>true</c> if the loop stops, <c>false</c> if the loop wasn't running.</returns>
    public bool Stop()
    {
        if (!Running)
            return false;

        Running = false;
        return true;
    }

    private void ResetData()
    {
        lastGameState = Memory.Singletons.GameFlow?.NextContext ?? GameFlow.GameState.PreLicence;

        for (int level = 0; level < 7; level++)
        {
            missionsComplete[level] = new bool[8];
            for (int i = 0; i < missionsComplete[level].Length; i++)
            {
                missionsComplete[level][i] = false;
            }
            bonusMissionComplete[level] = false;

            racesComplete[level] = new bool[3];
            for (int i = 0; i < racesComplete[level].Length; i++)
            {
                racesComplete[level][i] = false;
            }
            gambleRaceComplete[level] = false;

            cardsCollected[level] = new bool[7];
            for (int i = 0; i < cardsCollected[level].Length; i++)
            {
                cardsCollected[level][i] = false;
            }

            waspsDestroyed[level] = 0;

            fmvsWatched[level] = false;

            carsPurchased[level] = 0;
            skinsPurchased[level] = 0;

            currentSkins[level] = "NULL";

            gagsViewed[level] = new bool[32];
            for (int i = 0; i < gagsViewed[level].Length; i++)
            {
                gagsViewed[level][i] = false;
            }

            merchandiseEarned[level].Clear();
        }

        for (int i = 0; i < persistentObjectStates.Length; i++)
            persistentObjectStates[i] = false;

        lastCoins = 0;

        var gameplayManager = Memory.Globals.GameplayManager;
        lastLevel = gameplayManager?.LevelData.Level;
        lastMissionIndex = gameplayManager?.GetCurrentMissionIndex() ?? -1;
        lastVehicle = gameplayManager?.CurrentVehicle?.Address ?? 0;
    }

    private GameFlow.GameState lastGameState = GameFlow.GameState.PreLicence;
    private async void Tick()
    {
        ResetData();
        while (Memory.IsRunning && Running)
        {
            await Task.Delay(Interval);
            if (!Memory.IsRunning)
                break;

            try
            {
                var gameFlow = Memory.Singletons.GameFlow;
                if (gameFlow == null)
                    continue;
                var gameState = gameFlow.NextContext;
                switch (gameState)
                {
                    case GameFlow.GameState.PreLicence:
                    //case GameFlow.GameState.Licence:
                        lastGameState = gameState;
                        continue;
                }
                if (gameState != lastGameState)
                {
                    await GameStateChanged.InvokeAsync(Memory, new(lastGameState, gameState), CancellationToken.None);
                    lastGameState = gameState;
                }

                await CheckGameDataManager();
                await CheckCharacterSheet();
                await CheckCardGallery();
                await CheckGameplayManager();
                await CheckSoundManager();
                await CheckRewardsManager();
            }
            catch (Exception ex)
            {
                await Error.InvokeAsync(Memory, new ErrorEventArgs(ex), CancellationToken.None);
            }
        }
        Running = false;
    }

    private bool lastIsGameLoaded = false;
    private GameDataManager.FileOperation lastFileOperation = GameDataManager.FileOperation.None;
    private async Task CheckGameDataManager()
    {
        var gameDataManager = Memory.Singletons.GameDataManager;
        if (gameDataManager == null)
            return;

        if (lastIsGameLoaded && !gameDataManager.IsGameLoaded)
        {
            ResetData();
            lastIsGameLoaded = gameDataManager.IsGameLoaded;
            await NewGame.InvokeAsync(Memory, new(), CancellationToken.None);
        }
        else
        {
            lastIsGameLoaded = gameDataManager.IsGameLoaded;
        }

        var newFileOperation = gameDataManager.CurrentFileOperation;
        if (lastFileOperation != newFileOperation)
        {
            if (newFileOperation == GameDataManager.FileOperation.LoadComplete)
            {
                ResetData();
                lastFileOperation = newFileOperation;
                await LoadGame.InvokeAsync(Memory, new(), CancellationToken.None);
            }
            else
            {
                lastFileOperation = newFileOperation;
            }
        }
    }

    private readonly bool[][] missionsComplete = new bool[7][];
    private readonly bool[] bonusMissionComplete = new bool[7];
    private readonly bool[][] racesComplete = new bool[7][];
    private readonly bool[] gambleRaceComplete = new bool[7];
    private readonly int[] waspsDestroyed = new int[7];
    private readonly bool[] fmvsWatched = new bool[7];
    private readonly int[] carsPurchased = new int[7];
    private readonly int[] skinsPurchased = new int[7];
    private readonly string[] currentSkins = new string[7];
    private readonly bool[][] gagsViewed = new bool[7][];
    private readonly bool[] persistentObjectStates = new bool[82 * 128]; // 75 region sections + 7 level sectors * 128 bits per sector
    private int lastCoins = 0;
    private async Task CheckCharacterSheet()
    {
        var characterSheet = Memory.Singletons.CharacterSheetManager?.CharacterSheet;
        if (characterSheet == null)
            return;

        var levelArray = characterSheet.LevelList.ToArray();
        for (int level = 0; level < 7; level++)
        {
            var levelData = levelArray[level];

            for (int i = 0; i < missionsComplete[level].Length; i++)
            {
                var missionData = levelData.Missions.List[i];

                if (!missionsComplete[level][i] && missionData.Completed)
                {
                    missionsComplete[level][i] = missionData.Completed;
                    await MissionComplete.InvokeAsync(Memory, new(level, i), CancellationToken.None);
                }
                else
                {
                    missionsComplete[level][i] = missionData.Completed;
                }
            }
            if (!bonusMissionComplete[level] && levelData.BonusMission.Completed)
            {
                bonusMissionComplete[level] = levelData.BonusMission.Completed;
                await BonusMissionComplete.InvokeAsync(Memory, new(level), CancellationToken.None);
            }
            else
            {
                bonusMissionComplete[level] = levelData.BonusMission.Completed;
            }

            for (int i = 0; i < racesComplete[level].Length; i++)
            {
                var raceData = levelData.StreetRaces.List[i];

                if (!racesComplete[level][i] && raceData.Completed)
                {
                    racesComplete[level][i] = raceData.Completed;
                    await StreetRaceComplete.InvokeAsync(Memory, new(level, i), CancellationToken.None);
                }
                else
                {
                    racesComplete[level][i] = raceData.Completed;
                }
            }


            if (!gambleRaceComplete[level] && levelData.GambleRace.Completed)
            {
                gambleRaceComplete[level] = levelData.GambleRace.Completed;
                await GambleRaceComplete.InvokeAsync(Memory, new(level), CancellationToken.None);
            }
            else
            {
                gambleRaceComplete[level] = levelData.GambleRace.Completed;
            }

            var lastWaspsDestroyed = waspsDestroyed[level];
            waspsDestroyed[level] = levelData.WaspsDestroyed;
            if (levelData.WaspsDestroyed > lastWaspsDestroyed)
            {
                for (int i = 1; i <= levelData.WaspsDestroyed - lastWaspsDestroyed; i++)
                {
                    await WaspDestroyed.InvokeAsync(Memory, new(level, lastWaspsDestroyed + i), CancellationToken.None);
                }
            }

            if (!fmvsWatched[level] && levelData.FMVUnlocked)
            {
                fmvsWatched[level] = levelData.FMVUnlocked;
                await FMVWatched.InvokeAsync(Memory, new(level), CancellationToken.None);
            }
            else
            {
                fmvsWatched[level] = levelData.FMVUnlocked;
            }

            var lastNumCarsPurchased = carsPurchased[level];
            carsPurchased[level] = levelData.NumCarsPurchased;
            if (levelData.NumCarsPurchased > lastNumCarsPurchased)
            {
                for (int i = 1; i <= levelData.NumCarsPurchased - lastNumCarsPurchased; i++)
                {
                    await CarPurchased.InvokeAsync(Memory, new(level, lastNumCarsPurchased + i), CancellationToken.None);
                }
            }

            var lastNumSkinsPurchased = skinsPurchased[level];
            skinsPurchased[level] = levelData.NumSkinsPurchased;
            if (levelData.NumSkinsPurchased > lastNumSkinsPurchased)
            {
                for (int i = 1; i <= levelData.NumSkinsPurchased - lastNumSkinsPurchased; i++)
                {
                    await SkinPurchased.InvokeAsync(Memory, new(level, lastNumSkinsPurchased + i), CancellationToken.None);
                }
            }

            if (levelData.CurrentSkin != currentSkins[level])
            {
                var lastSkin = currentSkins[level];
                currentSkins[level] = levelData.CurrentSkin;
                await SkinChanged.InvokeAsync(Memory, new(level, lastSkin, levelData.CurrentSkin), CancellationToken.None);
            }

            for (int i = 0; i < gagsViewed[level].Length; i++)
            {
                var gagViewed = (levelData.GagMask & (1 << i)) != 0;
                if (!gagsViewed[level][i] && gagViewed)
                {
                    gagsViewed[level][i] = gagViewed;
                    await GagViewed.InvokeAsync(Memory, new(level, i), CancellationToken.None);
                }
                else
                {
                    gagsViewed[level][i] = gagViewed;
                }
            }
        }

        var currentPersistentObjectStates = characterSheet.PersistentObjectStates.ToArray();
        for (int i = 0; i < persistentObjectStates.Length; i++)
        {
            uint index = (uint)i / 8;
            bool state = (currentPersistentObjectStates[index] & (1 << i % 8)) == 0;
            if (state != persistentObjectStates[i])
            {
                persistentObjectStates[i] = state;
                if (state)
                    await PersistentObjectDestroyed.InvokeAsync(Memory, new((uint)i), CancellationToken.None);
            }
        }

        var newCoins = characterSheet.Coins;
        if (newCoins != lastCoins)
        {
            lastCoins = newCoins;
            await CoinsChanged.InvokeAsync(Memory, new(lastCoins, newCoins), CancellationToken.None);
        }
    }

    private readonly bool[][] cardsCollected = new bool[7][];
    private async Task CheckCardGallery()
    {
        var cardGallery = Memory.Singletons.CardGallery;
        if (cardGallery == null)
            return;

        var collectedCards = cardGallery.CollectedCards.ToArray();
        for (int level = 0; level < collectedCards.Length; level++)
        {
            var levelCards = collectedCards[level].Cards.ToArray();

            for (int card = 0; card < levelCards.Length; card++)
            {
                bool collected = levelCards[card] != null;

                if (collected != cardsCollected[level][card])
                {
                    cardsCollected[level][card] = collected;

                    if (collected)
                        await CardCollected.InvokeAsync(Memory, new(level, card), CancellationToken.None);
                }
            }
        }
    }

    private Globals.RenderEnums.LevelEnum? lastLevel = null;
    private int lastMissionIndex = -1;
    private uint lastMission = 0;
    private uint lastStage = 0;
    private uint lastVehicle = 0;
    private async Task CheckGameplayManager()
    {
        var gameplayManager = Memory.Globals.GameplayManager;
        if (gameplayManager == null)
        {
            lastLevel = null;
            lastMissionIndex = -1;
            return;
        }

        var levelData = gameplayManager.LevelData;
        var missionIndex = gameplayManager.GetCurrentMissionIndex();
        if (!lastLevel.HasValue || lastLevel.Value != levelData.Level || lastMissionIndex != missionIndex)
        {
            lastLevel = levelData.Level;
            lastMissionIndex = missionIndex;
            await MissionIndexChanged.InvokeAsync(Memory, new(lastLevel, lastMissionIndex, levelData.Level, missionIndex), CancellationToken.None);
        }

        if (gameplayManager is MissionManager missionManager)
        {
            var mission = missionManager.GetCurrentMission();
            if ((mission?.Address ?? 0) != lastMission)
            {
                Classes.Mission lastMission2 = null;
                if (lastMission != 0)
                {
                    try
                    {
                        lastMission2 = Memory.ClassFactory.Create<Classes.Mission>(lastMission);
                    }
                    catch { }
                }
                lastMission = mission?.Address ?? 0;
                await MissionChanged.InvokeAsync(Memory, new(lastMission2, mission, levelData.Level, missionIndex), CancellationToken.None);
            }

            var stage = mission?.GetCurrentStage();
            if ((stage?.Address ?? 0) != lastStage)
            {
                MissionStage lastStage2 = null;
                if (lastStage != 0)
                {
                    try
                    {
                        lastStage2 = Memory.ClassFactory.Create<MissionStage>(lastStage);
                    }
                    catch { }
                }
                lastStage = stage?.Address ?? 0;
                await MissionStageChanged.InvokeAsync(Memory, new(lastStage2, stage, levelData.Level, missionIndex), CancellationToken.None);
            }
        }

        var vehicle = gameplayManager.CurrentVehicle;
        if ((vehicle?.Address ?? 0) != lastVehicle)
        {
            Vehicle lastVehicle2 = null;
            if (lastVehicle != 0)
            {
                try
                {
                    lastVehicle2 = Memory.ClassFactory.Create<Vehicle>(lastVehicle);
                }
                catch { }
            }
            lastVehicle = vehicle?.Address ?? 0;
            await VehicleChanged.InvokeAsync(Memory, new(lastVehicle2, vehicle), CancellationToken.None);
        }
    }

    private uint lastDialogAddress = 0;
    private async Task CheckSoundManager()
    {
        var soundManager = Memory.Singletons.SoundManager;
        if (soundManager == null)
        {
            lastDialogAddress = 0;
            return;
        }

        var nowPlaying = soundManager.DialogCoordinator?.PlaybackQueue.NowPlaying?.Dialog;
        if (nowPlaying == null)
        {
            lastDialogAddress = 0;
        }
        else
        {
            if (nowPlaying is DialogSelectionGroup nowPlayingGroup)
                nowPlaying = nowPlayingGroup.DialogVector[nowPlayingGroup.CurrentlyPlayingDialog];
            else if (nowPlaying is Conversation nowPlayingConversation)
                nowPlaying = nowPlayingConversation.CurrentLine;

            if (nowPlaying == null)
            {
                lastDialogAddress = 0;
            }
            else if (nowPlaying.Address != lastDialogAddress)
            {
                lastDialogAddress = nowPlaying.Address;
                await DialogPlaying.InvokeAsync(Memory, new(nowPlaying), CancellationToken.None);
            }
        }
    }


    private readonly Dictionary<int, List<bool>> merchandiseEarned = new() { { 0, new() }, { 1, new() }, { 2, new() }, { 3, new() }, { 4, new() }, { 5, new() }, { 6, new() } };

    private async Task CheckRewardsManager()
    {
        var rewardsManager = Memory.Singletons.RewardsManager;
        if (rewardsManager == null)
        {
            for (int level = 0; level < 7; level++)
                merchandiseEarned[level].Clear();
            return;
        }

        var tokenStores = rewardsManager.LevelTokenStoreList.ToArray();
        for (int level = 0; level < 7; level++)
        {
            var levelTokenStore = tokenStores[level];


            var levelMerchandise = merchandiseEarned[level];
            while (levelMerchandise.Count > levelTokenStore.Counter)
                levelMerchandise.RemoveAt(levelMerchandise.Count - 1);
            while (levelMerchandise.Count < levelTokenStore.Counter)
                levelMerchandise.Add(false);

            for (int merchandiseIndex = 0; merchandiseIndex < levelTokenStore.Counter; merchandiseIndex++)
            {
                var merchandise = Memory.Functions.GetMerchandise(level, merchandiseIndex);
                if (merchandise == null)
                    continue;

                if (merchandise.Earned != levelMerchandise[merchandiseIndex])
                {
                    levelMerchandise[merchandiseIndex] = merchandise.Earned;
                    if (merchandise.Earned)
                    {
                        await MerchandisePurchased.InvokeAsync(Memory, new(level, merchandiseIndex, merchandise), CancellationToken.None);
                    }
                }
            }
        }
    }
}
