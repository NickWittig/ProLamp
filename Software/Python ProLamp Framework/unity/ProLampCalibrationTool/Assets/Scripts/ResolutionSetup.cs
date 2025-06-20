using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionSetup : MonoBehaviour
{
    // Start is called before the first frame update

    private void Awake()
    {

        Screen.SetResolution(1280, 800, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Screen.fullScreen = !Screen.fullScreen;

        }
    }
}
