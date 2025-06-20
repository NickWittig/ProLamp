using System;
using UnityEngine;
using UnityEngine.Assertions;

public class RenderCaptureCamera : MonoBehaviour
{
    void Start()
    {
        SessionManager.Instance.OnServerScenesLoaded += OnScenesLoaded_AddCameraProvider;
    }

    private void OnScenesLoaded_AddCameraProvider()
    {
        Assert.IsNotNull(TextureDistributorSingleton.Instance);
        TextureDistributorSingleton.Instance.
            Build(TextureType.Camera, GetComponent<Camera>());
    }
}
