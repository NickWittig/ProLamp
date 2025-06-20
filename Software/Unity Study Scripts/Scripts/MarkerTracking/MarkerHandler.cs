using OpenCVForUnity.CoreModule;
using System.Collections.Generic;
using UnityEngine;

public class MarkerHandler : SingletonBehaviour<MarkerHandler>
{
    public List<Marker> markers { get; private set; }
    private MarkerActivityQueue markerActivityQueue = new MarkerActivityQueue(150);

    private int currentMarkerId;

    public delegate void OnMarkerChangedDelegate(Marker marker);
    public event OnMarkerChangedDelegate OnMarkerChanged;
    private void Awake() => markers = new List<Marker>();

    public void UpdateMarker(int id, List<Mat> corners)
    {
        markerActivityQueue.EnqueueID((short)id);
        if (id == -1) return;
        if (corners.Count == 0) return;
        var marker = GetMarkerById(id);
        markers.Add(marker);
        marker.SetAllCorners(corners);
        CheckMarkerChanged(id, marker);
    }

    private void CheckMarkerChanged(int id, Marker marker)
    {
        if (currentMarkerId == id) return;
        currentMarkerId = id;
        OnMarkerChanged?.Invoke(marker);
    }

    public Marker GetCurrentMarker()
    {
        return markers[currentMarkerId];
    }

    private Marker GetMarkerById(int markerId)
    {
        var marker = markers.Find(marker => marker.id == markerId);
        return marker ?? new Marker(markerId);
    }

    public bool IsMarkerActive() => markerActivityQueue.IsMarkerActive();

}
