using UnityEngine;

public static class ProjectorConverterUtility
{
    // -- Configurable Parameters --

    /// <summary>
    /// Horizontal resolution of the projection in pixels (e.g. 1280).
    /// </summary>
    public static int HorizontalResolution = 1280;

    /// <summary>
    /// Vertical resolution of the projection in pixels (e.g. 800).
    /// </summary>
    public static int VerticalResolution = 800;

    /// <summary>
    /// Physical height of the projected image in millimeters.
    /// For example, at 100% zoom a 20-world-unit tall image equals 280 mm.
    /// </summary>
    public static float PhysicalHeightMM = 280f;

    /// <summary>
    /// The orthographic camera's size (half the vertical world units).
    /// With a size of 10, the camera shows 20 world units vertically.
    /// </summary>
    public static float CameraSize = 10f;


    // -- Derived Conversion Factors / Properties --

    /// <summary>
    /// Pixels per world unit. (Uses vertical resolution and camera size.)
    /// 800 pixels span 20 world units so 1 world unit = 40 pixels.
    /// </summary>
    public static float PixelsPerWorldUnit
    {
        get { return VerticalResolution / (2f * CameraSize); }
    }

    /// <summary>
    /// Millimeters per pixel.
    /// </summary>
    public static float MMPerPixel
    {
        get { return PhysicalHeightMM / VerticalResolution; }
    }

    /// <summary>
    /// Millimeters per world unit.
    /// </summary>
    public static float MMPerWorldUnit
    {
        get { return PhysicalHeightMM / (2f * CameraSize); }
    }

    /// <summary>
    /// The physical width of the projection in millimeters.
    /// Computed using the aspect ratio: PhysicalWidthMM = PhysicalHeightMM * (HorizontalResolution / VerticalResolution)
    /// </summary>
    public static float PhysicalWidthMM
    {
        get { return PhysicalHeightMM * ((float)HorizontalResolution / VerticalResolution); }
    }


    // -- Conversion Methods --

    /// <summary>
    /// 1) Converts a value in pixels to millimeters.
    /// </summary>
    /// <param name="pixels">Value in pixels.</param>
    /// <returns>Equivalent value in millimeters.</returns>
    public static float PixelsToMM(float pixels)
    {
        return pixels * MMPerPixel;
    }

    /// <summary>
    /// 2) Converts a value in millimeters to pixels.
    /// </summary>
    /// <param name="mm">Value in millimeters.</param>
    /// <returns>Equivalent value in pixels.</returns>
    public static float MMToPixels(float mm)
    {
        return mm * VerticalResolution / PhysicalHeightMM;
    }

    /// <summary>
    /// 3) Converts a value in world units to pixels.
    /// </summary>
    /// <param name="worldUnits">Value in world units.</param>
    /// <returns>Equivalent value in pixels.</returns>
    public static float WorldUnitsToPixels(float worldUnits)
    {
        return worldUnits * PixelsPerWorldUnit;
    }

    /// <summary>
    /// 4) Converts a value in pixels to world units.
    /// </summary>
    /// <param name="pixels">Value in pixels.</param>
    /// <returns>Equivalent value in world units.</returns>
    public static float PixelsToWorldUnits(float pixels)
    {
        return pixels / PixelsPerWorldUnit;
    }

    /// <summary>
    /// 5) Converts a value in world units to millimeters.
    /// </summary>
    /// <param name="worldUnits">Value in world units.</param>
    /// <returns>Equivalent value in millimeters.</returns>
    public static float WorldUnitsToMM(float worldUnits)
    {
        return worldUnits * MMPerWorldUnit;
    }


    // -- Point Conversions --

    /// <summary>
    /// Converts a pixel coordinate (Vector2) to a world coordinate (Vector2).
    /// Assumes:
    /// - Pixel (0,0) is the bottom-left of the projection.
    /// - Pixel (HorizontalResolution, VerticalResolution) is the top-right.
    /// - The camera is centered at world (0,0), so pixel (HorizontalResolution/2, VerticalResolution/2) maps to world (0,0).
    /// For example, passing (640,400) will return (0,0).
    /// </summary>
    /// <param name="pixelPoint">The pixel coordinate as a Vector2.</param>
    /// <returns>The corresponding world coordinate as a Vector2.</returns>
    public static Vector2 PixelPointToWorldPoint(Vector2 pixelPoint)
    {
        float worldX = (pixelPoint.x - HorizontalResolution / 2f) / PixelsPerWorldUnit;
        float worldY = (pixelPoint.y - VerticalResolution / 2f) / PixelsPerWorldUnit;
        return new Vector2(worldX, worldY);
    }

    /// <summary>
    /// Converts a millimeter coordinate (Vector2) to a world coordinate (Vector2).
    /// Assumes:
    /// - The mm coordinate (0,0) corresponds to the bottom-left of the projection,
    ///   and (PhysicalWidthMM, PhysicalHeightMM) corresponds to the top-right.
    /// - Therefore, the center in mm is (PhysicalWidthMM/2, PhysicalHeightMM/2), which will map to world (0,0).
    /// The conversion is done by first converting mm to pixel coordinates and then using PixelPointToWorldPoint.
    /// </summary>
    /// <param name="mmPoint">The point in millimeters as a Vector2.</param>
    /// <returns>The corresponding world coordinate as a Vector2.</returns>
    public static Vector2 MMPointToWorldPoint(Vector2 mmPoint)
    {
        // Convert mm point to pixel coordinates.
        float pixelX = mmPoint.x * HorizontalResolution / PhysicalWidthMM;
        float pixelY = mmPoint.y * VerticalResolution / PhysicalHeightMM;
        return PixelPointToWorldPoint(new Vector2(pixelX, pixelY));
    }

    /// <summary>
    /// Converts an OpenCV coordinate point (Vector2) to a world coordinate (Vector2).
    /// OpenCV coordinates assume (0,0) is at the top-left of the image, with y increasing downward.
    /// This function first flips the y-coordinate so that (0,0) becomes the bottom-left,
    /// and then converts that pixel coordinate to a world point.
    /// </summary>
    /// <param name="opencvPoint">The OpenCV coordinate as a Vector2.</param>
    /// <returns>The corresponding world coordinate as a Vector2.</returns>
    public static Vector2 OpenCVPointToWorldPoint(Vector2 opencvPoint)
    {
        // Flip the y coordinate: OpenCV's (0,0) is top-left; we need bottom-left.
        float flippedY = VerticalResolution - opencvPoint.y;
        Vector2 pixelPoint = new Vector2(opencvPoint.x, flippedY);
        return PixelPointToWorldPoint(pixelPoint);
    }
}
