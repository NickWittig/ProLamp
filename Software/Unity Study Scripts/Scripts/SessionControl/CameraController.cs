using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float DEFAULT_CAMERA_SIZE = 5f;
    private const float MAX_CAMERA_SIZE = 5f;
    private const float MIN_CAMERA_SIZE = 2.5f;
    private const float zoomSpeed = .35f;
    private const float cameraSpeed = 250f;
    private const float cameraMoveOutScale = .75f;
    private GameObject cameraGameObject;
    private Vector3 initialCameraPosition;
    private CinemachineVirtualCamera mCamera;

    private void Start()
    {
        cameraGameObject = GameObject.Find("VirtualCamera");
        mCamera = cameraGameObject.GetComponent<CinemachineVirtualCamera>();
        mCamera.m_Lens.OrthographicSize = DEFAULT_CAMERA_SIZE;
        initialCameraPosition = mCamera.transform.position;
    }

    private void Update()
    {
        HandleCameraZoom();
    }


    private void HandleCameraZoom()
    {
        var orthographicSize = mCamera.m_Lens.OrthographicSize;
        if (orthographicSize >= MAX_CAMERA_SIZE)
        {
            mCamera.transform.position = initialCameraPosition;
            orthographicSize = MAX_CAMERA_SIZE;
        }

        if ((orthographicSize >= MIN_CAMERA_SIZE
             && orthographicSize <= MAX_CAMERA_SIZE)
            || (orthographicSize < MIN_CAMERA_SIZE && Input.mouseScrollDelta.y < 0)
            || (orthographicSize > MAX_CAMERA_SIZE && Input.mouseScrollDelta.y > 0))
        {
            if (Input.mouseScrollDelta.y == 1)
            {
                LerpCameraTo(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                mCamera.m_Lens.OrthographicSize -= zoomSpeed * Input.mouseScrollDelta.y;
            }
            else if (Input.mouseScrollDelta.y == -1 && orthographicSize < MAX_CAMERA_SIZE)
            {
                LerpCameraTo(initialCameraPosition, cameraSpeed * cameraMoveOutScale);
                mCamera.m_Lens.OrthographicSize -= zoomSpeed * Input.mouseScrollDelta.y;
            }
        }
    }

    private void LerpCameraTo(Vector3 position, float speed = cameraSpeed)
    {
        var cameraZ = cameraGameObject.transform.position.z;
        cameraGameObject.transform.position = Vector2.Lerp(cameraGameObject.transform.position,
            position,
            speed * Time.deltaTime);

        cameraGameObject.transform.position = new Vector3(cameraGameObject.transform.position.x,
            cameraGameObject.transform.position.y,
            cameraZ);
    }
}