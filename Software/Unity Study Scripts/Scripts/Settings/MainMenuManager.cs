using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject configurationButtonPrefab;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button setttingsButton;
    [SerializeField] private GameObject startPanel;

    private void Awake()
    {
        PlayerPrefs.SetInt("Android", 0);
#if UNITY_ANDROID && !UNITY_EDITOR
        PlayerPrefs.SetInt("Android", 1);
#endif
        foreach (StudyConfiguration value in Enum.GetValues(typeof(StudyConfiguration)))
        {
            if (value == StudyConfiguration.Default) continue;
            if (value == StudyConfiguration.LearnAtHome) continue;
            var buttonGo = Instantiate(configurationButtonPrefab, startPanel.transform);
            var config = buttonGo.GetComponent<ConfigurationButton>();
            config.key = value;
            config.AddListener();
            buttonGo.GetComponentInChildren<TMP_Text>().text = value.ToString();
        }

        setttingsButton.onClick.AddListener(() =>
        {
            ModuleManager.Instance.SwitchSceneFromTo(1, 3);
        });
        quitButton.onClick.AddListener(() => { Application.Quit(); });
    }
}