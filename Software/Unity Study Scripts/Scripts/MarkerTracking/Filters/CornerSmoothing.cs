using UnityEngine;

public class CornerSmoothing
{
    private MedianFilter[] cornerFilters;

    public CornerSmoothing(int windowSize)
    {
        cornerFilters = new MedianFilter[4];
        for (int i = 0; i < 4; i++)
        {
            cornerFilters[i] = new MedianFilter(windowSize);
        }
    }

    public Vector2[] SmoothCorners(Vector2[] corners)
    {
        Vector2[] smoothedCorners = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            smoothedCorners[i] = cornerFilters[i].ApplyFilter(corners[i]);
        }
        return smoothedCorners;
    }

    public Vector2[] GetPreviousCorners(int[] idxs)
    {
        Vector2[] previousCorners = new Vector2[4];
        foreach(var idx in idxs )
        {
            previousCorners[idx] = cornerFilters[idx].GetPreviosCorner();
        }
        return previousCorners;
    }
}
