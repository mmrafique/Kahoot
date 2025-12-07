using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI timerText; // optional, se mantiene sin modificar si no se asigna
    [SerializeField] private bool updateTimerLabel = false;
    [SerializeField] private Image timerBarFill;
    [SerializeField] private Transform answersPanel;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject feedbackContainer;

    [SerializeField] private AnswerButtonController answerButtonPrefab;
    [SerializeField] private float feedbackDisplayDuration = 2f;
    [SerializeField] private float delayBetweenQuestions = 3f;

    private Kahoot currentKahoot;
    private int currentQuestionIndex = 0;
    private float timeRemaining;
    private bool isAnswered = false;
    private int correctAnswersCount = 0;
    private float totalGameTime = 0f;

    private List<AnswerButtonController> answerButtons = new List<AnswerButtonController>();
    private CanvasGroup feedbackCanvasGroup;

    // Color palette
    private Color[] answerColors = new Color[]
    {
        new Color(1f, 0.2f, 0.33f, 1f),      // #FF3355 - Red
        new Color(0.11f, 0.69f, 0.96f, 1f),  // #1CB0F6 - Blue
        new Color(0.2f, 0.84f, 0.51f, 1f),   // #32D583 - Green
        new Color(1f, 0.78f, 0.16f, 1f)      // #FFC629 - Yellow
    };

    private Color correctColor = new Color(0.2f, 0.84f, 0.51f, 1f);    // #32D583
    private Color incorrectColor = new Color(1f, 0.2f, 0.33f, 1f);     // #FF3355

    private void Start()
    {
        LoadKahoot();
        InitializeAnswerButtons();
        SetupFeedback();
        DisplayQuestion();
    }

    private void Update()
    {
        if (!isAnswered && currentKahoot != null && currentQuestionIndex < currentKahoot.questions.Count)
        {
            UpdateTimer();
        }

        totalGameTime += Time.deltaTime;
    }

    private void LoadKahoot()
    {
        string kahootId = PlayerPrefs.GetString("CurrentKahootId", "");

        if (string.IsNullOrEmpty(kahootId))
        {
            Debug.LogError("No kahoot ID found!");
            SceneManager.LoadScene("KahootList");
            return;
        }

        var filePaths = KahootLoader.GetAllKahootFilePaths();
        foreach (var filePath in filePaths)
        {
            try
            {
                var kahoot = KahootLoader.LoadFromFile(filePath);
                if (kahoot.id == kahootId)
                {
                    currentKahoot = kahoot;
                    break;
                }
            }
            catch { }
        }

        if (currentKahoot == null)
        {
            Debug.LogError($"Failed to load kahoot: {kahootId}");
            SceneManager.LoadScene("KahootList");
            return;
        }

        headerText.text = currentKahoot.title;
    }

    private void InitializeAnswerButtons()
    {
        // Clear existing buttons
        foreach (Transform child in answersPanel)
        {
            Destroy(child.gameObject);
        }
        answerButtons.Clear();
    }

    private void SetupFeedback()
    {
        if (feedbackContainer != null)
        {
            feedbackCanvasGroup = feedbackContainer.GetComponent<CanvasGroup>();
            if (feedbackCanvasGroup == null)
            {
                feedbackCanvasGroup = feedbackContainer.AddComponent<CanvasGroup>();
            }
            feedbackContainer.SetActive(false);
        }
    }

    private void DisplayQuestion()
    {
        if (currentQuestionIndex >= currentKahoot.questions.Count)
        {
            EndGame();
            return;
        }

        isAnswered = false;
        Question currentQuestion = currentKahoot.questions[currentQuestionIndex];

        questionText.text = currentQuestion.text;
        timeRemaining = currentKahoot.timePerQuestion;

        InitializeAnswerButtons();
        CreateAnswerButtons(currentQuestion);

        UpdateTimerUI();
    }

    private void CreateAnswerButtons(Question question)
    {
        for (int i = 0; i < question.options.Count; i++)
        {
            AnswerButtonController button = Instantiate(answerButtonPrefab, answersPanel);
            button.Initialize(question.options[i], answerColors[i], i, this);
            answerButtons.Add(button);
        }
    }

    private void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            HandleTimeUp();
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (currentKahoot == null)
            return;

        float duration = Mathf.Max(currentKahoot.timePerQuestion, 0.0001f);
        float clampedTime = Mathf.Max(timeRemaining, 0f);

        if (timerBarFill != null)
        {
            timerBarFill.fillAmount = Mathf.Clamp01(clampedTime / duration);
        }

        if (updateTimerLabel && timerText != null)
        {
            timerText.text = $"{Mathf.CeilToInt(clampedTime)}s";
        }
    }

    public void OnAnswerSelected(int answerIndex)
    {
        if (isAnswered)
            return;

        isAnswered = true;
        Question currentQuestion = currentKahoot.questions[currentQuestionIndex];
        bool isCorrect = answerIndex == currentQuestion.correctIndex;

        if (isCorrect)
        {
            correctAnswersCount++;
        }

        ShowFeedback(isCorrect);
        DisableAllButtons();
        StartCoroutine(MoveToNextQuestion());
    }

    private void HandleTimeUp()
    {
        if (!isAnswered)
        {
            isAnswered = true;
            ShowFeedback(false);
            DisableAllButtons();
            StartCoroutine(MoveToNextQuestion());
        }
    }

    private void ShowFeedback(bool isCorrect)
    {
        if (feedbackContainer == null)
            return;

        feedbackContainer.SetActive(true);

        if (isCorrect)
        {
            feedbackText.text = "Correct!";
            feedbackText.color = correctColor;
        }
        else
        {
            feedbackText.text = "Incorrect";
            feedbackText.color = incorrectColor;
        }
    }

    private void DisableAllButtons()
    {
        foreach (AnswerButtonController button in answerButtons)
        {
            button.Disable();
        }
    }

    private IEnumerator MoveToNextQuestion()
    {
        yield return new WaitForSeconds(feedbackDisplayDuration);

        if (feedbackContainer != null)
        {
            feedbackContainer.SetActive(false);
        }

        currentQuestionIndex++;

        yield return new WaitForSeconds(delayBetweenQuestions - feedbackDisplayDuration);

        DisplayQuestion();
    }

    private void EndGame()
    {
        // Calculate score
        int totalScore = CalculateScore();

        // Save result to XML
        SaveResult(totalScore);

        // Auto-generate a match report
        TryWriteAutoReport(totalScore);

        // Go to Results scene
        SceneManager.LoadScene("Results");
    }

    private int CalculateScore()
    {
        int totalQuestions = currentKahoot.questions.Count;
        float percentageCorrect = (float)correctAnswersCount / totalQuestions;
        int score = Mathf.RoundToInt(percentageCorrect * 1000); // Max 1000 points

        return score;
    }

    private void SaveResult(int score)
    {
        string username = PlayerProfile.Username;
        string kahootId = currentKahoot.id;

        LeaderboardManager.SaveResult(kahootId, username, score, totalGameTime);

        // Store result for Results scene
        PlayerPrefs.SetInt("LastScore", score);
        PlayerPrefs.SetInt("LastTime", Mathf.RoundToInt(totalGameTime));
        PlayerPrefs.SetInt("LastCorrect", correctAnswersCount);
        PlayerPrefs.SetInt("LastTotal", currentKahoot.questions.Count);
        PlayerPrefs.Save();
    }

    private void TryWriteAutoReport(int score)
    {
        try
        {
            string username = PlayerProfile.Username;
            string kahootId = currentKahoot?.id ?? "unknown";
            string title = "Match report";
            string body =
                $"Kahoot: {currentKahoot?.title} ({kahootId})\n" +
                $"User: {username}\n" +
                $"Score: {score}\n" +
                $"Correct: {correctAnswersCount}/{currentKahoot?.questions.Count}\n" +
                $"Total time: {totalGameTime:F1}s";

            ExceptionLogger.LogReport(title, body);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Could not generate auto report: {ex.Message}");
        }
    }
}
