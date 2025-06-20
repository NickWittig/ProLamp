using UnityEngine;
using System.Globalization;

/// <summary>
/// MarkerDataParser is responsible for parsing UDP messages containing marker center coordinates and orientations.
/// 
/// The expected message format is:
///     "N, center1_x, center1_y, orientation1, center2_x, center2_y, orientation2, �, centerN_x, centerN_y, orientationN"
/// where N is the number of markers.
/// Each numeric value is parsed using InvariantCulture.
/// </summary>
public static class MarkerDataParser
{
    /// <summary>
    /// Contains the parsed information for one marker.
    /// </summary>
    public struct MarkerInfo
    {
        public Vector2 Center;
        public int Orientation; // Rough orientation in degrees (e.g., 0, 90, 180, 270)

        public MarkerInfo(Vector2 center, int orientation)
        {
            Center = center;
            Orientation = orientation;
        }
    }

    /// <summary>
    /// Parses a UDP message into an array of MarkerInfo objects.
    /// </summary>
    /// <param name="message">The UDP message as a comma-separated string.</param>
    /// <returns>
    /// An array of MarkerInfo, or null if parsing fails.
    /// </returns>
    public static MarkerInfo[] ParseMarkerMessage(string message)
    {
        string[] tokens = message.Split(',');
        if (tokens.Length < 1)
        {
            Debug.LogWarning("Received UDP message is empty: " + message);
            return null;
        }

        // First token: number of markers (N)
        int markerCount;
        if (!int.TryParse(tokens[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out markerCount))
        {
            Debug.LogWarning("Failed to parse marker count from message: " + message);
            return null;
        }

        // Expect exactly 1 + 3 * markerCount tokens.
        if (tokens.Length != 1 + 3 * markerCount)
        {
            Debug.LogWarning($"Expected {1 + 3 * markerCount} tokens but got {tokens.Length}. Message: " + message);
            return null;
        }

        MarkerInfo[] markers = new MarkerInfo[markerCount];
        try
        {
            for (int i = 0; i < markerCount; i++)
            {
                float x = float.Parse(tokens[1 + 3 * i], CultureInfo.InvariantCulture);
                float y = float.Parse(tokens[1 + 3 * i + 1], CultureInfo.InvariantCulture);
                int orientation = int.Parse(tokens[1 + 3 * i + 2], CultureInfo.InvariantCulture);

                Vector2 opencvPoint = new Vector2(x, y);
                // Convert the OpenCV coordinate (top-left origin) to a Unity world coordinate.
                Vector2 worldPoint = ProjectorConverterUtility.OpenCVPointToWorldPoint(opencvPoint);
                markers[i] = new MarkerInfo(worldPoint, orientation);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing marker message: " + ex);
            return null;
        }
        return markers;
    }
}