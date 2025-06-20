
using UnityEngine;

public class RenderStreamTextureProvider : ITextureProvider
{

    private Texture renderStreamingTexture;

    public Texture GetTexture()
    {
        return renderStreamingTexture;
    }

    public void SetTexture(Texture texture)
    {
        renderStreamingTexture = texture;
        SetTextureResolution(texture.width, texture.height);

        Debug.Log("render stream texture reso");
        Debug.Log(texture.width);
        Debug.Log(texture.height);
    }

    private void SetTextureResolution(int width, int height)
    {
        PlayerPrefs.SetInt("width", width);
        PlayerPrefs.SetInt("height", height);
    }


}