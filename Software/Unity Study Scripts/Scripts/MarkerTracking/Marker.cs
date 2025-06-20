using OpenCVForUnity.CoreModule;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Marker
{
    private Dictionary<MarkerCorner, Vector2> corners;
    public int id { get; private set; }

    public Marker(int id = 0)
    {
        this.id = id;
        corners = new Dictionary<MarkerCorner, Vector2>
        {
            [MarkerCorner.TOP_LEFT] = new Vector2(),
            [MarkerCorner.TOP_RIGHT] = new Vector2(),
            [MarkerCorner.BOTTOM_LEFT] = new Vector2(),
            [MarkerCorner.BOTTOM_RIGHT] = new Vector2()
        };
    }


    public void SetAllCorners(List<Mat> corners)
    {
        for (var i = 0;  i < Enum.GetValues(typeof(MarkerCorner)).Length; i++)
        {
            var cornerVector = GetCorner((MarkerCorner)i);
            cornerVector.x = (float)corners[0].get(0, i)[0];
            cornerVector.y = (float)corners[0].get(0, i)[1];
            SetCorner((MarkerCorner)i, cornerVector);
        }
    }
    private void SetCorner(MarkerCorner corner, Vector2 position)
    {
        corners[corner] = position;
    }


    public Vector2 GetCorner(MarkerCorner corner)
    {
        return corners[corner];
    }

    public Vector2[] GetAllCorners()
    {
        List<Vector2> cornerList = new();
        foreach (MarkerCorner cornerType in Enum.GetValues(typeof(MarkerCorner)))
        {
            cornerList.Add(corners[cornerType]);
        }
        return cornerList.ToArray();
    }

    public Vector2[] GetAllTransformedCorners()
    {
        var halfWidth = PlayerPrefs.GetInt("width", 640) / 2f;
        var halfHeight = PlayerPrefs.GetInt("height", 480) / 2f;

        List<Vector2> cornersTransformedList = new();

        for (var i = 0; i < Enum.GetValues(typeof(MarkerCorner)).Length; i++)
        {
            var cornerVector = GetCorner((MarkerCorner)i);
            cornersTransformedList.Add(new Vector2((cornerVector.x - halfWidth) / halfWidth, -(cornerVector.y - halfHeight) / halfHeight));
        }
        return cornersTransformedList.ToArray();
    }
}