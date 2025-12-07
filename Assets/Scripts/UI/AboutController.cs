using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AboutController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button repoButton;
    [SerializeField] private TMP_Text versionText;
    [SerializeField] private TMP_Text bodyText;

    [Header("GitHub Repository")]
    [SerializeField] private string repositoryUrl = "https://github.com/tuusuario/kahoot";

    private void Start()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }

        if (repoButton != null && !string.IsNullOrEmpty(repositoryUrl))
        {
            repoButton.onClick.AddListener(OnRepoClicked);
        }

        // Opcional: establecer textos por código
        if (versionText != null)
        {
            versionText.text = "Kahoot++ v1.0";
        }
    }

    private void OnBackClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnRepoClicked()
    {
        Application.OpenURL(repositoryUrl);
    }
}
