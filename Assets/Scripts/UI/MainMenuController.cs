using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField Input_Username;
    public Button Button_Play;
    public Button Button_Leaderboards;
    public Button Button_Reports;
    public Button Button_create;
    public Button Button_About;
    public Button Button_Quit;


    private PlayerInput playerInput;
    private InputAction submitAction;

	private void Start()
	{
		// Asignar listeners a los botones
		Button_Play.onClick.AddListener(OnPlay);
		Button_Leaderboards.onClick.AddListener(OnLeaderboards);
		Button_Reports.onClick.AddListener(OnReports);
		Button_create.onClick.AddListener(OnCreate);
		Button_About.onClick.AddListener(OnAbout);
		Button_Quit.onClick.AddListener(OnQuit);        // Desactivar Play si el campo está vacío
        Button_Play.interactable = !string.IsNullOrWhiteSpace(Input_Username.text);
        Input_Username.onValueChanged.AddListener(OnUsernameChanged);

        // Configurar InputSystem para Submit en el campo de username
        SetupInputSystem();

        // Crear carpetas requeridas del proyecto
        KahootLoader.EnsureFolders();
        var _ = LeaderboardManager.Folder;
        var __ = ExceptionLogger.ReportsFolder;
    }

    private void SetupInputSystem()
    {
        // Buscar el PlayerInput en la escena o en el prefab
        playerInput = FindObjectOfType<PlayerInput>();

        if (playerInput != null && playerInput.actions != null)
        {
            submitAction = playerInput.actions.FindAction("Submit");
            if (submitAction != null)
            {
                submitAction.performed += OnSubmitInput;
            }
        }
    }

    private void OnSubmitInput(InputAction.CallbackContext context)
    {
        // Solo ejecutar si el InputField está activo (tiene el foco)
        if (Input_Username.isFocused && !string.IsNullOrWhiteSpace(Input_Username.text))
        {
            OnPlay();
        }
    }

    private void OnUsernameChanged(string value)
    {
        Button_Play.interactable = !string.IsNullOrWhiteSpace(value);
    }

    private void OnPlay()
    {
        if (string.IsNullOrWhiteSpace(Input_Username.text))
        {
            // No debería ocurrir, el botón estará desactivado
            return;
        }
        PlayerProfile.Username = Input_Username.text.Trim();
        SceneManager.LoadScene("KahootList");
    }

    private void OnLeaderboards()
    {
        SceneManager.LoadScene("Leaderboards");
    }

	private void OnReports()
	{
		SceneManager.LoadScene("Reports");
	}

	private void OnCreate()
	{
		SceneManager.LoadScene("KahootCreator");
	}

	private void OnAbout()
	{
		SceneManager.LoadScene("About");
	}    private void OnQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        // Desuscribirse del evento cuando se destruya el GameObject
        if (submitAction != null)
        {
            submitAction.performed -= OnSubmitInput;
        }
    }
}

