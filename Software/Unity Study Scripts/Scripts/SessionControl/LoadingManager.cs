using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    private const float MAX_LOADING_TIME = 2.5f;
    private float currentTime = MAX_LOADING_TIME;

    [SerializeField] private TMP_Text infoText;

    private void Awake()
    {
        Assert.IsNotNull(SessionManager.Instance);
        Assert.IsNotNull(NetworkManager.Singleton);
        SessionManager.Instance.OnConnectionAttemptFailed += OnConnectionAttemptFailed_ShowError;
        SessionManager.Instance.OnServerScenesLoaded += OnScenesLoaded_UnloadScene;
        if (SessionManager.GetRole() == Role.Student && !NetworkManager.Singleton.IsServer)
        {
            currentTime = 6f;
            infoText.text = "Connecting...";
        }
    }
  

    private void OnConnectionAttemptFailed_ShowError()
    {
        infoText.text = "Connection failed. Trying to reconnect...";
        currentTime = 4f;
        SessionManager.Instance.OnConnectionAttemptFailed -= OnConnectionAttemptFailed_ShowError;
        return;
    }

    private void OnScenesLoaded_UnloadScene()
    {
        if (SceneManager.GetSceneByBuildIndex(2).isLoaded) SceneManager.UnloadSceneAsync(sceneBuildIndex: 2);
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;
        if (currentTime < 0)
        {
            if (SceneManager.GetSceneByBuildIndex(2).isLoaded) SceneManager.UnloadSceneAsync(sceneBuildIndex: 2);
            currentTime = MAX_LOADING_TIME;
        }
    }
}
