public enum TextureType
{
    /// <summary>
    /// Provides the Texture of the local Webcam.
    /// </summary>
    Webcam,
    /// <summary>
    /// Provides the Texture of the remote Webcam through Unity Render Sreaming.
    /// <see href="https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/manual/index.html">Manual</see>.
    /// Version: 3.1.0-exp.3
    /// </summary>
    RenderStream,
    /// <summary>
    /// Provides the Texture of a Unity Camera. 
    /// </summary>
    Camera
}