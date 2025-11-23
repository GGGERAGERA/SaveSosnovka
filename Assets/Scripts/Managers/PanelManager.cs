using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
//using UnityEngine.SceneManagement;

// Этот класс управляет отображением UI-панелей через CanvasGroup
public class PanelManager : MonoBehaviour
{
    // Перечисление всех панелей — читаемо и безопасно
    public enum PanelType
    {
        LovePanel1,
        ChooseLanguagePanel1,
        ChooseLanguageReplayPanel1,
        NameEnterPanel1,
        NameReplayPanel1,
        NameApplyPanel1,
        ExitReplayPanel1,
        OptionsPanel1,
        ChooseLanguagePanel2,
        ChooseLanguageReplayPanel2,
        CameraOptionsPanel1,
        GameplayOptionsPanel1,
        ResetAllProgressPanel1,
        ResetAllProgressReplayPanel1,
        SaveOptionsReplayPanel1,
        ResetOptionsReplayPanel1,
        RejectСhangesOptionsReplayPanel1,
        ShopPanel,
        ShopReplayPanel1,
        ShopApplyPanel1,
        ShopReplayPanel2,
        //SettingsPanel,
        //MainMenuPanel,
        // ... и так все 29 штук
        TotalCount // ← фиктивный элемент, чтобы знать, сколько всего панелей
    }

    // Массив панелей. В инспекторе просто перетащи в правильном порядке!
    public GameObject[] panelObjects;

    private CanvasGroup[] canvasGroups;

    void Awake()
    {
        // Проверим, не забыл ли ты что-то в инспекторе
        if (panelObjects.Length != (int)PanelType.TotalCount)
        {
            Debug.LogError($"PanelManager: ожидается {(int)PanelType.TotalCount} панелей, но в инспекторе {panelObjects.Length}");
            return;
        }

        // Кэшируем CanvasGroup для оптимизации
        canvasGroups = new CanvasGroup[panelObjects.Length];
        for (int i = 0; i < panelObjects.Length; i++)
        {
            canvasGroups[i] = panelObjects[i].GetComponent<CanvasGroup>();
            if (canvasGroups[i] == null)
                canvasGroups[i] = panelObjects[i].AddComponent<CanvasGroup>();

            // Сразу прячем все панели
            HidePanel(i);
        }
    }

    /// <summary>
    /// Показать панель по её типу
    /// </summary>
    public void ShowPanel(PanelType panel)
    {
        int index = (int)panel;
        if (index < 0 || index >= canvasGroups.Length) return;

        // Сначала прячем все
        for (int i = 0; i < canvasGroups.Length; i++)
            HidePanel(i);

        // Теперь показываем нужную
        var cg = canvasGroups[index];
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    void HidePanel(int index)
    {
        var cg = canvasGroups[index];
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
}