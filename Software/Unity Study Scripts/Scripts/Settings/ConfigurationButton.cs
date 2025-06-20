using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ConfigurationButton : MonoBehaviour
{
    [SerializeField] private readonly bool canAutoConfigure = false;
    public StudyConfiguration key;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (canAutoConfigure) AddListener();
    }

    public void AddListener()
    {
        button.onClick.AddListener(delegate
        {
            ModuleManager.SetModulesToLoad(key);
            ModuleManager.Instance.ShowLoadingScreen();
            if (!SessionManager.Instance.TryConnect()) return;
            if (SessionManager.GetRole() == Role.Student)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect_LoadModules;
                StartCoroutine(ConnectionTimeout(2f));
                return;
            }
            ModuleManager.Instance.LoadModules();
        });
    }

    private IEnumerator ConnectionTimeout(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (NetworkManager.Singleton.IsConnectedClient) yield break;
        SessionManager.Instance.TriggerConnectionAttemptFailed();
        SessionManager.Instance.Disconnect();
    }

    private void OnClientConnect_LoadModules(ulong obj)
    {
        if (NetworkManager.Singleton.IsConnectedClient) ModuleManager.Instance.LoadModules();
    }
}