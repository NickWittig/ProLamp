using UnityEngine;
using UnityEngine.Assertions;

public class CameraTextureProvider : ITextureProvider
{
    private Camera captureCamera;

    private RenderTexture renderTexture;
    private Texture2D texture2D;
    public CameraTextureProvider(Camera camera)
    {
        captureCamera = camera;
        Assert.IsNotNull(captureCamera.targetTexture);
        renderTexture = captureCamera.targetTexture;
        texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
    }


    public Texture GetTexture()
    {
        RenderTextureToTexture2D(renderTexture, ref texture2D);
        return texture2D;
    }


    private Texture2D RenderTextureToTexture2D(RenderTexture renderTexture, ref Texture2D texture2D)
    {
        RenderTexture currentActiveRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // Read the pixel data from the RenderTexture
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        // Restore previously active RenderTexture
        RenderTexture.active = currentActiveRenderTexture;

        return texture2D;
    }

}
