using TMPro;
using UnityEngine;

public class ConfigurationInputField : MonoBehaviour
{
    private TMP_InputField inputField;
    [SerializeField] private string key;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        var value = PlayerPrefs.GetString(key, "NULL");
        if (value != "NULL") inputField.text = value;
        inputField.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    private void OnValueChanged()
    {
        PlayerPrefs.SetString(key, inputField.text);
    }
}