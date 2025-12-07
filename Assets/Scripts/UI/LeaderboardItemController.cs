using UnityEngine;
using TMPro;

public class LeaderboardItemController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dateText;

    public void SetData(int position, string username, int score, int time, string date)
    {
        positionText.text = position.ToString();
        usernameText.text = username;
        scoreText.text = score.ToString();
        timeText.text = $"{time}s";
        dateText.text = date;
    }
}
