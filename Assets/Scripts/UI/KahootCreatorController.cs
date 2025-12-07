using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System;

public class KahootCreatorController : MonoBehaviour
{
    [SerializeField] private TMP_InputField titleInput;
    [SerializeField] private TMP_InputField descriptionInput;
    [SerializeField] private TMP_InputField timePerQuestionInput;
    [SerializeField] private Transform questionsContainer;
    [SerializeField] private CreatorQuestionRowController questionRowPrefab;
    [SerializeField] private Button addQuestionButton;
    [SerializeField] private Button deleteQuestionButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button demoButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button testErrorButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private List<CreatorQuestionRowController> questionRows = new List<CreatorQuestionRowController>();

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (addQuestionButton != null)
        {
            addQuestionButton.onClick.AddListener(OnAddQuestionClicked);
        }

        if (deleteQuestionButton != null)
        {
            deleteQuestionButton.onClick.AddListener(OnDeleteLastQuestionClicked);
        }

        if (saveButton != null)
        {
            saveButton.onClick.AddListener(OnSaveClicked);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelClicked);
        }

        if (demoButton != null)
        {
            demoButton.onClick.AddListener(OnDemoClicked);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }

        if (testErrorButton != null)
        {
            testErrorButton.onClick.AddListener(OnTestErrorClicked);
        }
    }

    private void OnAddQuestionClicked()
    {
        CreatorQuestionRowController newRow = Instantiate(questionRowPrefab, questionsContainer);
        newRow.Initialize(questionRows.Count + 1, this);
        questionRows.Add(newRow);
    }

    private void OnDeleteLastQuestionClicked()
    {
        if (questionRows.Count == 0)
        {
            ShowStatus("No questions to remove.", Color.red);
            return;
        }

        var last = questionRows[questionRows.Count - 1];
        questionRows.RemoveAt(questionRows.Count - 1);
        if (last != null)
        {
            Destroy(last.gameObject);
        }
        UpdateQuestionNumbers();
    }

    private void OnTestErrorClicked()
    {
        ExceptionLogger.LogReport("Test Error from Creator", "This is a test error generated from the Kahoot Creator scene for testing the error reporting system.");
        ShowStatus("Test error report created.", Color.green);
    }

    private void OnSaveClicked()
    {
        if (!ValidateForm())
            return;

        Kahoot kahoot = BuildKahoot();

        if (kahoot == null)
            return;

        if (SaveKahootToFile(kahoot))
        {
            ShowStatus("Kahoot saved successfully!", Color.green);
            Invoke(nameof(ReturnToList), 2f);
        }
        else
        {
            ShowStatus("Failed to save Kahoot.", Color.red);
        }
    }

    private void OnCancelClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnBackClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDemoClicked()
    {
        // Create 10 demo kahoots
        for (int i = 1; i <= 10; i++)
        {
            CreateDemoKahoot(i);
        }
        ShowStatus("10 demo Kahoots created.", Color.green);
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(titleInput.text))
        {
            ShowStatus("Please enter a title.", Color.red);
            return false;
        }

        if (questionRows.Count == 0)
        {
            ShowStatus("Add at least one question.", Color.red);
            return false;
        }

        if (!int.TryParse(timePerQuestionInput.text, out int timePerQuestion) || timePerQuestion <= 0)
        {
            ShowStatus("Time per question must be a positive number.", Color.red);
            return false;
        }

        return true;
    }

    private Kahoot BuildKahoot()
    {
        string title = titleInput.text;
        string description = descriptionInput.text;
        int timePerQuestion = int.Parse(timePerQuestionInput.text);

        Kahoot kahoot = new Kahoot
        {
            id = System.DateTime.Now.Ticks.ToString(),
            title = title,
            description = description,
            timePerQuestion = timePerQuestion,
            questions = new List<Question>()
        };

        for (int i = 0; i < questionRows.Count; i++)
        {
            Question question = questionRows[i].GetQuestion(i + 1);

            if (question == null)
            {
                ShowStatus($"Check question {i + 1}.", Color.red);
                return null;
            }

            kahoot.questions.Add(question);
        }

        return kahoot;
    }

    private bool SaveKahootToFile(Kahoot kahoot)
    {
        try
        {
            string kahotsPath = Path.Combine(Application.persistentDataPath, "kahoots");
            Directory.CreateDirectory(kahotsPath);

            string filePath = Path.Combine(kahotsPath, $"kahoot_{kahoot.id}.json");
            string json = JsonUtility.ToJson(kahoot, true);

            File.WriteAllText(filePath, json);
            return true;
        }
        catch (System.Exception ex)
        {
            ExceptionLogger.LogException($"KahootCreatorController: SaveKahootToFile - {ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }

    private void CreateDemoKahoot(int index)
    {
        // Create demo questions about history
        var historyQuestions = new List<Question>
        {
            new Question
            {
                questionId = 1,
                text = "In what year did the French Revolution begin?",
                options = new List<string> { "1789", "1776", "1804", "1815" },
                correctIndex = 0
            },
            new Question
            {
                questionId = 2,
                text = "Who was the first President of the United States?",
                options = new List<string> { "Thomas Jefferson", "George Washington", "John Adams", "Benjamin Franklin" },
                correctIndex = 1
            },
            new Question
            {
                questionId = 3,
                text = "In what year did World War II end?",
                options = new List<string> { "1943", "1944", "1945", "1946" },
                correctIndex = 2
            },
            new Question
            {
                questionId = 4,
                text = "Which empire built the Great Wall of China?",
                options = new List<string> { "Han Dynasty", "Ming Dynasty", "Qin Dynasty", "Tang Dynasty" },
                correctIndex = 2
            },
            new Question
            {
                questionId = 5,
                text = "Who discovered America in 1492?",
                options = new List<string> { "Leif Erikson", "Ferdinand Magellan", "Christopher Columbus", "Vasco da Gama" },
                correctIndex = 2
            }
        };

        Kahoot kahoot = new Kahoot
        {
            id = $"demo_history_{index}_{System.DateTime.Now.Ticks}",
            title = $"History Quiz {index}",
            description = $"Test your knowledge of world history",
            timePerQuestion = 20,
            questions = historyQuestions
        };

        // Save only to persistentDataPath (dynamic user kahoots)
        SaveKahootToFile(kahoot);
    }

    private void ShowStatus(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
        }
    }

    private void ReturnToList()
    {
        SceneManager.LoadScene("KahootList");
    }

    public void RemoveQuestion(CreatorQuestionRowController row)
    {
        questionRows.Remove(row);
        Destroy(row.gameObject);
        UpdateQuestionNumbers();
    }

    private void UpdateQuestionNumbers()
    {
        for (int i = 0; i < questionRows.Count; i++)
        {
            questionRows[i].SetQuestionNumber(i + 1);
        }
    }
}
