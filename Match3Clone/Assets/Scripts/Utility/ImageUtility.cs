using UnityEngine;

public static class ImageUtility
{
    /// <summary>
    /// Converts a Texture2D to a PNG byte array.
    /// </summary>
    public static byte[] ConvertImageToBytes(Texture2D texture)
    {
        return texture.EncodeToPNG(); // Use EncodeToJPG() for smaller sizes but lossy compression
    }

    /// <summary>
    /// Creates a Texture2D from a byte array.
    /// </summary>
    public static Texture2D ConvertBytesToImage(byte[] imageData)
    {
        Texture2D texture = new Texture2D(2, 2); // 2x2 is a placeholder; it will resize automatically
        texture.LoadImage(imageData); // LoadImage resizes the texture to fit the data
        return texture;
    }
        // Resize a texture to target dimensions
    public static Texture2D ResizeTexture(Texture2D original, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        RenderTexture.active = rt;

        Graphics.Blit(original, rt);

        Texture2D resized = new Texture2D(width, height, original.format, false);
        resized.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        resized.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return resized;
    }

    // Compress texture to JPEG format with a specified quality
    public static byte[] CompressTexture(Texture2D texture, int quality = 50)
    {
        return texture.EncodeToJPG(quality); // JPEG quality from 1-100
    }
}
