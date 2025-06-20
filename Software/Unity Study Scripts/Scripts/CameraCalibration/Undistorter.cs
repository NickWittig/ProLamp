using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public class Undistorter : SingletonBehaviour<Undistorter>, ITextureTransformer
{

    private Mat newCameraMtx, srcMat, dstMat;
    private Size texture2DSize;
    private bool isInitialized = false;
    private bool isProcessing = false;
    private Texture2D bufferTexture;
    public Texture2D outputTexture { get; private set; }


    private void Awake()
    {
        base.Awake();
    }

    private void InitParameters(WebCamTexture webcamTexture)
    {
        Assert.IsNotNull(CameraCalibrationDataManager.Instance);
        Assert.IsNotNull(webcamTexture);

        texture2DSize = new Size(webcamTexture.width, webcamTexture.height);
        newCameraMtx = Calib3d.getOptimalNewCameraMatrix(CameraCalibrationDataManager.Instance.data.mtx, CameraCalibrationDataManager.Instance.data.dist, texture2DSize, 1, texture2DSize);
        srcMat = new Mat(texture2DSize, CvType.CV_8UC4);
        dstMat = new Mat(texture2DSize, CvType.CV_8UC4);
        bufferTexture = new Texture2D(webcamTexture.width, webcamTexture.height);
        outputTexture = new Texture2D(webcamTexture.width, webcamTexture.height);
        isInitialized = true;
    }

    private void InitParameter(Texture2D texture2D)
    {
        Assert.IsNotNull(CameraCalibrationDataManager.Instance);
        Assert.IsNotNull(texture2D);

        texture2DSize = new Size(texture2D.width, texture2D.height);
        newCameraMtx = Calib3d.getOptimalNewCameraMatrix(CameraCalibrationDataManager.Instance.data.mtx, CameraCalibrationDataManager.Instance.data.dist, texture2DSize, 1, texture2DSize);
        srcMat = new Mat(texture2DSize, CvType.CV_8UC4);
        dstMat = new Mat(texture2DSize, CvType.CV_8UC4);
        bufferTexture = new Texture2D(texture2D.width, texture2D.height);
        outputTexture = new Texture2D(texture2D.width, texture2D.height);
        isInitialized = true;
    }


    public Texture2D GetUndistortedTexture2DAsync(WebCamTexture webCamTexture)
    {
        if (!isProcessing)
        {
            if (bufferTexture != null && outputTexture != null)
            {
                outputTexture.Reinitialize(bufferTexture.width, bufferTexture.height);
                outputTexture.LoadRawTextureData(bufferTexture.GetRawTextureData());
                outputTexture.Apply();
            }
            StartCoroutine(UndistortTextureCoroutine(webCamTexture));
        }
        return outputTexture;
    }
    private IEnumerator UndistortTextureCoroutine(WebCamTexture texture)
    {
        isProcessing = true;
        yield return UndistortTextureAsync(texture);
        isProcessing = false;
    }


    private async Task UndistortTextureAsync(WebCamTexture webcamTexture)
    {
        if (!isInitialized) InitParameters(webcamTexture);

        Utils.webCamTextureToMat(webcamTexture, srcMat);
        await Task.Run(() => Calib3d.undistort(srcMat, dstMat, CameraCalibrationDataManager.Instance.data.mtx, CameraCalibrationDataManager.Instance.data.dist, newCameraMtx));
        bufferTexture.Reinitialize(webcamTexture.width, webcamTexture.height);
        Utils.matToTexture2D(dstMat, bufferTexture);

    }


    public Texture2D GetUndistortedTexture2D(WebCamTexture webCamTexture)
    {
        if (!isInitialized) InitParameters(webCamTexture);

        Utils.webCamTextureToMat(webCamTexture, srcMat);
        Calib3d.undistort(srcMat, dstMat, CameraCalibrationDataManager.Instance.data.mtx, CameraCalibrationDataManager.Instance.data.dist, newCameraMtx);
        bufferTexture.Reinitialize(webCamTexture.width, webCamTexture.height);
        Utils.matToTexture2D(dstMat, bufferTexture);
        return bufferTexture;
    }



    public Texture2D GetTexture2D(WebCamTexture webCamTexture)
    {
        if (!isInitialized) InitParameters(webCamTexture);

        bufferTexture.Reinitialize(webCamTexture.width, webCamTexture.height);
        Utils.textureToTexture2D(webCamTexture, bufferTexture);
        return bufferTexture;
    }


    public void Apply(ref Texture2D texture2d)
    {
        if (!isInitialized) InitParameter(texture2d);

        Utils.texture2DToMat(texture2d, srcMat);
        Calib3d.undistort(srcMat, dstMat, CameraCalibrationDataManager.Instance.data.mtx, CameraCalibrationDataManager.Instance.data.dist, newCameraMtx);
        bufferTexture.Reinitialize(texture2d.width, texture2d.height);
        Utils.matToTexture2D(dstMat, bufferTexture);
        texture2d = bufferTexture;

    }

}

