using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LanguageManager : MonoBehaviour
{
    public Dropdown languageDropdown;
    public InputField playerNameInput;
    public Button okButton;

    private void Start()
    {
        // Загрузка сохранённого имени
        if (PlayerPrefs.HasKey("PlayerName"))
            playerNameInput.text = PlayerPrefs.GetString("PlayerName");
    }

    public void OnOkClick()
    {
        string name = playerNameInput.text.Trim();
        if (string.IsNullOrEmpty(name))
            name = "Ivan";

        PlayerPrefs.SetString("PlayerName", name);
        PlayerPrefs.Save();

        // Переход на следующую сцену
        SceneManager.LoadScene("HQScene");
    }
}