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
}
