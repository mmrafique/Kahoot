using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LeaderboardController : MonoBehaviour
{
    [SerializeField] private Transform contentTransform;
    [SerializeField] private LeaderboardItemController leaderboardItemPrefab;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI titleText;

    private void Start()
    {
        string kahootId = PlayerPrefs.GetString("CurrentKahootId", "");
        DisplayLeaderboard(kahootId);

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    private void DisplayLeaderboard(string kahootId)
    {
        // Clear existing items
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        if (string.IsNullOrEmpty(kahootId))
        {
            titleText.text = "Leaderboards";
            return;
        }

        List<LeaderboardEntry> entries = LeaderboardManager.LoadEntries(kahootId);
        var filePaths = KahootLoader.GetAllKahootFilePaths();
        Kahoot kahoot = null;
        foreach (var filePath in filePaths)
        {
            try
            {
                var k = KahootLoader.LoadFromFile(filePath);
                if (k.id == kahootId)
                {
                    kahoot = k;
                    break;
                }
            }
            catch { }
        }

        if (kahoot != null)
        {
            titleText.text = $"Leaderboard: {kahoot.title}";
        }

        // Display top 10
        for (int i = 0; i < Mathf.Min(10, entries.Count); i++)
        {
            LeaderboardItemController item = Instantiate(leaderboardItemPrefab, contentTransform);
            item.SetData(i + 1, entries[i].username, entries[i].score, (int)entries[i].timeTaken, entries[i].date.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }

    private void OnBackClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
