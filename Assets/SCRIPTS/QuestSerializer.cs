using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SFB;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public static class QuestSerializer
{
    private static readonly string DefaultExportPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "QuestExports");

    private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
    {
        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All,
        Formatting = Newtonsoft.Json.Formatting.Indented,
        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
        ContractResolver = new DefaultContractResolver
        {
            IgnoreSerializableAttribute = false,
            IgnoreSerializableInterface = false
        }
    };

    static QuestSerializer()
    {
        if (!Directory.Exists(DefaultExportPath))
        {
            Directory.CreateDirectory(DefaultExportPath);
        }
    }

    public static void ExportQuest(BaseQuest quest)
    {
        try
        {
            quest.BeforeSerialize();

            string defaultName = SanitizeFileName(quest.Title);
            var path = StandaloneFileBrowser.SaveFilePanel(
                "Export Quest",
                DefaultExportPath,
                defaultName,
                new[] { new ExtensionFilter("JSON Files", "json") }
            );

            if (!string.IsNullOrEmpty(path))
            {
                string json = JsonConvert.SerializeObject(quest, SerializerSettings);
                File.WriteAllText(path, json);
                Debug.Log($"Exported quest: {quest.Title} to {path}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Export failed for {quest.Title}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static void ExportAllQuests(List<BaseQuest> quests)
    {
        try
        {
            foreach (var quest in quests)
            {
                quest.BeforeSerialize();
            }

            var path = StandaloneFileBrowser.SaveFilePanel(
                "Export All Quests",
                DefaultExportPath,
                "AllQuests",
                new[] { new ExtensionFilter("JSON Files", "json") }
            );

            if (!string.IsNullOrEmpty(path))
            {
                string json = JsonConvert.SerializeObject(quests, SerializerSettings);
                File.WriteAllText(path, json);
                Debug.Log($"Exported {quests.Count} quests to {path}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Bulk export failed: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static List<BaseQuest> ImportQuests()
    {
        var importedQuests = new List<BaseQuest>();

        // Open file browser to select JSON files
        var paths = StandaloneFileBrowser.OpenFilePanel(
            "Import Quest(s)",
            DefaultExportPath,
            new[] { new ExtensionFilter("JSON Files", "json") },
            true
        );

        if (paths.Length == 0)
        {
            Debug.Log("Import canceled by user");
            return importedQuests;
        }

        foreach (var path in paths)
        {
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    BaseQuest quest = JsonConvert.DeserializeObject<BaseQuest>(json, SerializerSettings);

                    if (quest != null)
                    {
                        quest.AfterDeserialize();
                        importedQuests.Add(quest);
                        Debug.Log($"Imported quest from: {path}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to import {path}: {ex.Message}");
                }
            }
        }

        return importedQuests;
    }

    private static string SanitizeFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }
}