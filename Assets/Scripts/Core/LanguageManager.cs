using UnityEngine;
using System.Collections.Generic;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;
    private Dictionary<string, string> currentTexts = new Dictionary<string, string>();
    public string currentLanguage = "en";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // НЕ загружаем язык из PlayerPrefs до завершения регистрации!
        currentLanguage = "en"; // временно
    }

    public void SetLanguageTemporarily(string langCode)
    {
        // Временный выбор языка (до ввода имени)
        currentLanguage = langCode;
        LoadLocalization(langCode);
        OnLanguageChanged?.Invoke();
    }

    public void FinalizeLanguage(string langCode)
    {
        // Только после ввода имени — сохраняем навсегда
        currentLanguage = langCode;
        PlayerPrefs.SetString("GameLanguage", langCode);
        PlayerPrefs.Save();
        LoadLocalization(langCode);
        OnLanguageChanged?.Invoke();
    }

    void LoadLocalization(string langCode)
    {
        TextAsset asset = Resources.Load<TextAsset>($"Localization/{langCode}");
        if (asset == null)
        {
            Debug.LogError($"Localization file not found: {langCode}");
            return;
        }

        // Парсим JSON в Dictionary<string, string>
        currentTexts = SimpleJSON.Parse(asset.text);
    }

    public string Get(string key)
    {
        if (currentTexts.TryGetValue(key, out string value))
            return value;
        return $"[{key}]";
    }

    public static System.Action OnLanguageChanged;
}