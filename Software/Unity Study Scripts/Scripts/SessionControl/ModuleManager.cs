using System;
using System.Collections.Generic;
using Assets.Scripts.AppControl;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModuleManager : SingletonBehaviour<ModuleManager>
{
    private const string MODULE_LOAD_STRING = "load";
    private const string MODULE_UNLOAD_STRING = "unload";

    private List<Module> configuredModules;
    private Queue<Module> modulesToLoad = new();

    private void Awake()
    {
        base.Awake();
    }


    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnServerShutdown_LoadMainMenu;
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

    }

    private void OnServerShutdown_LoadMainMenu(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer) return;
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }

    internal void SwitchSceneFromTo(int from, int to)
    {
        UnloadSceneOfBuildIndex(from);
        SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);

    }


    internal void ShowLoadingScreen()
    {
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }


    internal void LoadMainMenu()
    {
        SessionManager.Instance.Disconnect();
        ShowLoadingScreen();
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            UnloadSceneOfBuildIndex(i);
        }
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }



    private static bool ShouldLoadModule(Module module)
    {
        return PlayerPrefs.GetString(module.ToString(), MODULE_UNLOAD_STRING) == MODULE_LOAD_STRING;
    }


    private void SetConfiguredModules()
    {
        configuredModules = new List<Module>();
        configuredModules.Add(Module.Default);
        configuredModules.Add(Module.Loading);
        foreach (Module module in Enum.GetValues(typeof(Module)))
        {
            if (!ShouldLoadModule(module)) continue;
            Enum.TryParse(module.ToString(), out Module moduleToAdd);
            configuredModules.Add(moduleToAdd);
        }
    }



    public void AddModule(Module module)
    {
        PlayerPrefs.SetString(module.ToString(), MODULE_LOAD_STRING);
    }

    public void AddModules(List<Module> modules)
    {
        foreach (var module in modules)
            AddModule(module);
    }

    public void UnloadAllModules()
    {
        foreach (Module module in Enum.GetValues(typeof(Module)))
            PlayerPrefs.DeleteKey(module.ToString());
    }


    public void LoadModules()
    {
        SceneManager.sceneUnloaded += OnMainMenuUnload_LoadModules; 
        UnloadSceneOfBuildIndex(1);
     
    }

    private void OnMainMenuUnload_LoadModules(Scene arg0)
    {
        SceneManager.sceneUnloaded -= OnMainMenuUnload_LoadModules;
        LoadScenes();
        UnloadAllModules();
        configuredModules.Clear();
    }

    private static void UnloadSceneOfBuildIndex(int index)
    {
        var scene = SceneManager.GetSceneByBuildIndex(index);
        if (scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(scene);
        }
    }

    public List<Module> GetModules()
    {
        SetConfiguredModules();
        return configuredModules;
    }

    private void OnDestroy()
    {
        UnloadAllModules();
    }


    private void LoadScenes()
    {

        foreach (var module in GetModules()) modulesToLoad.Enqueue(module);
        NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
        if (NetworkManager.Singleton.IsServer) LoadNextScene();
    }

    public void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.LoadEventCompleted:
                if (NetworkManager.Singleton.IsServer) LoadNextScene();
                else Debug.Log("scenes loaded client");
                break;
            case SceneEventType.SynchronizeComplete:
                if (NetworkManager.Singleton.LocalClientId == sceneEvent.ClientId)
                {
                    SessionManager.Instance.TriggerScenesLoaded();
                }
                break;
        }
    }

    private void LoadNextScene()
    {
        if (modulesToLoad.Count == 0)
        {
            SessionManager.Instance.TriggerScenesLoaded();
            return;
        }
        var sceneName = $"{modulesToLoad.Dequeue()}Scene";
        Debug.Log($"Loading Scene: {sceneName}");
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }



    public static void SetModulesToLoad(StudyConfiguration configuration)
    {
        List<Module> modulesToLoad = new();
        switch (configuration)
        {
            case StudyConfiguration.LearnAtHome:
                modulesToLoad.AddRange(new List<Module> { Module.TextureDistribution, Module.RenderStreaming, Module.Drawing });
                break;
            case StudyConfiguration.WizardOfDevices:
                modulesToLoad.AddRange(new List<Module>
                    { Module.TextureDistribution,  Module.MarkerTracking, Module.WizardOfOz });
                break;
            case StudyConfiguration.Calibration:
                modulesToLoad.AddRange(new List<Module> {  Module.RenderStreaming, Module.TextureDistribution,  Module.HomographyCalibration });
                break;
        }

        Instance.UnloadAllModules();
        Instance.AddModules(modulesToLoad);
    }
}