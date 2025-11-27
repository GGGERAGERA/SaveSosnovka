using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstLaunchController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private PanelManagerSimple panelManager; // ссылка на твой PanelManagerSimple

    [Header("PlayerPrefs Keys")]
    private const string LANGUAGE_CHOSEN_KEY = "LanguageWasChosen";
    private const string NAME_CHOSEN_KEY = "NameWasChosen";

    public GameObject chooseLanguagePanel1;

    public GameObject nameEnterPanel1;

    public GameObject StartGamePanel1;

    void Start()
    {
        bool languageChosen = PlayerPrefs.GetInt(LANGUAGE_CHOSEN_KEY, 0) == 1;
        bool nameChosen = PlayerPrefs.GetInt(NAME_CHOSEN_KEY, 0) == 1;

        if (!languageChosen)
        {
            // Первый запуск — показываем выбор языка
            panelManager.ShowPanel(chooseLanguagePanel1);
        }
        else if (!nameChosen)
        {
            // Язык выбран, но имя нет — показываем ввод имени
            panelManager.ShowPanel(nameEnterPanel1);
        }
        else
        {
            panelManager.ShowPanel(StartGamePanel1);
        }
    }

    // Вызывается при нажатии "OK" в ChooseLanguageReplayPanel1
    public void OnLanguageConfirmed(string languageCode)
    {
        PlayerPrefs.SetString("Language", languageCode);
        PlayerPrefs.SetInt(LANGUAGE_CHOSEN_KEY, 1); // язык выбран
        PlayerPrefs.Save();

        // Переходим к вводу имени
        panelManager.ShowPanel(nameEnterPanel1);
    }

    // Вызывается при нажатии "OK" в NameApplyPanel1
    public void OnNameConfirmed(string playerName)
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetInt(NAME_CHOSEN_KEY, 1); // имя выбрано
        PlayerPrefs.Save();

        // Переходим к следующей панели
        panelManager.ShowPanel(StartGamePanel1);
    }
}