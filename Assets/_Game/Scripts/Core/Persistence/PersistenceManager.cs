using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using QuestionManagement;

public static class PersistenceManager
{
    public static string persistentDataPath;
    public static string localDataPath;
    private const string gameplayConfigExtension = "Gameplay Configs";
    private const string databaseProfileExtension = "Database Profiles";
    private const string exampleCSV =
        "Question Type,QuestionText,AnswerText,IsCorrect,HostNotes\n" +
        "R1,Identify the animals listed in the Bluey opening titles,Bluey,true,Some notes on this question\n" +
        ",,Snickers,true,\n" +
        ",,Muffin,true,\n" +
        ",,My name is \"Winton\",true,\n" +
        ",,Strawberry,false,\n" +
        "R2,Identify the genuine British towns,Shitterton,true,Some notes on this question\n" +
        ",,Fulking Hill,true,\n" +
        ",,Cocking,true,\n" +
        ",,Skidmark Lane,false,\n" +
        ",,Oily Basterd,false,\n" +
        "R3,Identify the video games,BioShock,true,Some notes on this question\n" +
        ",,Inception,false,\n" +
        ",,Untitled Goose Game,true,";

    public static List<GameplayConfig> storedGameplayConfigs = new List<GameplayConfig>();
    public static List<DatabaseProfile> storedDatabaseProfiles = new List<DatabaseProfile>();

    private static Questions _persistentDatabase = new Questions();
    public static Questions PersistentDatabase
    {
        get { return _persistentDatabase; }
        set
        {
            _persistentDatabase = value;
        }
    }

    private static HackboxConfig _HackboxConfig = new HackboxConfig();
    public static HackboxConfig HackboxConfig
    {
        get { return _HackboxConfig; }
        set
        {
            _HackboxConfig = value;
            WriteHackboxConfig();
        }
    }

    private static GameplayConfig _currentGameplayConfig = new GameplayConfig();
    public static GameplayConfig CurrentGameplayConfig
    {
        get { return _currentGameplayConfig; }
        set
        {
            _currentGameplayConfig = value;
            foreach (GameplayConfig gpf in storedGameplayConfigs.Where(x => x != value))
                gpf.IsCurrent = false;
            value.IsCurrent = true;
            WriteGameplayConfigs();
        }
    }

    private static DatabaseProfile _currentDatabaseProfile = new DatabaseProfile();
    public static DatabaseProfile CurrentDatabaseProfile
    {
        get { return _currentDatabaseProfile; }
        set
        {
            _currentDatabaseProfile = value;
            foreach (DatabaseProfile dbp in storedDatabaseProfiles.Where(x => x != value))
                dbp.IsCurrent = false;
            value.IsCurrent = true;
            WriteDatabaseProfiles();
        }
    }

    public static void OnStartup()
    {
        SetDataPaths();
        ReadHackboxConfig();
        ReadGameplayConfigs();
        ReadQuestionDatabase();
        ReadDatabaseProfiles();
    }

    public static void SetDataPaths()
    {
        persistentDataPath = Application.persistentDataPath;
        localDataPath = Directory.GetParent(Application.dataPath).ToString().Replace("\\", "/");
        if (!Directory.Exists(persistentDataPath))
        {
            Directory.CreateDirectory(persistentDataPath);
            DebugLog.Print("Data path directory created", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
        }
        persistentDataPath += "/";
        if (!Directory.Exists(persistentDataPath + gameplayConfigExtension))
        {
            Directory.CreateDirectory(persistentDataPath + gameplayConfigExtension);
            DebugLog.Print("Gameplay config subdirectory created", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
        }
        if (!Directory.Exists(persistentDataPath + databaseProfileExtension))
        {
            Directory.CreateDirectory(persistentDataPath + databaseProfileExtension);
            DebugLog.Print("Database profile subdirectory created", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
        }

        if (!Directory.Exists(localDataPath + "/Question Data"))
        {
            Directory.CreateDirectory(localDataPath + "/Question Data");
            File.WriteAllText(localDataPath + "/Question Data/Question Data Template.csv", exampleCSV);
            DebugLog.Print("Assets subdirectory and demo created", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
        }
    }

    #region Hackbox Config

    private static void ReadHackboxConfig()
    {
        if (!File.Exists(persistentDataPath + "HackboxConfig.json"))
        {
            File.WriteAllText(persistentDataPath + "HackboxConfig.json", JsonConvert.SerializeObject(new HackboxConfig(), Formatting.Indented));
            DebugLog.Print("Default Hackbox config created", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
        }
        else
            DebugLog.Print("Default Hackbox config restored", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Orange);

        JsonConvert.PopulateObject(File.ReadAllText(persistentDataPath + "HackboxConfig.json"), HackboxConfig);
        var x = HackboxConfig.DefaultGridGap;
    }

    public static void WriteHackboxConfig()
    {
        File.WriteAllText(persistentDataPath + "HackboxConfig.json", JsonConvert.SerializeObject(HackboxConfig, Formatting.Indented));
    }

    #endregion

    #region Gameplay Config

    private static void ReadGameplayConfigs()
    {
        var storedConfigs = Directory.GetFiles(persistentDataPath + gameplayConfigExtension, "*.json");

        if(storedConfigs.Length < 1)
        {
            var def = new GameplayConfig()
            {
                LockConfig = true,
            };
            File.WriteAllText(persistentDataPath + gameplayConfigExtension + $"/{def.ID}.json", JsonConvert.SerializeObject(def, Formatting.Indented));
            storedGameplayConfigs.Add(def);

            CurrentGameplayConfig = def;
            DebugLog.Print("Default gameplay configs created", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
        }
        else
        {
            storedGameplayConfigs = storedConfigs.Select(config => JsonConvert.DeserializeObject<GameplayConfig>(File.ReadAllText(config))).OrderBy(x => x.Epoch).ToList();
            if(storedGameplayConfigs.FirstOrDefault(x => x.IsCurrent) == null)
                storedGameplayConfigs.FirstOrDefault().IsCurrent = true;
            CurrentGameplayConfig = storedGameplayConfigs.FirstOrDefault(x => x.IsCurrent);
            DebugLog.Print("Gameplay configs restored", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Orange);
        }
    }

    public static void WriteGameplayConfigs()
    {
        foreach(GameplayConfig gcf in storedGameplayConfigs)
            File.WriteAllText(persistentDataPath + gameplayConfigExtension + $"/{gcf.ID}.json", JsonConvert.SerializeObject(gcf, Formatting.Indented));
    }

    public static void OnDeleteConfig()
    {
        File.Delete(persistentDataPath + gameplayConfigExtension + $"/{CurrentGameplayConfig.ID}.json");
        storedGameplayConfigs.Remove(CurrentGameplayConfig);
        CurrentGameplayConfig = storedGameplayConfigs.FirstOrDefault();
    }

    #endregion

    #region Question Database

    private static void ReadQuestionDatabase()
    {
        if (!File.Exists(persistentDataPath + "QuestionDatabase.json"))
        {
            File.WriteAllText(persistentDataPath + "QuestionDatabase.json", JsonConvert.SerializeObject(PersistentDatabase, Formatting.Indented));
            DebugLog.Print("Empty question database created", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
        }
        else
            DebugLog.Print("Question database restored", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Orange);

        JsonConvert.PopulateObject(File.ReadAllText(persistentDataPath + "QuestionDatabase.json"), PersistentDatabase);

        //(MainMenuManager.Get.GetDBMan() as DatabaseManager).BuildQuestionObjects();
    }

    private static void ReadDatabaseProfiles()
    {
        var storedDbProfiles = Directory.GetFiles(persistentDataPath + databaseProfileExtension, "*.json");

        if (storedDbProfiles.Length < 1)
        {
            var def = new DatabaseProfile()
            {
                LockConfig = true
            };
            File.WriteAllText(persistentDataPath + databaseProfileExtension + $"/{def.ID}.json", JsonConvert.SerializeObject(def, Formatting.Indented));
            storedDatabaseProfiles.Add(def);

            CurrentDatabaseProfile = def;
            DebugLog.Print("Default database profile created", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
        }
        else
        {
            storedDatabaseProfiles = storedDbProfiles.Select(profile => JsonConvert.DeserializeObject<DatabaseProfile>(File.ReadAllText(profile))).OrderBy(x => x.Epoch).ToList();
            if (storedDatabaseProfiles.FirstOrDefault(x => x.IsCurrent) == null)
                storedDatabaseProfiles.FirstOrDefault().IsCurrent = true;
            CurrentDatabaseProfile = storedDatabaseProfiles.FirstOrDefault(x => x.IsCurrent);
            DebugLog.Print("Database profiles restored", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Orange);
        }

        SetDatabaseProfile();
    }

    public static void WriteQuestionDatabase()
    {
        File.WriteAllText(persistentDataPath + "QuestionDatabase.json", JsonConvert.SerializeObject(PersistentDatabase, Formatting.Indented));
    }

    public static void WriteDatabaseProfiles()
    {
        foreach (DatabaseProfile dbp in storedDatabaseProfiles)
            File.WriteAllText(persistentDataPath + databaseProfileExtension + $"/{dbp.ID}.json", JsonConvert.SerializeObject(dbp, Formatting.Indented));
    }

    private static void SetDatabaseProfile()
    {
        //(MainMenuManager.Get.GetDBMan() as DatabaseManager).SetProfileSpecifics();
    }

    public static void OnDeleteProfile()
    {
        File.Delete(persistentDataPath + databaseProfileExtension + $"/{CurrentDatabaseProfile.ID}.json");
        storedDatabaseProfiles.Remove(CurrentDatabaseProfile);
        CurrentDatabaseProfile = storedDatabaseProfiles.FirstOrDefault();
    }

    public static void OnDeleteQuestion(Question q)
    {
        PersistentDatabase.Remove(q);
        //(MainMenuManager.Get.GetDBMan() as DatabaseManager).OnDeleteQuestion(q);

        foreach (DatabaseProfile p in storedDatabaseProfiles)
            p.UsedQsOnThisProfile.Remove(q.ID);

        WriteQuestionDatabase();
        WriteDatabaseProfiles();
    }

    #endregion
}
