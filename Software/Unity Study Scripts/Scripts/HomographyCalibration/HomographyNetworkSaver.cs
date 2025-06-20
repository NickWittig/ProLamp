using OpenCVForUnity.CoreModule;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class HomographyNetworkSaver : NetworkSingleton<HomographyNetworkSaver>
{

    private void Awake()
    {
        base.Awake();
    }

    [ClientRpc]
    public void SaveDataClientRPC(string contents)
    {
        File.WriteAllText(Application.persistentDataPath + "/homography.json", contents);
    }
}
