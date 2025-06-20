using UnityEngine;

public class InputManager : MonoBehaviour
{
    private const string TeacherCanvasName = "TeacherUICanvas";
    private const string CalibrationCanvasName = "SettingsToggleButton";

    public static Mode MODE = Mode.Drawing;
    private GameObject calibrationCanvas;
    private bool mIsFullscreen;
    private GameObject teacherCanvas;

    private void Awake()
    {
        teacherCanvas = GameObject.Find(TeacherCanvasName);
        calibrationCanvas = GameObject.Find(CalibrationCanvasName);
    }

    private void Start()
    {
        teacherCanvas.SetActive(SessionManager.GetRole() == Role.Teacher);
        calibrationCanvas.SetActive(SessionManager.GetRole() == Role.Teacher);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D)) MODE = Mode.Drawing;
        if (Input.GetKeyDown(KeyCode.S)) MODE = Mode.Selecting;
        if (Input.GetKeyDown(KeyCode.M)) MODE = Mode.Moving;

        if (Input.GetKeyDown(KeyCode.Escape) && SessionManager.GetRole() == Role.Student)
        {
            mIsFullscreen = !mIsFullscreen;
            Screen.SetResolution(DefaultManager.Instance.screenWidth, DefaultManager.Instance.screenHeight, mIsFullscreen);
        }

        if (!Input.GetKeyDown(KeyCode.Tab) || SessionManager.GetRole() == Role.Student) return;
        teacherCanvas.SetActive(!teacherCanvas.activeSelf);
        calibrationCanvas.SetActive(!calibrationCanvas.activeSelf);
    }
}