using UnityEngine;

public class WebcamTextureProvider : ITextureProvider
{
    private WebCamTexture webcamTexture;


    public WebcamTextureProvider()
    {
        var selectedWebcamName = WebCamTexture.devices[PlayerPrefs.GetInt("CameraIndex", 0)].name;
        Debug.Log($"using camera {selectedWebcamName}");
        webcamTexture = new WebCamTexture(selectedWebcamName);
        webcamTexture.Play();
        SetTextureResolution(webcamTexture.width, webcamTexture.height);
    }

    private void SetTextureResolution(int width, int height)
    {
        PlayerPrefs.SetInt("width", width);
        PlayerPrefs.SetInt("height", height);
    }

    public Texture GetTexture()
    {
        return webcamTexture;
    }


    ~WebcamTextureProvider()
    {
        if (webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }

        webcamTexture = null;
    }

}