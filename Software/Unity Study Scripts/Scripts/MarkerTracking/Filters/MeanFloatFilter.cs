using System;
using System.Collections.Generic;

public class MeanFloatFilter
{
    private Queue<float[]> values;
    private int windowSize;
    private int valueCount;

    public MeanFloatFilter(int windowSize, int valueCount)
    {
        this.windowSize = windowSize;
        this.valueCount = valueCount;
        values = new Queue<float[]>(windowSize);
    }

    public void AddValues(float[] newValues)
    {
        if (newValues.Length != valueCount)
            throw new ArgumentException($"New values must have length {valueCount}");

        values.Enqueue(newValues);

        if (values.Count > windowSize)
            values.Dequeue();
    }

    public float[] GetMeanValues()
    {
        float[] meanValues = new float[valueCount];

        foreach (var valueSet in values)
        {
            for (int i = 0; i < valueCount; i++)
            {
                meanValues[i] += valueSet[i];
            }
        }

        for (int i = 0; i < valueCount; i++)
        {
            meanValues[i] /= values.Count;
        }

        return meanValues;
    }

    public void Clear()
    {
        values.Clear();
    }
}
