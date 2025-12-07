using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class ReportViewerController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reportContentText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button backButton;

    private void Start()
    {
        DisplayReport();

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    private void DisplayReport()
    {
        string reportPath = PlayerPrefs.GetString("CurrentReportPath", "");

        if (string.IsNullOrEmpty(reportPath) || !File.Exists(reportPath))
        {
            reportContentText.text = "File not found.";
            return;
        }

        try
        {
            string content = File.ReadAllText(reportPath);
            reportContentText.text = content;

            FileInfo fileInfo = new FileInfo(reportPath);
            titleText.text = $"Report: {fileInfo.Name}";

            // Reset scroll to top
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }
        catch (System.Exception ex)
        {
            reportContentText.text = $"Error reading file: {ex.Message}";
            ExceptionLogger.LogException($"ReportViewerController: DisplayReport - {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void OnBackClicked()
    {
        SceneManager.LoadScene("Reports");
    }
}
