using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AppControl;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private TMP_Dropdown webcamSelectDropdown;


    private void Awake()
    {
        AddWebCamsToDropdown(webcamSelectDropdown);
        backButton.onClick.AddListener(() => { ModuleManager.Instance.SwitchSceneFromTo(3, 1); });
    }


    private void AddWebCamsToDropdown(TMP_Dropdown dropdown)
    {
        var devices = WebCamTexture.devices;
        dropdown.ClearOptions();
        var options = devices.Select(device => new TMP_Dropdown.OptionData(device.name)).ToList();

        dropdown.AddOptions(options);
        dropdown.value = PlayerPrefs.GetInt("CameraIndex", 0);
        dropdown.onValueChanged.AddListener(index => PlayerPrefs.SetInt("CameraIndex", index));
    }
}