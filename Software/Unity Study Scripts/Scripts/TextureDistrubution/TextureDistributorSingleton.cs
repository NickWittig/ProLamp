using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class TextureDistributorSingleton : SingletonBehaviour<TextureDistributorSingleton>
{

    private Dictionary<TextureType, TextureDistributor> distributorDict = new();

    private void Awake()
    {
        base.Awake();
        Build(TextureType.Webcam);
        Build(TextureType.RenderStream);
    }


    public void Build(TextureType textureType, object value = null)
    {
        switch (textureType)
        {
            case TextureType.Webcam:
                distributorDict.Add(textureType, TextureDistributor.Create(TextureType.Webcam));
                break;
            case TextureType.RenderStream:
                distributorDict.Add(textureType, TextureDistributor.Create(TextureType.RenderStream));
                break;
            case TextureType.Camera:
                var cam = value as Camera;
                distributorDict.Add(textureType, TextureDistributor.Create(TextureType.Camera, cam));
                break;
            default:
                throw new System.Exception("Use Non Generic Build instead.");
        }
    }

    public TextureDistributor GetDistributor(TextureType textureType)
    {

        distributorDict.TryGetValue(textureType, out var textureDistributor);
        return textureDistributor;
    }


}