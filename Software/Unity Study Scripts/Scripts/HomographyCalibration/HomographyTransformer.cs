using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using UnityEngine;

public class HomographyTransformer : ITextureTransformer
{

    private Mat srcMat = new();
    private Mat dstMat = new();
    private Mat homographyMatrix;
    public HomographyTransformer(Mat homographyMatrix)
    {
        this.homographyMatrix = homographyMatrix;
    }
    public void Apply(ref Texture2D srcTexture)
    {

        if (srcTexture == null) return;
        var size = new Size(srcTexture.width, srcTexture.height);
        if (srcMat.rows() != srcTexture.width || srcMat.cols() != srcTexture.height)
        {
            srcMat.create(srcTexture.height, srcTexture.width, CvType.CV_8UC3);
        }
        if (dstMat.rows() != srcTexture.width || dstMat.cols() != srcTexture.height)
        {
            dstMat.create(srcTexture.height, srcTexture.width, CvType.CV_8UC3);
        }


        Utils.texture2DToMat(srcTexture, srcMat);
        Imgproc.warpPerspective(srcMat, dstMat, homographyMatrix, size);
        srcTexture.Reinitialize(dstMat.cols(), dstMat.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(dstMat, srcTexture);
        srcTexture.Apply();
    }

}
