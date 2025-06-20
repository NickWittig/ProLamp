using UnityEngine;
using System.Collections.Generic;

public class MedianQuaternionFilter
{
    private int windowSize;
    private Queue<Quaternion> values;

    public MedianQuaternionFilter(int windowSize)
    {
        this.windowSize = windowSize;
        this.values = new Queue<Quaternion>(windowSize);
    }

    public Quaternion ApplyFilter(Quaternion newQuat)
    {
        if (values.Count == windowSize)
        {
            values.Dequeue(); // Remove the oldest value
        }

        values.Enqueue(newQuat); // Add the new value

        // Convert the queue to an array for ease of use
        Quaternion[] quats = values.ToArray();

        // Find the quaternion that minimizes the sum of squared differences
        float minSumSqrDifferences = float.MaxValue;
        Quaternion median = newQuat;
        foreach (Quaternion candidate in quats)
        {
            float sumSqrDifferences = 0f;
            foreach (Quaternion other in quats)
            {
                float diff = Quaternion.Angle(candidate, other);
                sumSqrDifferences += diff * diff;
            }
            if (sumSqrDifferences < minSumSqrDifferences)
            {
                minSumSqrDifferences = sumSqrDifferences;
                median = candidate;
            }
        }

        return median;
    }

    // Optional method to reset the filter
    public void Reset()
    {
        values.Clear();
    }
}
