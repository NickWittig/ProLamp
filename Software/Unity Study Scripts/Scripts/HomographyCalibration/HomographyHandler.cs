using OpenCVForUnity.CoreModule;
using System.IO;
using UnityEngine;

public class HomographyHandler : IDataHandler<Mat>, ICreatable
{

    private string path;

    private string defaultPath = Application.persistentDataPath + "/homography.json";

    private Mat homographyMat = new Mat(3, 3, CvType.CV_64F);
    private HomographyData homographyData;
    private HomographyHandler() { }

    public static HomographyHandler Create()
    {
        return new HomographyHandler();
    }

    public HomographyHandler WithPath(string path)
    {
        this.path = path;
        return this;
    }


    private void UpdateHomographyMat()
    {
        TryUpdateHomographyDataFromFile();
        if (homographyData == null) return;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                homographyMat.put(i, j, homographyData.matrix[i * 3 + j]);
            }
        }
    }

    private void TryUpdateHomographyDataFromFile()
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            if (!File.Exists(defaultPath)) return;
            else path = defaultPath;
        }

        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<HomographyData>(json);
        homographyData = data;


    }
    public Mat GetData()
    {
        UpdateHomographyMat();
        return homographyMat;
    }

    private HomographyData GetHomographyDataFromMat(Mat homographyMat)
    {
        HomographyData data = new HomographyData();
        data.matrix = new double[9];

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                data.matrix[i * 3 + j] = homographyMat.get(i, j)[0];
            }
        }
        return data;

    }
    public void SaveData(Mat homographyMat)
    {
        var data = GetHomographyDataFromMat(homographyMat);
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
        if (HomographyNetworkSaver.Instance == null) return;
        HomographyNetworkSaver.Instance.SaveDataClientRPC(json);
    }
}
