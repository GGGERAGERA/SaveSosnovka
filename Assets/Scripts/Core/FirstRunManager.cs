using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FirstRunManager : MonoBehaviour
{
    // 🎯 ОСНОВНЫЕ ПАНЕЛИ (СЕРИАЛИЗОВАНЫ)
    public GameObject languagePanel;          // Выбор языка
    public GameObject namePanel;              // Ввод имени
    public GameObject confirmationPanel;      // Подтверждение (после ввода имени)
    public GameObject topMenuPanel;           // Верхнее меню (Settings/Exit) - появляется после выбора языка

    // 📝 ТЕКСТОВЫЕ ЭЛЕМЕНТЫ (СЕРИАЛИЗОВАНЫ)
    public Text nameLabel;                    // Надпись "Enter your name"
    public Text confirmText;                  // Надпись "Ready to start?"
    public Text nameTakenText;                // Надпись "Unfortunately, this name is already taken..."
    public Text goodNameText;                 // Надпись "Great name!"

    // 🖊️ INPUT FIELD (СЕРИАЛИЗОВАН)
    public InputField nameInput;              // Поле для ввода имени

    // 🏆 АЧИВКА (СЕРИАЛИЗОВАНЫ)
    public GameObject achievementPopup;       // Окно ачивки
    public Text achievementText;              // Текст ачивки

    // 🔄 СТАТУСЫ (ДЛЯ ЛОГИКИ)
    private string currentLanguage = "en";    // Текущий язык (временный до сохранения)

    // ⏱️ ЗАДЕРЖКА АЧИВКИ (ПУБЛИЧНАЯ НАСТРОЙКА)
    public float achievementDelay = 3f;       // Задержка перед исчезновением ачивки

    // 🎨 АНИМАЦИИ (ПУБЛИЧНЫЕ НАСТРОЙКИ)
    public float fadeDuration = 0.3f;         // Длительность анимации появления/исчезновения панелей

    // 🔁 ПАНЕЛИ ПОДТВЕРЖДЕНИЯ (СЕРИАЛИЗОВАНЫ И НАГЛЯДНЫ)
    public GameObject chooseLanguageReplayPanel; // Панель подтверждения выбора языка
    public GameObject exitReplayPanel;          // Панель подтверждения выхода
    public GameObject optionsPanel;             // Панель настроек
    public GameObject saveOptionsReplayPanel;   // Панель подтверждения сохранения настроек
    public GameObject rejectChangesOptionsReplayPanel; // Панель подтверждения отмены изменений

    void Start()
    {
        // При первом запуске — показываем выбор языка
        if (PlayerPrefs.GetInt("FirstRunCompleted", 0) == 0)
        {
            ShowLanguagePanel();
        }
        else
        {
            // Если уже прошли первый запуск — загружаем последнюю сцену
            LoadNextScene();
        }
    }

    // 🌐 МЕТОДЫ ДЛЯ ПОКАЗА ПАНЕЛЕЙ (СЕРИАЛИЗОВАНЫ И НАГЛЯДНЫ)

    public void ShowLanguagePanel()
    {
        StartCoroutine(FadeInPanel(languagePanel));
        StartCoroutine(FadeOutPanel(namePanel));
        StartCoroutine(FadeOutPanel(confirmationPanel));
        StartCoroutine(FadeOutPanel(topMenuPanel)); // На экране выбора языка — только Exit

        // Сбрасываем имя и язык
        nameInput.text = "";
        currentLanguage = "en"; // дефолт
    }

    public void ShowNamePanel()
    {
        StartCoroutine(FadeOutPanel(languagePanel));
        StartCoroutine(FadeInPanel(namePanel));
        StartCoroutine(FadeOutPanel(confirmationPanel));
        StartCoroutine(FadeInPanel(topMenuPanel)); // Теперь видны Settings и Exit

        // Обновляем надпись на выбранном языке
        nameLabel.text = LanguageManager.Instance.Get("enter_name");
        nameTakenText.gameObject.SetActive(false); // Скрываем сообщение об ошибке
        goodNameText.gameObject.SetActive(false);  // Скрываем сообщение об успехе
    }

    public void ShowConfirmationPanel()
    {
        StartCoroutine(FadeOutPanel(namePanel));
        StartCoroutine(FadeInPanel(confirmationPanel));
        StartCoroutine(FadeInPanel(topMenuPanel));

        confirmText.text = LanguageManager.Instance.Get("ready_to_start");
    }

    // 🗣️ МЕТОДЫ ДЛЯ КНОПОК (СЕРИАЛИЗОВANЫ И НАГЛЯДНЫ)

    public void OnLanguageSelected(string langCode)
    {
        currentLanguage = langCode;
        LanguageManager.Instance.SetLanguageTemporarily(langCode); // Временно меняем язык
        ShowChooseLanguageReplayPanel(); // Показываем подтверждение выбора языка
    }

    public void OnNameSubmit()
    {
        string name = nameInput.text.Trim();

        if (string.IsNullOrEmpty(name) || name.Length < 2)
        {
            Debug.Log("Имя слишком короткое!");
            return;
        }

        // 🔜 ПОЗЖЕ: проверка в Firebase
        // Сейчас: эмуляция — имя не может повторяться
        if (PlayerPrefs.HasKey("UsedName_" + name))
        {
            nameTakenText.gameObject.SetActive(true);
            goodNameText.gameObject.SetActive(false);
            return;
        }

        // Сохраняем имя и финализируем язык
        PlayerPrefs.SetString("PlayerName", name);
        PlayerPrefs.SetInt("UsedName_" + name, 1);
        PlayerPrefs.SetInt("FirstRunCompleted", 1);
        PlayerPrefs.SetString("LastCompletedScene", "Cutscene1"); // Только после полного прохождения!
        PlayerPrefs.Save();

        LanguageManager.Instance.FinalizeLanguage(currentLanguage); // Сохраняем язык навсегда

        // Показываем ачивку
        ShowAchievement("first_run_achievement");

        // Показываем подтверждение
        ShowConfirmationPanel();
    }

    public void OnStartGame()
    {
        LoadNextScene();
    }

    public void OnExit()
    {
        ShowExitReplayPanel(); // Показываем подтверждение выхода
    }

    public void OnSettings()
    {
        ShowOptionsPanel(); // Показываем окно настроек
    }

    public void OnBackToLanguage()
    {
        ShowLanguagePanel();
    }

    // 🎁 МЕТОДЫ ДЛЯ АЧИВОК (СЕРИАЛИЗОВАНЫ И НАГЛЯДНЫ)

    void ShowAchievement(string key)
    {
        achievementText.text = LanguageManager.Instance.Get(key);
        StartCoroutine(FadeInPanel(achievementPopup));
        StartCoroutine(HideAchievementAfterDelay(achievementDelay));
    }

    IEnumerator HideAchievementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(FadeOutPanel(achievementPopup));
    }

    // 🚀 МЕТОД ЗАГРУЗКИ СЛЕДУЮЩЕЙ СЦЕНЫ

    void LoadNextScene()
    {
        SceneManager.LoadScene("LoadingScene1");
    }

    // 🔄 МЕТОДЫ ДЛЯ ПОДТВЕРЖДЕНИЙ (СЕРИАЛИЗОВАНЫ И НАГЛЯДНЫ)

    public void ShowChooseLanguageReplayPanel()
    {
        StartCoroutine(FadeInPanel(chooseLanguageReplayPanel));
    }

    public void OnConfirmLanguage()
    {
        StartCoroutine(FadeOutPanel(chooseLanguageReplayPanel));
        ShowNamePanel();
    }

    public void OnCancelLanguage()
    {
        StartCoroutine(FadeOutPanel(chooseLanguageReplayPanel));
        ShowLanguagePanel();
    }

    public void ShowExitReplayPanel()
    {
        StartCoroutine(FadeInPanel(exitReplayPanel));
    }

    public void OnConfirmExit()
    {
        Application.Quit();
    }

    public void OnCancelExit()
    {
        StartCoroutine(FadeOutPanel(exitReplayPanel));
    }

    public void ShowOptionsPanel()
    {
        StartCoroutine(FadeInPanel(optionsPanel));
    }

    public void OnSaveOptions()
    {
        ShowSaveOptionsReplayPanel();
    }

    public void OnRejectOptions()
    {
        ShowRejectChangesOptionsReplayPanel();
    }

    public void OnBackFromOptions()
    {
        StartCoroutine(FadeOutPanel(optionsPanel));
    }

    public void ShowSaveOptionsReplayPanel()
    {
        StartCoroutine(FadeInPanel(saveOptionsReplayPanel));
    }

    public void OnConfirmSaveOptions()
    {
        StartCoroutine(FadeOutPanel(saveOptionsReplayPanel));
        StartCoroutine(FadeOutPanel(optionsPanel));
    }

    public void OnCancelSaveOptions()
    {
        StartCoroutine(FadeOutPanel(saveOptionsReplayPanel));
    }

    public void ShowRejectChangesOptionsReplayPanel()
    {
        StartCoroutine(FadeInPanel(rejectChangesOptionsReplayPanel));
    }

    public void OnConfirmRejectChanges()
    {
        StartCoroutine(FadeOutPanel(rejectChangesOptionsReplayPanel));
        StartCoroutine(FadeOutPanel(optionsPanel));
    }

    public void OnCancelRejectChanges()
    {
        StartCoroutine(FadeOutPanel(rejectChangesOptionsReplayPanel));
    }

    // 🎨 МЕТОДЫ ДЛЯ АНИМАЦИЙ (СЕРИАЛИЗОВАНЫ И НАГЛЯДНЫ)

    IEnumerator FadeInPanel(GameObject panel)
    {
        if (panel == null) yield break;

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>();

        panel.SetActive(true);
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    IEnumerator FadeOutPanel(GameObject panel)
    {
        if (panel == null) yield break;

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        panel.SetActive(false);
    }
}