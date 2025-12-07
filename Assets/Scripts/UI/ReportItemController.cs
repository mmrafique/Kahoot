using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReportItemController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI filenameText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private Button viewButton;

    private string reportPath;

    private void OnEnable()
    {
        if (viewButton != null)
        {
            viewButton.onClick.AddListener(OnViewClicked);
        }
    }

    private void OnDisable()
    {
        if (viewButton != null)
        {
            viewButton.onClick.RemoveListener(OnViewClicked);
        }
    }

    public void SetData(string filename, string date, string path)
    {
        filenameText.text = filename;
        dateText.text = date;
        reportPath = path;
    }

    private void OnViewClicked()
    {
        PlayerPrefs.SetString("CurrentReportPath", reportPath);
        PlayerPrefs.Save();
        SceneManager.LoadScene("ReportViewer");
    }
}
