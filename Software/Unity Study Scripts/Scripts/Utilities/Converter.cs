using OpenCVForUnity.CoreModule;
using System.Collections.Generic;
using UnityEngine;

public static class Converter
{
    public static float[] DoubleArrayToFloatArray(double[] doubleArray)
    {
        var floatList = new List<float>();
        foreach (var value in doubleArray)
        {
            floatList.Add((float)value);
        }

        float[] floatArray = floatList.ToArray();
        return floatArray;
    }

    public static MatOfPoint2f Vector2ArrayToMatOfPoint2f(Vector2[] vec2)
    {
        List<Point> points = new();
        var output = new MatOfPoint2f();
        foreach (var point in vec2) points.Add(new Point(point.x, point.y));
        output.fromList(points);
        return output;
    }

    public static Mat ListOfListOfDoubleToMat(List<List<double>> list)
    {
        int rows = list.Count;
        int cols = list[0].Count;
        Mat mat = new Mat(rows, cols, CvType.CV_64FC1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                mat.put(i, j, list[i][j]);
            }
        }
        return mat;
    }

    public static List<T> ArrayToList<T>(T[] array)
    {
        return new List<T>(array);
    }

}
