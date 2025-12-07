using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreatorQuestionRowController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionNumberText;
    [SerializeField] private TMP_InputField questionTextInput;
    [SerializeField] private TMP_InputField option1Input;
    [SerializeField] private TMP_InputField option2Input;
    [SerializeField] private TMP_InputField option3Input;
    [SerializeField] private TMP_InputField option4Input;
    [SerializeField] private TMP_Dropdown correctAnswerDropdown;
    [SerializeField] private Button deleteButton;

    private KahootCreatorController parentController;
    private int questionNumber;

    private void OnEnable()
    {
        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(OnDeleteClicked);
        }
    }

    private void OnDisable()
    {
        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveListener(OnDeleteClicked);
        }
    }

    public void Initialize(int number, KahootCreatorController controller)
    {
        parentController = controller;
        questionNumber = number;
        SetQuestionNumber(number);

        // Setup correct answer dropdown
        if (correctAnswerDropdown != null)
        {
            correctAnswerDropdown.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>
            {
                new TMP_Dropdown.OptionData("Select the correct answer"),
                new TMP_Dropdown.OptionData("Answer 1"),
                new TMP_Dropdown.OptionData("Answer 2"),
                new TMP_Dropdown.OptionData("Answer 3"),
                new TMP_Dropdown.OptionData("Answer 4")
            };
            correctAnswerDropdown.AddOptions(options);
            correctAnswerDropdown.value = 0;
            correctAnswerDropdown.RefreshShownValue();
        }
    }

    public void SetQuestionNumber(int number)
    {
        questionNumber = number;
        if (questionNumberText != null)
        {
            questionNumberText.text = $"Question {number}";
        }
    }

    public Question GetQuestion(int id)
    {
        if (string.IsNullOrWhiteSpace(questionTextInput.text))
            return null;

        // Collect only non-empty options
        var options = new List<string>();
        if (!string.IsNullOrWhiteSpace(option1Input.text)) options.Add(option1Input.text);
        if (!string.IsNullOrWhiteSpace(option2Input.text)) options.Add(option2Input.text);
        if (!string.IsNullOrWhiteSpace(option3Input.text)) options.Add(option3Input.text);
        if (!string.IsNullOrWhiteSpace(option4Input.text)) options.Add(option4Input.text);

        // Need at least 2 options
        if (options.Count < 2)
        {
            return null;
        }

        // Validate correct index is within bounds (subtract 1 for placeholder)
        int correctIdx = correctAnswerDropdown != null ? correctAnswerDropdown.value - 1 : 0;
        if (correctIdx < 0 || correctIdx >= options.Count)
        {
            return null;
        }

        return new Question
        {
            questionId = id,
            text = questionTextInput.text,
            options = options,
            correctIndex = correctIdx
        };
    }

    private void OnDeleteClicked()
    {
        if (parentController != null)
        {
            parentController.RemoveQuestion(this);
        }
    }
}
