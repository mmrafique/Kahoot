using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class KahootListItemController : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Button playButton;
    public GameObject star; // Opcional

    public void Setup(Kahoot kahoot, Action onPlay)
    {
        title.text = kahoot.title;
        description.text = $"{kahoot.questions?.Count ?? 0} questions";
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(() => onPlay?.Invoke());
        if (star != null)
            star.SetActive(false); // Oculta la estrella si no usas favoritos
    }
}
