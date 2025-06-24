using SFB; // Standalone File Browser namespace
using System;
using UnityEngine;

public static class NativeFileBrowser
{
    public static string[] OpenFilePanel(string title, string directory, string extension, bool multiSelect)
    {
        try
        {
            var extensions = new[] {
                new ExtensionFilter("JSON Files", "json"),
                new ExtensionFilter("All Files", "*")
            };

            return StandaloneFileBrowser.OpenFilePanel(title, directory, extensions, multiSelect);
        }
        catch (Exception ex)
        {
            Debug.LogError($"File dialog error: {ex.Message}");
            return new string[0];
        }
    }

    public static string SaveFilePanel(string title, string directory, string defaultName, string extension)
    {
        try
        {
            var extensions = new[] {
                new ExtensionFilter("JSON Files", "json")
            };

            return StandaloneFileBrowser.SaveFilePanel(title, directory, defaultName, extensions);
        }
        catch (Exception ex)
        {
            Debug.LogError($"File dialog error: {ex.Message}");
            return "";
        }
    }
}