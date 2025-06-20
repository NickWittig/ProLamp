using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;

public static class TexturePreprocessor
{
    public static void Preprocess(ref Mat imgMat, double alpha = 1.5, double beta = 50)
    {

        Imgproc.cvtColor(imgMat, imgMat, Imgproc.COLOR_BGR2GRAY);
        Imgproc.GaussianBlur(imgMat, imgMat, new Size(5, 5), 0);

    }
}
