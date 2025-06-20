#if UNITY_EDITOR
using ParrelSync;
using System.IO;
#endif
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : NetworkSingleton<SessionManager>
{

    public delegate void OnServerScenesLoadedDelegate();
    public event OnServerScenesLoadedDelegate OnServerScenesLoaded;

    public delegate void OnServerShutdownDelegate();
    public event OnServerShutdownDelegate OnServerShutdown;

    public delegate void OnClientShutdownDelegate();
    public event OnClientShutdownDelegate OnClientShutdown;

    public delegate void OnConnectionAttemptFailedDelegate();
    public event OnConnectionAttemptFailedDelegate OnConnectionAttemptFailed;


    private void Awake()
    {
        base.Awake();
        Screen.SetResolution(1280, 800, GetRole() == Role.Student);
    }

    internal void TriggerConnectionAttemptFailed()
    {
        OnConnectionAttemptFailed?.Invoke();
    }

    internal void TriggerScenesLoaded()
    {
        OnServerScenesLoaded?.Invoke();
    }

    private void ShutdownServer()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        NetworkManager.Singleton.Shutdown();
        OnServerShutdown?.Invoke();
    }


    private bool StartClient()
    {

        var status = NetworkManager.Singleton.StartClient();

        if (status)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent += ModuleManager.Instance.SceneManager_OnSceneEvent;

        }
        return status;
    }

    private bool StartServer()
    {
        var status = NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
        NetworkManager.Singleton.SceneManager.VerifySceneBeforeLoading += VerifyScene;
        return status;
    }

    private bool VerifyScene(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (sceneIndex <= 1)
        {
            return false;
        }
        return true;
    }

    private void UpdateNetcodeIp()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address =
            PlayerPrefs.GetString("NetCodeIP", "127.0.0.1");
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port=
         ushort.Parse(PlayerPrefs.GetString("NetCodePort", "8427"));

    }

    public static Role GetRole()
    {
#if UNITY_EDITOR
        if (ClonesManager.IsClone()) return Role.Student;
#endif
        return PlayerPrefs.GetInt("Teacher", 1) == 1 ? Role.Teacher : Role.Student;
    }


    public bool TryConnect()
    {
        Debug.Log($"This is the teacher: {GetRole()}");
        if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer) return true;
        UpdateNetcodeIp();
        bool status;
        if (GetRole() == Role.Teacher) {
            status = StartServer();
        }
        else {
            status = StartClient();
        }

        return status;
    }

    public void Disconnect()
    {
        if (NetworkManager.Singleton == null)
        {
            return;
        }
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            ShutdownServer();
        }
        else if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
            OnClientShutdown?.Invoke();
        }
        return;
    }


    private void OnApplicationQuit()
    {
        Disconnect();
    }

}
