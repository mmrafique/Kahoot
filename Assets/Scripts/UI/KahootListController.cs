using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class KahootListController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField searchInput;
    public Button refreshButton;
    public Transform contentParent; // ScrollView Content
    public GameObject kahootListItemPrefab;

    private GameObject prefabTemplate;

    private List<Kahoot> kahoots = new List<Kahoot>();

    void Awake()
    {
        prefabTemplate = kahootListItemPrefab;
        // Make sure we don't accidentally render the template if it lives in the hierarchy
        if (prefabTemplate != null && prefabTemplate.scene.IsValid())
        {
            prefabTemplate.SetActive(false);
        }
    }

    void Start()
    {
        if (!ValidateBindings())
        {
            enabled = false;
            return;
        }

        refreshButton.onClick.AddListener(LoadKahoots);
        searchInput.onValueChanged.AddListener(OnSearchChanged);
        LoadKahoots();
    }

    private bool ValidateBindings()
    {
        bool ok = true;

        if (searchInput == null)
        {
            Debug.LogError("KahootListController: searchInput is not assigned.");
            ok = false;
        }

        if (refreshButton == null)
        {
            Debug.LogError("KahootListController: refreshButton is not assigned.");
            ok = false;
        }

        if (contentParent == null)
        {
            Debug.LogError("KahootListController: contentParent is not assigned.");
            ok = false;
        }

        if (kahootListItemPrefab == null)
        {
            Debug.LogError("KahootListController: kahootListItemPrefab is not assigned.");
            ok = false;
        }

        return ok;
    }

    void LoadKahoots()
    {
        kahoots = new List<Kahoot>();
        foreach (var path in KahootLoader.GetAllKahootFilePaths())
        {
            try
            {
                var k = KahootLoader.LoadFromFile(path);
                kahoots.Add(k);
            }
            catch { /* Ya loguea el error internamente */ }
        }
        ShowKahoots(kahoots);
    }

    void ShowKahoots(List<Kahoot> list)
    {
        if (!ValidateBindings())
            return;

        foreach (Transform child in contentParent)
        {
            if (kahootListItemPrefab != null && child.gameObject == kahootListItemPrefab)
                continue; // keep the template alive
            Destroy(child.gameObject);
        }

        var prefabToUse = prefabTemplate != null ? prefabTemplate : kahootListItemPrefab;
        if (prefabToUse == null)
        {
            Debug.LogError("Kahoot list item prefab is missing; cannot render list.");
            return;
        }

        foreach (var kahoot in list)
        {
            var itemGO = Instantiate(prefabToUse, contentParent);
            itemGO.SetActive(true);
            var item = itemGO.GetComponent<KahootListItemController>();
            item.Setup(kahoot, () => OnPlayKahoot(kahoot));
        }
    }

    void OnSearchChanged(string text)
    {
        var filtered = kahoots.FindAll(k =>
            (k.title != null && k.title.ToLower().Contains(text.ToLower())) ||
            (k.description != null && k.description.ToLower().Contains(text.ToLower()))
        );
        ShowKahoots(filtered);
    }

    void OnPlayKahoot(Kahoot kahoot)
    {
        // Guarda el ID y navega a la escena de juego
        PlayerPrefs.SetString("CurrentKahootId", kahoot.id);
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
