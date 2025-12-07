using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI correctText;
    [SerializeField] private Button repeatButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button menuButton;

    private void Start()
    {
        DisplayResults();
        SetupButtons();
    }

    private void DisplayResults()
    {
        int score = PlayerPrefs.GetInt("LastScore", 0);
        int time = PlayerPrefs.GetInt("LastTime", 0);
        int correct = PlayerPrefs.GetInt("LastCorrect", 0);
        int total = PlayerPrefs.GetInt("LastTotal", 0);

        scoreText.text = $"Score: {score}";
        timeText.text = $"Time: {time}s";
        correctText.text = $"Correct answers: {correct}/{total}";
    }

    private void SetupButtons()
    {
        if (repeatButton != null)
        {
            repeatButton.onClick.AddListener(OnRepeatClicked);
        }

        if (leaderboardButton != null)
        {
            leaderboardButton.onClick.AddListener(OnLeaderboardClicked);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuClicked);
        }
    }

    private void OnRepeatClicked()
    {
        SceneManager.LoadScene("Game");
    }

    private void OnLeaderboardClicked()
    {
        SceneManager.LoadScene("Leaderboards");
    }

    private void OnMenuClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
