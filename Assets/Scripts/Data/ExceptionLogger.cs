using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class ExceptionLogger
{
    public static string ReportsFolder => Path.Combine(Application.persistentDataPath, "reports");

    static ExceptionLogger()
    {
        try
        {
            if (!Directory.Exists(ReportsFolder)) Directory.CreateDirectory(ReportsFolder);
        }
        catch (Exception)
        {
            // no-op during initialisation
        }
    }

    public static void LogException(string message)
    {
        try
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filename = $"report_{timestamp}.txt";
            string path = Path.Combine(ReportsFolder, filename);
            File.WriteAllText(path, $"Time: {DateTime.Now.ToString("o")}\n\n{message}");
            Debug.LogWarning($"ExceptionLogger wrote report: {path}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"ExceptionLogger failed to write report: {ex.Message}");
        }
    }

    // Permite generar un reporte sin necesidad de una excepción
    public static void LogReport(string title, string body)
    {
        try
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filename = $"report_{timestamp}.txt";
            string path = Path.Combine(ReportsFolder, filename);

            string content = $"Title: {title}\nTime: {DateTime.Now:o}\n\n{body}";
            File.WriteAllText(path, content);
            Debug.LogWarning($"ExceptionLogger wrote report: {path}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"ExceptionLogger failed to write report: {ex.Message}");
        }
    }
}
