using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDPReceiver : MonoBehaviour
{
    [Header("UDP Settings")]
    [Tooltip("Port to listen on for UDP messages.")]
    public int port = 8051;

    public delegate void MessageReceivedHandler(string message);
    public event MessageReceivedHandler OnMessageReceived;

    private UdpClient udpClient;
    private Thread receiveThread;
    private bool isRunning = false;

    void Start()
    {
        // Initialize the UDP client and start the listener thread.
        udpClient = new UdpClient(port);
        isRunning = true;
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();

        Debug.Log("UDPReceiver started on port " + port);
    }

    private void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        while (isRunning)
        {
            try
            {
                // Block until a message is received.
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.ASCII.GetString(data);

                // Raise the event to notify subscribers.
                if (OnMessageReceived != null)
                {
                    OnMessageReceived(message);
                }
            }
            catch (SocketException se)
            {
                Debug.LogError("Socket exception: " + se.Message);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Exception in UDPReceiver: " + ex.ToString());
            }
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        if (udpClient != null)
            udpClient.Close();
        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Abort();
    }
}
