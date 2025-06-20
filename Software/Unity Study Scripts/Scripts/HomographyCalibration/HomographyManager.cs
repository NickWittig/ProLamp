using OpenCVForUnity.CoreModule;
using UnityEngine;

public class HomographyManager : SingletonBehaviour<HomographyManager>
{
    public IDataHandler<Mat> homographyHandler { get; private set; }
    public HomographyTransformer homographyTransformer { get; private set; }

    private void Awake()
    {
        base.Awake();
        homographyHandler = HomographyHandler
            .Create()
            .WithPath(Application.persistentDataPath + "/homography.json");

        homographyTransformer = new HomographyTransformer(homographyHandler.GetData());
    }


}