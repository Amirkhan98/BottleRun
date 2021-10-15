using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using MoreMountains.NiceVibrations;
using Amir.Utils;
using Amir.Level;

/// <summary>
/// Containt all player data.
/// </summary>
[Serializable]
public class PlayerData
{
    public int money;
    public int equippedHatID;
    public int equippedWeaponID;
    public List<PlayerUpgradeData> playerUpgradeData = new List<PlayerUpgradeData>(3);
    public bool allLevelComplete = false;
}

/// <summary>
/// Class for upgrade some stats with fixed step and increasing cost.
/// </summary>
[Serializable]
public class PlayerUpgradeData
{
    public int costMultiply = 20;
    public float stepUpgrade = 0;
    public int upgradeLevel = 1;
    public int upgradeCost = 110;

    public void Upgrade()
    {
        stepUpgrade += 0.02f;
        upgradeLevel += 1;
        costMultiply += costMultiply / 6;
        upgradeCost += costMultiply;
    }
}

/// <summary>
/// Game states
/// </summary>
public enum GameStatus
{
    Menu,
    Shop,
    Ride,
    Fly
}

/// <summary>
/// Singleton. Manager for game states, levels, player data.
/// </summary>
public class StaticManager : MonoBehaviour
{
    public PlayerData playerData;
    public GameStatus gameStatus;
    public LevelManager levelManager;
    public int filledWineGlasses;

    /// <summary>
    /// Level count what we will have in Resources/Levels/
    /// </summary>
    [PropertySpace(SpaceBefore = 0, SpaceAfter = 20)]
    public int uniqueLevelsCount = 10;


    [BoxGroup("DEBUG MODE", true, true)] [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    public bool debugMode = false;

    [ShowIf("debugMode")] [BoxGroup("DEBUG MODE", true, true)]
    public int debugLevel = 0;

    public static StaticManager instance;

    /// <summary>
    /// Called when money count changed.
    /// </summary>
    public delegate void OnValueChange();

    public static event OnValueChange onValueChange;

    internal static int levelID = 0;
    internal static int starsCollected = 2;
    internal static int moneyCollectedOnLevel = 500;
    internal static float currentMoneyMultiplier = 1;
    internal static bool reloadLevel = false;
    internal static bool reviveReload = false;
    internal static bool allLevelsReached = false;
    internal static int restartLevelID = -1;
    public static bool levelFinished;

    private static RandomNoRepeate randomLevelsNoRepeate = new RandomNoRepeate();

    void Awake()
    {
        levelFinished = false;

        if (debugMode)
            levelID = debugLevel;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 300;

        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        //DeslyticsManager.SetUniqueLevelsCount(uniqueLevelsCount);
        InitRandomLevels(uniqueLevelsCount);
        LoadPlayerData();
    }

    #region Player Data

    /// <summary>
    /// Load player data from PlayerPrefs.
    /// </summary>
    public static void LoadPlayerData()
    {
        if (!instance.debugMode)
            levelID = PlayerPrefs.GetInt("LevelID", 0);

        if (PlayerPrefs.HasKey("PlayerData"))
        {
            string serializedData = PlayerPrefs.GetString("PlayerData");
            //Debug.LogError("Player data : " + serializedData);
            instance.playerData = JsonConvert.DeserializeObject<PlayerData>(serializedData);
        }

        instance.levelManager.LoadLevelFromResources();
        // UIManager.instance.SetUpgradesStats(instance.playerData.playerUpgradeData);
        ValueChanged();
    }

    /// <summary>
    /// Save player data to PlayerPrefs.
    /// </summary>
    public static void SavePlayerData()
    {
        PlayerPrefs.SetInt("LevelID", levelID);

        string serializedData = JsonConvert.SerializeObject(instance.playerData, Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        PlayerPrefs.SetString("PlayerData", serializedData);
    }

    #endregion

    #region Money

    /// <summary>
    /// Adds money count to PlayerData
    /// </summary>
    /// <param name="count">Money count</param>
    public static void AddMoney(int count)
    {
        instance.playerData.money += count;
        ValueChanged();
    }

    /// <summary>
    /// Substract money count from PlayerData
    /// </summary>
    /// <param name="count">Money count</param>
    public static void SubstractMoney(int count)
    {
        instance.playerData.money -= count;
        ValueChanged();
    }

    /// <summary>
    /// Collect money to temp variable for show collected money on level.
    /// After showing Win or Lose Screen and clicked Claim button then need to call AddMoney(int count);
    /// </summary>
    /// <param name="count"></param>
    public static void AddMoneyOnLevel(int count)
    {
        moneyCollectedOnLevel += count;
        ValueChanged();
    }

    /// <summary>
    /// When we change money value we call that event for Vibrate and call all subscribed scripts
    /// </summary>
    private static void ValueChanged()
    {
        if (onValueChange != null)
        {
            onValueChange();
            LightVibrate();
        }
    }

    #endregion

    #region Level Management

    /// <summary>
    /// Initialize random levels list what we will load from resources if player all levels completed
    /// </summary>
    /// <param name="levelsCount">Amount of levels placed as prefab in Resources/Levels/ </param>
    private static void InitRandomLevels(int levelsCount)
    {
        randomLevelsNoRepeate.SetCount(levelsCount);
    }

    /// <summary>
    /// Get current level id. If player completed all levels return random level id 
    /// from 5 level to levelsCount setted in InitRandomLevels
    /// </summary>
    /// <returns></returns>
    public static int GetLevelID()
    {
        if (levelID > instance.uniqueLevelsCount - 1)
        {
            if (!instance.playerData.allLevelComplete)
            {
                //if all levels complete then send event to analytics by uncomment follow methods

                //DeslyticsManager.AllLevelsComplete();
                //DeslyticsManager.SetLevelsRandomizer(true);
            }

            if (reviveReload)
            {
                return restartLevelID;
            }

            int newID = randomLevelsNoRepeate.GetAvailable();
            while (newID <= 5) // Ignore first 5 levels because first levels too easy
                newID = randomLevelsNoRepeate.GetAvailable();

            restartLevelID = newID;
            return newID;
        }
        else return levelID;
    }

    public delegate void OnRestart();

    public static event OnRestart onRestart;

    /// <summary>
    /// When level completed or failed we need to restart temp variables
    /// </summary>
    public static void Restart()
    {
        moneyCollectedOnLevel = 0;
        starsCollected = 2;
        currentMoneyMultiplier = 0;
        onRestart?.Invoke();
    }

    public static void LoadNextLevel()
    {
        instance.levelManager.LoadLevelFromResources();
    }

    #endregion

    #region Vibration

    public static bool vibrationEnabled = true;

    /// <summary>
    /// LightVibrate
    /// </summary>
    public static void LightVibrate()
    {
        if (vibrationEnabled)
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
    }

    /// <summary>
    /// SuccessVibrate
    /// </summary>
    public static void SuccessVibrate()
    {
        if (vibrationEnabled)
            MMVibrationManager.Haptic(HapticTypes.Success);
    }

    /// <summary>
    /// FailureVibrate
    /// </summary>
    public static void FailureVibrate()
    {
        if (vibrationEnabled)
            MMVibrationManager.Haptic(HapticTypes.Failure);
    }

    /// <summary>
    /// SelectionVibrate
    /// </summary>
    public static void SelectionVibrate()
    {
        if (vibrationEnabled)
        {
#if UNITY_ANDROID
            MMNVAndroid.AndroidVibrate(30);
#endif
#if UNITY_IOS
                MMVibrationManager.Haptic(HapticTypes.Selection);
#endif
        }
    }

    #endregion
}