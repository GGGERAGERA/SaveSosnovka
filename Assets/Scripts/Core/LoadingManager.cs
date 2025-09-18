using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public GameObject loadingText;
    public GameObject loadingSlider;

    private void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("LanguageScene");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.GetComponent<Slider>().value = progress;

            if (progress >= 0.9f)
            {
                loadingText.GetComponent<UnityEngine.UI.Text>().text = "Готово!";
                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}