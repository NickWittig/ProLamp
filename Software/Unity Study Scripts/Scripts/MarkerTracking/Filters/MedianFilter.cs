using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class MedianFilter
{
    private int windowSize;
    private List<Vector2> values;

    public MedianFilter(int windowSize)
    {
        this.windowSize = windowSize;
        this.values = new List<Vector2>(windowSize);
    }

    public Vector2 ApplyFilter(Vector2 newCorner)
    {
        if (values.Count == windowSize)
        {
            // Remove the oldest value
            values.RemoveAt(0);
        }

        // Add the new value to the list
        values.Add(newCorner);

        // Sort the values
        Vector2[] sortedValues = values.OrderBy(v => v.x).ThenBy(v => v.y).ToArray();

        // Calculate the median
        Vector2 median;
        if (sortedValues.Length % 2 == 0)
        {
            median = (sortedValues[sortedValues.Length / 2 - 1] + sortedValues[sortedValues.Length / 2]) / 2.0f;
        }
        else
        {
            median = sortedValues[sortedValues.Length / 2];
        }

        return median;
    }

    public Vector2 GetPreviosCorner()
    {
        return values[^1];
    }
    // Optional method to reset the filter
    public void Reset()
    {
        values.Clear();
    }
}
