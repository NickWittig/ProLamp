using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// MarkerVisualizer listens for UDP marker messages and visualizes them in the scene.
/// 
/// The UDP message is expected to have the following format:
/// "N, center1_x, center1_y, orientation1, center2_x, center2_y, orientation2, �, centerN_x, centerN_y, orientationN"
/// where N is the number of markers.
/// 
/// The script instantiates marker prefabs dynamically and updates their positions and rotations.
/// If a marker is not updated for a specified number of frames (deletionDelayFrames),
/// it is removed. If a marker reappears before deletion, its position and rotation are lerped
/// from the last-known state to the new state.
/// </summary>
public class MarkerVisualizer : MonoBehaviour
{
    [Header("Marker Settings")]
    [Tooltip("Prefab for a marker center visualization.")]
    public GameObject markerPrefab;

    [Tooltip("Delay in frames before deleting a marker when it is no longer detected.")]
    public int deletionDelayFrames = 15;

    // A list of instantiated marker GameObjects.
    private List<GameObject> markerInstances = new List<GameObject>();
    // A parallel list tracking how many frames have passed since each marker was last updated.
    private List<int> markerAges = new List<int>();

    // Latest received marker information.
    private MarkerDataParser.MarkerInfo[] latestMarkers = null;

    void Start()
    {
        if (markerPrefab == null)
        {
            Debug.LogError("Please assign a marker prefab in the inspector.");
            return;
        }

        // Subscribe to the UDPReceiver's event (assumes UDPReceiver is attached to the same GameObject).
        UDPReceiver udpReceiver = GetComponent<UDPReceiver>();
        if (udpReceiver != null)
        {
            udpReceiver.OnMessageReceived += OnMarkerMessageReceived;
        }
        else
        {
            Debug.LogError("UDPReceiver component not found on the GameObject. Please attach UDPReceiver.");
        }
    }

    /// <summary>
    /// Called when a UDP message is received. Parses the message and stores the resulting marker info.
    /// </summary>
    /// <param name="message">The UDP message as a comma-separated string.</param>
    void OnMarkerMessageReceived(string message)
    {
        Debug.Log("Received UDP message: " + message);
        MarkerDataParser.MarkerInfo[] markers = MarkerDataParser.ParseMarkerMessage(message);
        if (markers != null)
        {
            Debug.Log("Parsed " + markers.Length + " markers from UDP message.");
            latestMarkers = markers;
        }
    }

    void Update()
    {
        // Only process if we received new marker data.
        if (latestMarkers != null)
        {
            int numDetected = latestMarkers.Length;
            int numInstances = markerInstances.Count;

            // Update positions and rotations for marker instances that correspond to detected markers.
            for (int i = 0; i < Mathf.Min(numDetected, numInstances); i++)
            {
                // Get current position and target position.
                Vector3 currentPos = markerInstances[i].transform.position;
                Vector3 targetPos = new Vector3(latestMarkers[i].Center.x, latestMarkers[i].Center.y, 0f);
                // If the marker was "aged" (i.e. not updated in the previous frame), interpolate (lerp).
                float lerpFactor = (markerAges[i] > 0) ? 0.2f : 1f; // 0.2 means quick interpolation.
                Vector3 newPos = Vector3.Lerp(currentPos, targetPos, lerpFactor);
                markerInstances[i].transform.position = newPos;

                // Update rotation similarly.
                float currentRot = markerInstances[i].transform.eulerAngles.z;
                float targetRot = latestMarkers[i].Orientation;
                float newRot = (markerAges[i] > 0) ? Mathf.LerpAngle(currentRot, targetRot, 0.2f) : targetRot;
                markerInstances[i].transform.eulerAngles = new Vector3(0f, 0f, newRot);

                // Reset the age for this marker.
                markerAges[i] = 0;
            }

            // If more markers are detected than are currently instantiated, instantiate new ones.
            for (int i = numInstances; i < numDetected; i++)
            {
                Vector3 pos = new Vector3(latestMarkers[i].Center.x, latestMarkers[i].Center.y, 0f);
                GameObject newMarker = Instantiate(markerPrefab, pos, Quaternion.Euler(0f, 0f, latestMarkers[i].Orientation), transform);
                markerInstances.Add(newMarker);
                markerAges.Add(0);
                Debug.Log("Instantiated new marker. Total now: " + markerInstances.Count);
            }

            // For any existing markers that were not updated (i.e. if fewer markers are detected than instances),
            // increment their age.
            for (int i = numDetected; i < numInstances; i++)
            {
                markerAges[i]++;
            }

            // Remove marker instances that have exceeded the deletion delay.
            for (int i = markerInstances.Count - 1; i >= numDetected; i--)
            {
                if (markerAges[i] >= deletionDelayFrames)
                {
                    Destroy(markerInstances[i]);
                    markerInstances.RemoveAt(i);
                    markerAges.RemoveAt(i);
                    Debug.Log("Removed marker instance due to deletion delay. Total now: " + markerInstances.Count);
                }
            }

            // Clear the latest markers for the next frame.
            latestMarkers = null;
        }
    }
}