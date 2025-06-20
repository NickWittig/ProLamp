using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnManager : MonoBehaviour
{
    [SerializeField] private Button tryReturnButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject confirmationPanel;

    void Start()
    {
        tryReturnButton.onClick.AddListener(() => { confirmationPanel.SetActive(!confirmationPanel.activeInHierarchy); });
        cancelButton.onClick.AddListener(() => { confirmationPanel.SetActive(false); });
    }
}
