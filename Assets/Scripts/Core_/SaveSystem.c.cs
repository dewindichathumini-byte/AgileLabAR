using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int pre = -1;
    public int post = -1;
    public int finalScore = -1;
    public List<string> completed = new List<string>();
}

public static class SaveSystem
{
    static string FilePath
    {
        get { return Path.Combine(Application.persistentDataPath, "progress.json"); }
    }

    public static SaveData Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                SaveData d = JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath));
                if (d != null) return d;
            }
        }
        catch (System.Exception e) { Debug.LogWarning("Load failed: " + e.Message); }
        return new SaveData();
    }

    public static void Save(SaveData d)
    {
        try { File.WriteAllText(FilePath, JsonUtility.ToJson(d)); }
        catch (System.Exception e) { Debug.LogWarning("Save failed: " + e.Message); }
    }

    public static void MarkCompleted(string lessonName)
    {
        SaveData d = Load();
        if (!d.completed.Contains(lessonName))
        {
            d.completed.Add(lessonName);
            Save(d);
        }
    }
}
