using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public GameObject loadingText; // Text (Legacy) или TextMeshPro
    public GameObject loadingSlider; // Slider

    private void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        // 1. Проверяем: первый ли запуск?
        bool isFirstRun = PlayerPrefs.GetInt("FirstRunCompleted", 0) == 0;

        string sceneToLoad = "SettingsScene1"; // По умолчанию — настройки

        if (!isFirstRun)
        {
            // Если не первый запуск — грузим последнюю сцену
            string lastScene = PlayerPrefs.GetString("LastCompletedScene", "Cutscene1");
            sceneToLoad = lastScene;
        }

        // 2. Загружаем сцену асинхронно
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // 90% прогресса — до активации
            if (loadingSlider != null)
                loadingSlider.GetComponent<Slider>().value = progress;

            if (progress >= 0.9f)
            {
                if (loadingText != null)
                    loadingText.GetComponent<Text>().text = "Готово!";

                // Ждём нажатия любой клавиши/тапа
                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}