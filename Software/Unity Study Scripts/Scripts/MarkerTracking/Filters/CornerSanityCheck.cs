using System;
using System.Collections.Generic;
using UnityEngine;

public class CornerSanityCheck
{
    private MeanFloatFilter meanFilter;
    private float[] smoothedPreviousDistances;
    private int numCorners;

    public CornerSanityCheck(int windowSize, int numCorners)
    {
        this.numCorners = numCorners;
        meanFilter = new MeanFloatFilter(windowSize, numCorners);
    }

    public int[] CheckCorners(Vector2[] corners)
    {
        if (corners.Length != numCorners)
            throw new ArgumentException($"Corners array must have length {numCorners}");

        float[] currentDistances = new float[numCorners];
        List<int> rejectedIndexes = new List<int>();

        for (int i = 0; i < numCorners; i++)
        {
            for (int j = 0; j < numCorners; j++)
            {
                if (i != j)
                {
                    currentDistances[i] += Vector2.Distance(corners[i], corners[j]);
                }
            }

            currentDistances[i] /= numCorners - 1;

            if (smoothedPreviousDistances != null &&
                currentDistances[i] > smoothedPreviousDistances[i] + 1f)
            {
                rejectedIndexes.Add(i);
            }
        }

        meanFilter.AddValues(currentDistances);
        smoothedPreviousDistances = meanFilter.GetMeanValues();

        return rejectedIndexes.ToArray();
    }

    public void Reset()
    {
        meanFilter.Clear();
        smoothedPreviousDistances = null;
    }
}
