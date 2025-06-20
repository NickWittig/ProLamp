using UnityEngine;

public class QuaternionSanityCheck
{
    private MeanQuaternionFilter meanFilter;
    private float thresholdAngle;
    private Quaternion lastRejectedQuaternion;
    private int rejectCounter;

    public QuaternionSanityCheck(float thresholdAngle, int windowSize)
    {
        this.thresholdAngle = thresholdAngle;
        this.meanFilter = new MeanQuaternionFilter(windowSize);
        this.lastRejectedQuaternion = Quaternion.identity;
        this.rejectCounter = 0;
    }

    public bool CheckQuaternion(Quaternion newQuaternion)
    {
        Quaternion meanQuaternion = meanFilter.ApplyFilter(newQuaternion);

        float angleToMean = Quaternion.Angle(meanQuaternion, newQuaternion);
        float angleToLastRejected = Quaternion.Angle(lastRejectedQuaternion, newQuaternion);

        if (angleToMean <= thresholdAngle || angleToLastRejected <= thresholdAngle)
        {
            rejectCounter = 0;
            return true;
        }
        else
        {
            lastRejectedQuaternion = newQuaternion;
            rejectCounter++;
            if (rejectCounter >= 3) // You can adjust this number based on your needs
            {
                // If we have continuous rejections, it might be a valid change, so we apply the filter anyway
                meanFilter.ApplyFilter(newQuaternion);
                rejectCounter = 0;
            }
            return false;
        }
    }

    // Optional method to reset the sanity check
    public void Reset()
    {
        meanFilter.Reset();
        lastRejectedQuaternion = Quaternion.identity;
        rejectCounter = 0;
    }
}
