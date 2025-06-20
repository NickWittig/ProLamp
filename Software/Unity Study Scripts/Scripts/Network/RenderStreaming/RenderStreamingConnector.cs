using System.Collections;
using Assets;
using Unity.RenderStreaming;
using UnityEngine;
using UnityEngine.SceneManagement;

internal class RenderStreamingConnector : MonoBehaviour
{
    private const float CONNECTION_DELAY = 2f;
    private string connectionId;


    private string ip, port;
    private readonly bool IsConnecting = true;

    [SerializeField] private VideoStreamReceiver remoteVideoStreamReceiver;
    [SerializeField] private RenderStreaming renderStreaming;
    [SerializeField] private SingleConnection singleConnection;
    [SerializeField] private WebCamStreamSender webCamStreamer;


    private void Awake()
    {
        ip = PlayerPrefs.GetString("RenderStreamingIP", "192.168.0.0);
        port = PlayerPrefs.GetString("RenderStreamingPort", "80");
        connectionId = PlayerPrefs.GetString("ConnectionID", "12345");
        webCamStreamer.SetDeviceIndex(PlayerPrefs.GetInt("CameraIndex", 0));
        Debug.Log($"{ip}:{port}");
        webCamStreamer.OnStartedStream += id => remoteVideoStreamReceiver.enabled = true;

    }


    private void Start()
    {
        SetupVideoStreams();
        RenderStreamingSettings.SignalingAddress = $"{ip}:{port}";
        Debug.Log(RenderStreamingSettings.SignalingAddress);
        renderStreaming.Run(
            RenderStreamingSettings.EnableHWCodec,
            RenderStreamingSettings.Signaling);
        StartCoroutine(Connect());
    }

    private IEnumerator Connect()
    {
        yield return new WaitForSeconds(CONNECTION_DELAY);
        webCamStreamer.enabled = true;
        if (IsConnecting) singleConnection.CreateConnection(connectionId);
        Debug.Log(singleConnection.IsConnected(connectionId));

    }


    private void SetupVideoStreams()
    {
        remoteVideoStreamReceiver.OnUpdateReceiveTexture += texture =>
        {
            var provider = TextureDistributorSingleton.Instance
              .GetDistributor(TextureType.RenderStream)?.GetTextureProvider() as RenderStreamTextureProvider;
            provider.SetTexture(texture);        };

        webCamStreamer.OnUpdateWebCamTexture += texture =>
        {; 
        };
    }

    private void HangUp()
    {
        if (singleConnection != null)
        {
            if (singleConnection.IsConnected(connectionId)) singleConnection.DeleteConnection(connectionId);
        }
        renderStreaming.Stop();
    }

    private void OnDestroy()
    {
        HangUp();
    }

    private void OnApplicationQuit()
    {
        HangUp();
    }
}