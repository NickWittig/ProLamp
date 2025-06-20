using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;


public class CameraCalibrationDataManager : SingletonBehaviour<CameraCalibrationDataManager>
{
    internal class SerializedCalibrationData
    {
        public List<List<double>> mtx;
        public List<List<double>> dist;
        public List<List<List<double>>> rvecs;
        public List<List<List<double>>> tvecs;
    }
    public CameraCalibrationData data { get; private set; }



    private void Awake()
    {
        base.Awake();
        var fullPath = $"{Application.persistentDataPath}/{PlayerPrefs.GetString("CameraCalibFile", "calibration_v4k")}.json";
        var serializedCalibrationData = DeserializeCalibrationJson(fullPath);
        data = GetCalibrationData(serializedCalibrationData);
    }

    private CameraCalibrationData GetCalibrationData(SerializedCalibrationData serializedCalibrationData)
    {
        CameraCalibrationData calibrationData = new();
        calibrationData.mtx = Converter.ListOfListOfDoubleToMat(serializedCalibrationData.mtx);
        calibrationData.dist = Converter.ListOfListOfDoubleToMat(serializedCalibrationData.dist);
        return calibrationData;
    }

    private SerializedCalibrationData DeserializeCalibrationJson(string path)
    {
        Assert.IsTrue(File.Exists(path));
        string jsonString = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<SerializedCalibrationData>(jsonString);
    }
}

