using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class KahootLoader
{
    public static string DefaultFolder => Path.Combine(Application.streamingAssetsPath, "kahoots_default");
    public static string UserFolder => Path.Combine(Application.persistentDataPath, "kahoots");

    public static void EnsureFolders()
    {
        try
        {
            if (!Directory.Exists(DefaultFolder))
                Directory.CreateDirectory(DefaultFolder);
            if (!Directory.Exists(UserFolder))
                Directory.CreateDirectory(UserFolder);
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException($"EnsureFolders failed: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static List<string> GetAllKahootFilePaths()
    {
        EnsureFolders();
        var list = new List<string>();
        try
        {
            if (Directory.Exists(DefaultFolder)) list.AddRange(Directory.GetFiles(DefaultFolder, "*.json"));
            if (Directory.Exists(UserFolder)) list.AddRange(Directory.GetFiles(UserFolder, "*.json"));
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException($"GetAllKahootFilePaths error: {ex.Message}\n{ex.StackTrace}");
        }
        return list;
    }

    public static Kahoot LoadFromFile(string path)
    {
        try
        {
            if (!File.Exists(path)) throw new FileNotFoundException($"Kahoot file not found: {path}");
            string json = File.ReadAllText(path);
            Kahoot kahoot = JsonUtility.FromJson<Kahoot>(json);
            if (kahoot == null)
            {
                ExceptionLogger.LogException($"JsonUtility returned null parsing {path}. Raw length: {json?.Length}");
                throw new Exception("Invalid JSON format for Kahoot");
            }
            if (string.IsNullOrEmpty(kahoot.id)) kahoot.id = Path.GetFileNameWithoutExtension(path);
            return kahoot;
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException($"Error loading Kahoot from {path}: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }

    public static void SaveKahootToUserFolder(string json, string filename)
    {
        try
        {
            EnsureFolders();
            string path = Path.Combine(UserFolder, filename);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException($"Error saving kahoot: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }
}
