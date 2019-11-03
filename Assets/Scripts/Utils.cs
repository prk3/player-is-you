using UnityEngine;

public class Utils
{
    /**
     * Returns screen size in pixels.
     */
    public static Vector2Int GetScreenSizePx()
    {
        return new Vector2Int(Screen.width, Screen.height);
    }

    /**
     * Returns screen size in unity units.
     */
    public static Vector2 GetScreenSize()
    {
        float halfScreenHeight = Camera.main.orthographicSize;

        var screenSizePx = GetScreenSizePx();
    
        var screenHeight = 2 * halfScreenHeight;
        var screenWidth = ((float)screenSizePx.x / screenSizePx.y) * screenHeight;

        return new Vector2(screenWidth, screenHeight);
    }
}