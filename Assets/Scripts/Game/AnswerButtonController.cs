using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerButtonController : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private Button button;

    private GameController gameController;
    private int answerIndex;
    private bool isEnabled = true;

    private Color originalColor;
    private Color selectedColor;
    private Color disabledColor;

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    public void Initialize(string answerOption, Color buttonColor, int index, GameController controller)
    {
        answerText.text = answerOption;
        answerIndex = index;
        gameController = controller;

        originalColor = buttonColor;
        buttonImage.color = originalColor;

        // Darker version for selected state
        selectedColor = new Color(
            buttonColor.r * 0.7f,
            buttonColor.g * 0.7f,
            buttonColor.b * 0.7f,
            buttonColor.a
        );

        // Lighter gray for disabled state
        disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        isEnabled = true;
    }

    private void OnButtonClicked()
    {
        if (!isEnabled)
            return;

        // Visual feedback - darken button
        buttonImage.color = selectedColor;

        // Notify GameController
        if (gameController != null)
        {
            gameController.OnAnswerSelected(answerIndex);
        }
    }

    public void Disable()
    {
        isEnabled = false;
        button.interactable = false;
        buttonImage.color = disabledColor;
    }

    public void Enable()
    {
        isEnabled = true;
        button.interactable = true;
        buttonImage.color = originalColor;
    }
}
