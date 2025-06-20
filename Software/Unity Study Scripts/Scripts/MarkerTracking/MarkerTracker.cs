using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenCVForUnity.ArucoModule;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using UnityEngine;
using UnityEngine.Assertions;

public class MarkerTracker : MonoBehaviour
{

    private const int ARUCO_VERSION_USED = Aruco.DICT_ARUCO_ORIGINAL;

    private bool isProcessing = false;
    private bool hasStarted = false;

    private Mat ids = new();
    private Mat imgMat = new();
    private Mat tmpMat = new();
    private List<Mat> corners = new();
    private Dictionary arucoDict;
    private float timeSinceLastDetection = 0f;
    private float detectionInterval = 1/30f;





    private void Start()
    {
        Assert.IsNotNull(SessionManager.Instance);
        SessionManager.Instance.OnServerScenesLoaded += OnScenesLoaded_Start;
        arucoDict = Aruco.getPredefinedDictionary(ARUCO_VERSION_USED);
    }

    private void OnScenesLoaded_Start()
    {
        Assert.IsNotNull(TextureDistributorSingleton.Instance);
        hasStarted = true;
    }

    private void Update()
    {
        if (!hasStarted) return;
        if (TextureDistributorSingleton.Instance == null) return;
        if (MarkerHandler.Instance == null) return;
        timeSinceLastDetection += Time.deltaTime;
        if (timeSinceLastDetection < detectionInterval) return;

        timeSinceLastDetection = 0f;
        var texture = TextureDistributorSingleton.Instance
            .GetDistributor(TextureType.Webcam)
            ?.GetTexture2D(true, true);


        SetupVariables(texture);
        Task.Run(() =>
        {
            Aruco.detectMarkers(imgMat, arucoDict, corners, ids, tmpMat: ref tmpMat);
        }).ContinueWith(_ =>
        {
            // Update UI with the result
            int id = -1;
            if (!ids.empty())
            {
                if (ids.get(0, 0).Length > 0)
                {
                    id = (int)ids.get(0, 0)[0];
                }
            }
            MarkerHandler.Instance.UpdateMarker(id, corners);
            corners.Clear();
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private IEnumerator FindMarkerAsync(Texture2D texture)
    {
        isProcessing = true;
        yield return FindMarkerInTextureTask(texture);
        isProcessing = false;
    }


    private void FindMarkerInTexture(Texture2D texture2D)
    {
        if (imgMat.rows() != texture2D.width || imgMat.cols() != texture2D.height)
        {
            imgMat.create(texture2D.height, texture2D.width, CvType.CV_8UC3); // Adjust type as needed
        }

        Utils.texture2DToMat(texture2D, imgMat);
        TexturePreprocessor.Preprocess(ref imgMat);
        corners.Clear();

        Aruco.detectMarkers(imgMat, arucoDict, corners, ids, tmpMat: ref tmpMat);

        var id = (int)ids.get(0, 0)[0];
        if (id != 0) MarkerHandler.Instance.UpdateMarker(id, corners);
    }

    private async Task FindMarkerInTextureTask(Texture2D texture2D)
    {
        SetupVariables(texture2D);
        await Task.Run(() =>
        {
            Aruco.detectMarkers(imgMat, arucoDict, corners, ids, tmpMat: ref tmpMat);
        }).ContinueWith(_ =>
        {
            // Update UI with the result
            var id = (int)ids.get(0, 0)[0];
            if (id != 0) MarkerHandler.Instance.UpdateMarker(id, corners);
            corners.Clear();
        }, TaskScheduler.FromCurrentSynchronizationContext());


    }

    private void SetupVariables(Texture2D texture2D)
    {
        if (imgMat.rows() != texture2D.width || imgMat.cols() != texture2D.height)
        {
            imgMat.create(texture2D.height, texture2D.width, CvType.CV_8UC3);
        }

        Utils.texture2DToMat(texture2D, imgMat);
        TexturePreprocessor.Preprocess(ref imgMat);
    }

    private void OnDestroy()
    {
        if (corners != null) foreach (var corner in corners) corner.release();
        if (tmpMat != null) tmpMat.Dispose();
        if (imgMat != null) imgMat.Dispose();
        if (ids != null) ids.release();
    }
}