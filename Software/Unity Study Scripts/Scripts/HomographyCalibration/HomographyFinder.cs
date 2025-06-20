using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.UnityUtils;
public class HomographyFinder : MonoBehaviour
{
    public Renderer displayRenderer; 
    private Texture2D srcTexture2D;

    private MatOfPoint2f srcCorners = new MatOfPoint2f();
    private MatOfPoint2f dstCorners = new MatOfPoint2f();
    private int clickedCorners = 0;

    public GameObject cornerPrefab;
    private GameObject[] cornerObjects = new GameObject[4];



    void Update()
    {
        var remoteTexture = TextureDistributorSingleton.Instance
            .GetDistributor(TextureType.RenderStream)
            .GetTexture();
        if (remoteTexture == null) return;
        if (srcTexture2D == null)
        {
            srcTexture2D = new Texture2D(remoteTexture.width, remoteTexture.height);
            dstCorners.fromArray(
                new Point(0, 0),
                new Point(srcTexture2D.width, 0),
                new Point(srcTexture2D.width, srcTexture2D.height),
                new Point(0, srcTexture2D.height)
                );
        }

        else srcTexture2D.Reinitialize(remoteTexture.width, remoteTexture.height);
        Utils.textureToTexture2D(remoteTexture, srcTexture2D);
        Mat webcamMat = new Mat(srcTexture2D.height, srcTexture2D.width, CvType.CV_8UC3);
        Utils.texture2DToMat(srcTexture2D, webcamMat);

        OnClickAddCornerPoint();

        UpdateSourceCorners();

        if (clickedCorners == 4)
        {
            Mat homographyMatrix = Calib3d.findHomography(srcCorners, dstCorners);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HomographyManager.Instance.homographyHandler.SaveData(homographyMatrix);
                Debug.Log("homograhpy matrix saved");
            }
            webcamMat.Dispose();
        }

    }

    private void UpdateSourceCorners()
    {
        srcCorners = new MatOfPoint2f();
        for (int i = 0; i < clickedCorners; i++)
        {
            Point point = ConvertToWorldSpace(cornerObjects[i].transform.position);
            srcCorners.push_back(new MatOfPoint2f(point));
        }
    }

    private void OnClickAddCornerPoint()
    {
        if (!Input.GetMouseButtonDown(0) || clickedCorners >= 4) return;
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 1f));
        cornerObjects[clickedCorners] = Instantiate(cornerPrefab, worldPos, Quaternion.identity);
        cornerObjects[clickedCorners].AddComponent<Draggable>();
        clickedCorners++;
    }

    private Point ConvertToWorldSpace(Vector3 position)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(position);

        float x = screenPos.x * srcTexture2D.width / Screen.width;
        float y = srcTexture2D.height - (screenPos.y * srcTexture2D.height / Screen.height);

        return new Point(x, y);
    }

}
