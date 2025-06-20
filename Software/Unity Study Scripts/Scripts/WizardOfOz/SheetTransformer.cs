using UnityEngine;

public class SheetTransformer : MonoBehaviour
{

    [SerializeField] [Tooltip("in cm - demo sheet has 3.6 cm")]
    private readonly float markerSizeOnSheet = 2.7f;

    [SerializeField] private SheetManager sheetManager;

    private QuaternionSmoothing quatSmoother = new(5);
    private void Update()
    {
        if (sheetManager == null) return;
        if (sheetManager.currentMarker == null) return;
        if (MarkerTransformer.Instance == null) return;
        sheetManager.currentSheet.gameObject.transform.localPosition =  MarkerTransformer.Instance.GetLocalPosition(sheetManager.currentMarker);
        var rotation = GetNewRotation(sheetManager.currentMarker);
        sheetManager.currentSheet.gameObject.transform.GetChild(0).localRotation = rotation;
    }
    private Vector3 GetNewScale(Marker marker)
    {
        var topLine = marker.GetCorner(MarkerCorner.TOP_LEFT) - marker.GetCorner(MarkerCorner.TOP_RIGHT);
        var bottomLine = marker.GetCorner(MarkerCorner.BOTTOM_LEFT) - marker.GetCorner(MarkerCorner.BOTTOM_RIGHT);
        var scale = (topLine.magnitude + bottomLine.magnitude) / (markerSizeOnSheet * 2f) * .01f; // .13f => intrinsic camera matrix value; 1.6f => x size
        return new Vector3(5f + (scale * 1.6f), 5f + scale , 1);
    }

    private Quaternion GetNewRotation(Marker marker)
    {
        var topVector = marker.GetCorner(MarkerCorner.TOP_LEFT) - marker.GetCorner(MarkerCorner.TOP_RIGHT);
        var angle = Vector2.SignedAngle(Vector2.left, topVector);
        var newAngle = Quaternion.Euler(0f, 0f, -angle);
        return quatSmoother.SmoothQuaternion(newAngle);
    }


}