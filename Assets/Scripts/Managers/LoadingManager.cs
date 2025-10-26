using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    // --- ПУБЛИЧНЫЕ ПЕРЕМЕННЫЕ ---
    [Header("UI Elements")]
    public Slider loadingSlider; // Слайдер

    [Header("Loading Settings")]
    [Tooltip("Минимальное время загрузки (в секундах), чтобы анимация всегда была видна")]
    public float minLoadTime = 2f;

    // --- ПРИВАТНЫЕ ПЕРЕМЕННЫЕ ---
    private AsyncOperation asyncOperation;
    private int nextSceneIndex; // Индекс следующей сцены

    void Start()
    {
        LoadNextScene();
    }

    /// <summary>
    /// Загружаем следующую сцену
    /// </summary>
    private void LoadNextScene()
    {
        // Получаем сохранённый индекс сцены
        int savedIndex = PlayerPrefs.GetInt("LastSceneIndex", -1);

        // Если индекс некорректный — грузим первую сцену (SettingsScene1)
        if (savedIndex < 0 || savedIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("Сохранённая сцена не найдена. Загружаем SettingsScene1 (индекс 1).");
            nextSceneIndex = 1; // Предполагаем, что SettingsScene1 — вторая в Build Settings (индекс 1)
        }
        else
        {
            nextSceneIndex = savedIndex;
        }

        Debug.Log($"Загружаем сцену с индексом: {nextSceneIndex}");

        StartCoroutine(LoadSceneCoroutine());
    }

    /// <summary>
    /// Корутина загрузки сцены с плавным слайдером
    /// </summary>
    IEnumerator LoadSceneCoroutine()
    {
        asyncOperation = SceneManager.LoadSceneAsync(nextSceneIndex);
        asyncOperation.allowSceneActivation = false;

        float elapsedTime = 0f;

        while (elapsedTime < minLoadTime || asyncOperation.progress < 0.9f)
        {
            float progress = asyncOperation.progress;

            // Показываем прогресс до 95% при минимальном времени
            if (elapsedTime < minLoadTime)
            {
                loadingSlider.value = Mathf.Lerp(0f, 0.95f, elapsedTime / minLoadTime);
            }
            else
            {
                // После minLoadTime — показываем реальный прогресс, но не ниже 95%
                if (progress >= 0.9f)
                    loadingSlider.value = Mathf.Lerp(0.95f, 1f, (progress - 0.9f) / 0.1f);
                else
                    loadingSlider.value = 0.95f;
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Активируем сцену
        asyncOperation.allowSceneActivation = true;

        Debug.Log("Сцена загружена!");
    }

    /// <summary>
    /// Сохраняем индекс сцены в PlayerPrefs
    /// </summary>
    public static void SaveSceneIndex(int index)
    {
        PlayerPrefs.SetInt("LastSceneIndex", index);
        PlayerPrefs.Save();
        Debug.Log($"Сохранён индекс сцены: {index}");
    }
}
