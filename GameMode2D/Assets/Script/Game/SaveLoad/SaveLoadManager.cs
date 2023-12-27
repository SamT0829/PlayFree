using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    private string jsonFolder;      //C:\Users\USER\AppData\LocalLow\GameWin\PlayMoreWinMore\SAVE
    private List<ISaveable> saveableList = new List<ISaveable>();
    private Dictionary<string, GameSaveData> saveDataDict = new Dictionary<string, GameSaveData>();

    private static SaveLoadManager _instance;
    public static SaveLoadManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            _instance = this;


        DontDestroyOnLoad(gameObject);

        /// <summary>
        /// Windows Store Apps: Application.persistentDataPath points to C:\Users\<user>\AppData\LocalLow\<company name>.

        // Windows Editor and Standalone Player: Application.persistentDataPath usually points to % userprofile %\AppData\LocalLow\< companyname >\< productname >.It is resolved by SHGetKnownFolderPath with FOLDERID_LocalAppDataLow, or SHGetFolderPathW with CSIDL_LOCAL_APPDATA if the former is not available.

        // WebGL: Application.persistentDataPath points to / idbfs /< md5 hash of data path> where the data path is the URL stripped of everything including and after the last '/' before any '?' components.

        // Linux: Application.persistentDataPath points to $XDG_CONFIG_HOME / unity3d or $HOME /.config / unity3d.

        // iOS: Application.persistentDataPath points to / var / mobile / Containers / Data / Application /< guid >/ Documents.

        // tvOS: Application.persistentDataPath is not supported and returns an empty string.

        // Android: Application.persistentDataPath points to / storage / emulated /< userid >/ Android / data /< packagename >/ files on most devices(some older phones might point to location on SD card if present), the path is resolved using android.content.Context.getExternalFilesDir.

        // Mac: Application.persistentDataPath points to the user Library folder. (This folder is often hidden.) In recent Unity releases user data is written into ~/ Library / Application Support / company name / product name.Older versions of Unity wrote into the ~/ Library / Caches folder, or ~/ Library / Application Support / unity.company name.product name. These folders are all searched for by Unity. The application finds and uses the oldest folder with the required data on your system.

        /// </summary>
        jsonFolder = Application.persistentDataPath + "/SAVE/";
    }

    public void Register(ISaveable saveable)
    {
        saveableList.Add(saveable);
    }

    public void Save()
    {
        saveDataDict.Clear();

        foreach (var saveable in saveableList)
        {
            saveDataDict.Add(saveable.GetType().Name, saveable.GenerateSaveData());
        }

        var resultPath = jsonFolder + "data.sav";

        var jsonData = JsonConvert.SerializeObject(saveDataDict, Formatting.Indented);

        if (!File.Exists(resultPath))
        {
            Directory.CreateDirectory(jsonFolder);
        }

        File.WriteAllText(resultPath, jsonData);
    }

    public void Load()
    {
        var resultPath = jsonFolder + "data.sav";

        if (!File.Exists(resultPath)) return;

        var stringData = File.ReadAllText(resultPath);

        if (stringData == string.Empty) return;

        var jsonData = JsonConvert.DeserializeObject<Dictionary<string, GameSaveData>>(stringData);

        foreach (var saveable in saveableList)
        {
            saveable.RestoreGameData(jsonData[saveable.GetType().Name]);
        }
    }
}
