using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class TabNavigation : MonoBehaviour
{
    [SerializeField] private GameObject inputFieldContainer;
    private TMP_InputField[] inputFields;
    private Dictionary<TMP_InputField, int> inputFieldDict = new();
    private int currentIndex = 0;

    private void Start()
    {
        inputFields = inputFieldContainer.GetComponentsInChildren<TMP_InputField>();
        int i = 0;
        foreach (var inputField in inputFields)
        {
            inputFieldDict.Add(inputField, ++i % inputFields.Length);
            inputField.onSelect.AddListener(delegate {
                SetCurrentIndex(inputField);
            });
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            EventSystem.current.SetSelectedGameObject(inputFields[currentIndex].gameObject);
        }
    }
    private void SetCurrentIndex(TMP_InputField current)
    {
        currentIndex = inputFieldDict[current];
    }
}