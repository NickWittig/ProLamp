using OpenCVForUnity.UnityUtils;
using System;
using UnityEngine;

public class TextureDistributor
{
    private ITextureProvider textureProvider;
    private Texture2D texture2D;

    private TextureDistributor(ITextureProvider textureProvider)
    {
        this.textureProvider = textureProvider;
    }

    internal static TextureDistributor Create(TextureType textureType, Camera camera = null)
    {
        TextureDistributor textureDistributor = textureType switch
        {
            TextureType.Webcam => new TextureDistributor(new WebcamTextureProvider()),
            TextureType.RenderStream => new TextureDistributor(new RenderStreamTextureProvider()),
            TextureType.Camera => new TextureDistributor(new CameraTextureProvider(camera)),
            _ => null,
        };
        return textureDistributor;
    }


    public Texture GetTexture()
    {
        return textureProvider?.GetTexture();
    }

    public Texture2D GetTexture2D(bool homography = false, bool undistorted = false)
    {
        var texture = textureProvider?.GetTexture();
        if (texture == null) return null;
        if (texture2D == null || !texture2D.texelSize.Equals(texture.texelSize))
        {
            texture2D = new Texture2D(
                texture.width,
                texture.height,
                TextureFormat.RGBA32,
                false);
        }

        Utils.textureToTexture2D(texture, texture2D);
        if (undistorted) Undistorter.Instance.Apply(ref texture2D);
        if (homography) HomographyManager.Instance.homographyTransformer.Apply(ref texture2D);
        return texture2D;
    }

   public ITextureProvider GetTextureProvider()
    {
        return textureProvider;
    }


}