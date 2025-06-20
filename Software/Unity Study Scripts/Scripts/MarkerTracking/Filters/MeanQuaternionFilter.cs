using UnityEngine;
using System.Collections.Generic;

public class MeanQuaternionFilter
{
    private int windowSize;
    private Queue<Quaternion> values;

    public MeanQuaternionFilter(int windowSize)
    {
        this.windowSize = windowSize;
        this.values = new Queue<Quaternion>(windowSize);
    }

    public Quaternion ApplyFilter(Quaternion newQuaternion)
    {
        if (values.Count == windowSize)
        {
            values.Dequeue();
        }

        values.Enqueue(newQuaternion);

        // Compute the mean of the quaternions
        Quaternion mean = new Quaternion(0, 0, 0, 0);
        foreach (Quaternion q in values)
        {
            float scale = 1.0f / values.Count;
            mean = Quaternion.Slerp(mean, q, scale);
        }

        return mean;
    }

    public void Reset()
    {
        values.Clear();
    }
}
