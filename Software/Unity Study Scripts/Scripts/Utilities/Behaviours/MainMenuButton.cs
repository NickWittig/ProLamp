using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour
{

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate { ModuleManager.Instance.LoadMainMenu(); });
    }

}
