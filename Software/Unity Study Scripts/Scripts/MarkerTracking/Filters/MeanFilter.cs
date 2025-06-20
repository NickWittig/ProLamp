using System.Collections.Generic;
using UnityEngine;

public class MeanFilter
{
    private int windowSize;
    private Queue<Vector2> values;
    private Vector2 sum;

    public MeanFilter(int windowSize)
    {
        this.windowSize = windowSize;
        this.values = new Queue<Vector2>(windowSize);
        this.sum = Vector2.zero;
    }

    public Vector2 ApplyFilter(Vector2 newCorner)
    {
        if (values.Count == windowSize)
        {
            // Subtract the value that will be removed from the sum
            Vector2 oldestValue = values.Dequeue();
            sum -= oldestValue;
        }

        // Add the new value to the sum and queue
        sum += newCorner;
        values.Enqueue(newCorner);


        // Return the mean of the values
        return sum / values.Count;
    }

    public Vector2 GetPreviosCorner()
    {
        return values.Peek();
    }

    public void Reset()
    {
        values.Clear();
        sum = Vector2.zero;
    }
}
