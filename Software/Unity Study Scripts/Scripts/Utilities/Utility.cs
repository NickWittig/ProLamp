using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utility
{
    public static Vector3[] NetworkListToArray(NetworkList<Vector3> networkList)
    {
        var tmpPositions = new Vector3[networkList.Count];
        for (var i = 0; i < networkList.Count; i++) tmpPositions[i] = networkList[i];
        return tmpPositions;
    }


    public static bool IsInside(Collider c, Vector3 point)
    {
        return c.ClosestPoint(point) == point;
    }

    public static bool IsMouseOnUI()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return true;

        var pe = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pe, hits);
        return hits.Count > 0;
    }


    public static Dictionary<string, T> LoadResources<T>(string path) where T : UnityEngine.Object
    {
        var resources = Resources.LoadAll(path, typeof(T));
        return resources.ToDictionary(pref => pref.name, pref => pref as T);
    }

    public static Vector2 GetVector2FromColonString(string key = "ProjectorAspectRatio")
    {
        Vector2 value;
        var scaleString = PlayerPrefs.GetString(key, "16:10");
        var parts = scaleString.Trim().Split(':');

        if (int.TryParse(parts[0], out var x) && int.TryParse(parts[1], out var y))
            value = new Vector2(x, y);
        else
            value = new Vector2(16, 10);
        return value;
    }


    public static void ReplaceInArray<T>(T[] array, T oldValue, T newValue)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array), "Array cannot be null.");
        }

        for (int i = 0; i < array.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(array[i], oldValue))
            {
                array[i] = newValue;
            }
        }
    }

    /// <summary>
    /// Deprecated.
    /// </summary>
    /// <param name="src"></param>
    /// <param name="dst"></param>
    public static void ConvertTextureToTexture2D(Texture src, ref Texture2D dst)
    {
        var width = src.width;
        var height = src.height;
        var tmpRT = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);
        Graphics.Blit(src, tmpRT);

        var prevRT = RenderTexture.active;
        RenderTexture.active = tmpRT;

        dst.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        dst.Apply();

        RenderTexture.active = prevRT;
        RenderTexture.ReleaseTemporary(tmpRT);
    }


}