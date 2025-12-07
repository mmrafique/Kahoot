using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButtonController : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "MainMenu";
    private Button backButton;

    void Awake()
    {
        backButton = GetComponent<Button>();
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    private void OnBackClicked()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
