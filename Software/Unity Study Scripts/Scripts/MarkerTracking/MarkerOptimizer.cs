using UnityEngine;
using System.Collections.Generic;

public class MarkerOptimizer : MonoBehaviour
{
    Queue<List<Vector2>> cornersHistory = new Queue<List<Vector2>>();
    int maxHistoryCount = 10;

    public List<Vector2> GetOptimizedCurrentCorners()
    {
        List<Vector2> currentCorners = DetectCorners();

        if (currentCorners != null) 
        {
            cornersHistory.Enqueue(currentCorners);
            if (cornersHistory.Count > maxHistoryCount)
            {
                cornersHistory.Dequeue();
            }
        }
        else if (cornersHistory.Count > 0) 
        {
            currentCorners = InterpolateCorners();
        }

        return currentCorners;
    }

    List<Vector2> DetectCorners()
    {
        var cornersArray = MarkerHandler.Instance.GetCurrentMarker().GetAllCorners();
        return Converter.ArrayToList(cornersArray);
    }

    List<Vector2> InterpolateCorners()
    {
        var lastPosition = cornersHistory.Peek();
        var penultimatePosition = cornersHistory.Count >= 2 ? cornersHistory.ToArray()[cornersHistory.Count - 2] : lastPosition;

        List<Vector2> interpolatedCorners = new List<Vector2>();
        for (int i = 0; i < lastPosition.Count; i++)
        {
            float t = 1.0f / maxHistoryCount; 
            Vector2 interpolatedCorner = Vector2.Lerp(penultimatePosition[i], lastPosition[i], t);
            interpolatedCorners.Add(interpolatedCorner);
        }
        return interpolatedCorners;
    }
}
