using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSaver : MonoBehaviour
{
    private static SceneSaver instance;
    
    void Awake()
    {
        // Реализация Singleton - только один экземпляр на всю игру
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Подписываемся на события смены сцены
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            Debug.Log("SceneSaver инициализирован и сохранён между сценами");
        }
        else
        {
            // Если уже существует другой SceneSaver - уничтожаем этот
            Destroy(gameObject);
        }
    }
    
    void OnDestroy()
    {
        // Отписываемся от события при уничтожении
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Сохраняем каждую загруженную сцену, кроме самой LoadingScene1
        if (scene.name != "LoadingScene1")
        {
            SaveCurrentScene(scene.name);
        }
    }
    
    // Метод для сохранения текущей сцены
    private void SaveCurrentScene(string sceneName)
    {
        PlayerPrefs.SetString("LastActiveScene", sceneName);
        PlayerPrefs.Save();
        Debug.Log($"Автосохранение сцены: {sceneName}");
    }
    
    // Публичный метод для принудительного сохранения (на всякий случай)
    public void ForceSaveCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "LoadingScene1")
        {
            SaveCurrentScene(currentScene);
        }
    }
}