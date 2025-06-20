using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.CoreModule;
using System.Collections.Generic;
using UnityEngine;

public class MarkerTransformer : SingletonBehaviour<MarkerTransformer>
{
    private const float markerCmOnSheet = 2.7f;

    private Mat rotationMat = new Mat(3, 3, CvType.CV_64FC1);
    private Matrix4x4 rotationMatrix = new();
    private Mat rvec = new();
    private Mat tvec = new();
    private MatOfPoint3f objectPoints = new();

    private CornerSmoothing cornerSmoother = new(25);

    private CornerSanityCheck CornerSanityCheck = new(6, 4);

    internal class Pose
    {
        public Mat rvec;
        public Mat tvec;

        public Pose(Mat rvec, Mat tvec)
        {
            this.rvec = rvec;
            this.tvec = tvec;
        }
    }

    private void Awake()
    {
        var unityPoints = new List<Vector3>
        {
            new Vector3(-markerCmOnSheet / 2f, -markerCmOnSheet / 2f, 0),
            new Vector3(markerCmOnSheet / 2f, -markerCmOnSheet / 2f, 0),
            new Vector3(-markerCmOnSheet / 2f,  markerCmOnSheet / 2f , 0),
            new Vector3(markerCmOnSheet / 2f , markerCmOnSheet / 2f, 0)
        };

        objectPoints = GetObjectPoints(unityPoints);

    }


    private Pose EstimatePose(Marker marker)
    {
        var imgPoints = Converter.Vector2ArrayToMatOfPoint2f(marker.GetAllCorners());
        var distNew = new MatOfDouble(CameraCalibrationDataManager.Instance.data.dist);
        Calib3d.solvePnP(objectPoints, imgPoints, CameraCalibrationDataManager.Instance.data.mtx, distNew, rvec, tvec);
        return new Pose(rvec, tvec);
    }

    public Vector3 GetLocalPosition(Marker marker)
    {
        var meanPoint = GetMeanPoint(marker);
        return new Vector3(meanPoint.x, meanPoint.y, -.01f);
    }
    private Vector2 GetMeanPoint(Marker marker)
    {
  
        var filteredTransformedCorners = GetFilteredCorners(marker.GetAllTransformedCorners());

        var meanPoint = new Vector2();

        foreach (var corner in filteredTransformedCorners)
        {
            meanPoint.x += corner.x;
            meanPoint.y += corner.y;
        }
        meanPoint /= filteredTransformedCorners.Length;
        return meanPoint;

    }
    private Vector2[] GetFilteredCorners(Vector2[] corners)
    {
        var rejectedIdxs = CornerSanityCheck.CheckCorners(corners);

        if (rejectedIdxs.Length != 0)
        {
            var previousCorners = cornerSmoother.GetPreviousCorners(rejectedIdxs);
            foreach (var idx in rejectedIdxs)
            {
                Utility.ReplaceInArray(corners, corners[idx], previousCorners[idx]);
            }
        }
        var smoothedKalmanCorners = cornerSmoother.SmoothCorners(corners);
        return smoothedKalmanCorners;
    }


    public Quaternion GetLocalRotation(Marker marker)
    {
        var pose = EstimatePose(marker);
        rotationMatrix = GetRotationMatrix(pose.rvec);
        var rotationQuaternion = rotationMatrix.rotation;
        var transformedRotationQuaternion = new Quaternion(-rotationQuaternion.x, rotationQuaternion.y, -rotationQuaternion.z, rotationQuaternion.w);
        return transformedRotationQuaternion;
    }

    private Matrix4x4 GetRotationMatrix(Mat rvec)
    {
        Calib3d.Rodrigues(rvec, rotationMat);

        double[] rotationDobuleArray = new double[9];
        rotationMat.get(0, 0, rotationDobuleArray);

        float[] rotationFloatArray = Converter.DoubleArrayToFloatArray(rotationDobuleArray);

        rotationMatrix.SetRow(0, new Vector4(rotationFloatArray[0], rotationFloatArray[1], rotationFloatArray[2], 0));
        rotationMatrix.SetRow(1, new Vector4(rotationFloatArray[3], rotationFloatArray[4], rotationFloatArray[5], 0));
        rotationMatrix.SetRow(2, new Vector4(rotationFloatArray[6], rotationFloatArray[7], rotationFloatArray[8], 0));
        rotationMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

        return rotationMatrix;
    }

    private MatOfPoint3f GetObjectPoints(List<Vector3> unityPoints)
    {
        List<Point3> pointList = new List<Point3>();
        foreach (var point in unityPoints) pointList.Add(new Point3(point.x, point.y, point.z));
        objectPoints.fromList(pointList);
        return objectPoints;
    }

}
