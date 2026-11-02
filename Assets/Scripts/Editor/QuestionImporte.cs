#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class QuestionImporter
{
    [MenuItem("AgileLab/Import Questions from CSV")]
    public static void Import()
    {
        const string csvPath = "Assets/Data/questions.csv";
        const string outDir = "Assets/ScriptableObjects/Questions";
        const string bankPath = "Assets/ScriptableObjects/QuestionBank.asset";

        if (!File.Exists(csvPath)) { Debug.LogError("CSV not found: " + csvPath); return; }
        Directory.CreateDirectory(outDir);

        var pre = new List<QuestionData>();
        var post = new List<QuestionData>();
        var fin = new List<QuestionData>();
        int n = 0;

        foreach (string raw in File.ReadAllLines(csvPath))
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            string[] f = raw.Split(';');
            if (f.Length < 8) { Debug.LogWarning("Skipped line: " + raw); continue; }
            string kind = f[0].Trim().ToLower();
            if (kind == "set") continue;   // header row

            int correct;
            if (!int.TryParse(f[6].Trim(), out correct)) { Debug.LogWarning("Bad number: " + raw); continue; }

            n++;
            string path = outDir + "/Q" + n.ToString("00") + "_" + kind + ".asset";
            QuestionData q = AssetDatabase.LoadAssetAtPath<QuestionData>(path);
            if (q == null)
            {
                q = ScriptableObject.CreateInstance<QuestionData>();
                AssetDatabase.CreateAsset(q, path);
            }
            q.question = f[1].Trim();
            q.options = new[] { f[2].Trim(), f[3].Trim(), f[4].Trim(), f[5].Trim() };
            q.correctIndex = correct - 1;
            q.feedback = f[7].Trim();
            q.usedInPreQuiz = kind == "pre";
            EditorUtility.SetDirty(q);

            if (kind == "pre") pre.Add(q);
            else if (kind == "post") post.Add(q);
            else fin.Add(q);
        }

        QuestionBank bank = AssetDatabase.LoadAssetAtPath<QuestionBank>(bankPath);
        if (bank == null)
        {
            bank = ScriptableObject.CreateInstance<QuestionBank>();
            AssetDatabase.CreateAsset(bank, bankPath);
        }
        bank.preQuestions = pre.ToArray();
        bank.postQuestions = post.ToArray();
        bank.finalQuestions = fin.ToArray();
        EditorUtility.SetDirty(bank);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Imported " + n + " questions (pre " + pre.Count + ", post " + post.Count + ", final " + fin.Count + ")");
    }
}
#endif