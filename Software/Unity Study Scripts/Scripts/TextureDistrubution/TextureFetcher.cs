using System;
using UnityEngine;

public class TextureFetcher : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    [SerializeField] private TextureType textureType = TextureType.Webcam;
    [SerializeField] private bool applyHomography = false;
    [SerializeField] private bool applyUndistort = false;

    private void Awake()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (TextureDistributorSingleton.Instance == null) return;

        var texture2D = textureType switch
        {
            TextureType.Webcam => TextureDistributorSingleton.Instance
            .GetDistributor(TextureType.Webcam)
            ?.GetTexture2D(applyHomography, applyUndistort),
            TextureType.RenderStream => TextureDistributorSingleton.Instance
            .GetDistributor(TextureType.RenderStream)
            ?.GetTexture2D(applyHomography, applyUndistort),
            TextureType.Camera => TextureDistributorSingleton.Instance
            .GetDistributor(TextureType.Camera)
            ?.GetTexture2D(applyHomography, applyUndistort),
            _ => throw new NotImplementedException()
        };

        meshRenderer.material.mainTexture = texture2D;
    }
}