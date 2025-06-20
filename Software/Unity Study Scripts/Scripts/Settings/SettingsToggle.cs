using UnityEngine;
using UnityEngine.UI;

public class SettingsToggle : MonoBehaviour
{
    [SerializeField] private string key;

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        var value = PlayerPrefs.GetInt(key, 0) == 1;
        toggle.isOn = value;
        toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle); });
    }

    private void ToggleValueChanged(Toggle toggle)
    {
        PlayerPrefs.SetInt(key, toggle.isOn ? 1 : 0);
    }
}