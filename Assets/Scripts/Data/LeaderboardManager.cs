using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;
public class LeaderboardEntry
{
    public string username;
    public int score;
    public float timeTaken;
    public DateTime date;
}

public static class LeaderboardManager
{
    public static string Folder => Path.Combine(Application.persistentDataPath, "leaderboards");

    static LeaderboardManager()
    {
        try
        {
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException($"LeaderboardManager static ctor error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static void SaveResult(string kahootId, string username, int score, float timeTaken)
    {
        try
        {
            EnsureFolder();
            string path = Path.Combine(Folder, kahootId + ".xml");
            XDocument doc;
            if (File.Exists(path))
            {
                doc = XDocument.Load(path);
            }
            else
            {
                doc = new XDocument(new XElement("leaderboard", new XAttribute("kahootId", kahootId)));
            }
            XElement root = doc.Element("leaderboard");
            root.Add(new XElement("entry",
                        new XElement("username", username),
                        new XElement("score", score),
                        new XElement("timeTaken", timeTaken),
                        new XElement("date", DateTime.UtcNow.ToString("o"))
                    ));
            doc.Save(path);
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException($"Error saving leaderboard for {kahootId}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static List<LeaderboardEntry> LoadEntries(string kahootId)
    {
        var result = new List<LeaderboardEntry>();
        try
        {
            EnsureFolder();
            string path = Path.Combine(Folder, kahootId + ".xml");
            if (!File.Exists(path)) return result;
            var doc = XDocument.Load(path);
            foreach (var e in doc.Descendants("entry"))
            {
                try
                {
                    var ent = new LeaderboardEntry
                    {
                        username = (string)e.Element("username") ?? "Anon",
                        score = int.Parse((string)e.Element("score") ?? "0"),
                        timeTaken = float.Parse((string)e.Element("timeTaken") ?? "0"),
                        date = DateTime.Parse((string)e.Element("date"))
                    };
                    result.Add(ent);
                }
                catch (Exception parseEx)
                {
                    ExceptionLogger.LogException($"Error parsing leaderboard entry in {path}: {parseEx.Message}");
                }
            }
            result = result.OrderByDescending(x => x.score).ThenBy(x => x.timeTaken).ToList();
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException($"Error loading leaderboard for {kahootId}: {ex.Message}\n{ex.StackTrace}");
        }
        return result;
    }

    private static void EnsureFolder()
    {
        if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);
    }
}
