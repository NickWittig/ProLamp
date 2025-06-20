using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultManager : SingletonBehaviour<DefaultManager>
{

    private const string DEFAULT_SCREEN_WIDTH = "1280";
    private const string DEFAULT_SCREEN_HEIGHT = "800";
    public int screenWidth { get; private set; }
    public int screenHeight { get; private set; }

    private void Awake()
    {
        base.Awake(false);
    }
    void Start()
    {
        var status = int.TryParse(PlayerPrefs.GetString("ProjectorWidth", DEFAULT_SCREEN_WIDTH) , out var width);
        screenWidth = status ? width : int.Parse(DEFAULT_SCREEN_WIDTH);
        status = int.TryParse(PlayerPrefs.GetString("ProjectorHeight", DEFAULT_SCREEN_HEIGHT), out var height);
        screenHeight = status ? height: int.Parse(DEFAULT_SCREEN_HEIGHT);
        Screen.SetResolution(screenWidth, screenHeight, SessionManager.GetRole() == Role.Student);
    }


}
