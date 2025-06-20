using UnityEngine;

public class QuaternionSmoothing
{
    private MedianQuaternionFilter quaternionFilter;

    public QuaternionSmoothing(int windowSize)
    {
        quaternionFilter = new MedianQuaternionFilter(windowSize);
    }

    public Quaternion SmoothQuaternion(Quaternion quaternion)
    {
        return quaternionFilter.ApplyFilter(quaternion);
    }

    // Optional method to reset the filter
    public void Reset()
    {
        quaternionFilter.Reset();
    }
}
