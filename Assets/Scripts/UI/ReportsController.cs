using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class ReportsController : MonoBehaviour
{
    [SerializeField] private Transform contentTransform;
    [SerializeField] private ReportItemController reportItemPrefab;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI noReportsText;

    private void Start()
    {
        DisplayReports();

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    private void DisplayReports()
    {
        // Clear existing items
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        string reportsPath = Path.Combine(Application.persistentDataPath, "reports");

        if (!Directory.Exists(reportsPath))
        {
            Directory.CreateDirectory(reportsPath);
        }

        string[] reportFiles = Directory.GetFiles(reportsPath, "report_*.txt");

        if (reportFiles.Length == 0)
        {
            noReportsText.gameObject.SetActive(true);
            return;
        }

        noReportsText.gameObject.SetActive(false);

        // Sort by date descending (newest first)
        System.Array.Sort(reportFiles, (a, b) => File.GetLastWriteTime(b).CompareTo(File.GetLastWriteTime(a)));

        foreach (string reportFile in reportFiles)
        {
            FileInfo fileInfo = new FileInfo(reportFile);
            ReportItemController item = Instantiate(reportItemPrefab, contentTransform);
            item.SetData(fileInfo.Name, fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"), reportFile);
        }
    }

    private void OnBackClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
