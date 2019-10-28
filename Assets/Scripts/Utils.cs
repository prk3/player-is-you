
using System;
using UnityEngine;

public class Utils
{
    public static (int, int) GetScreenSizePx()
    {
        return (Screen.width, Screen.height);
    }

    public static (float, float) GetScreenSize()
    {
        float halfScreenHeight = Camera.main.orthographicSize;

        var (widthPx, heightPx) = GetScreenSizePx();
    
        var screenHeight = 2 * halfScreenHeight;
        var screenWidth = ((float)widthPx / heightPx) * screenHeight;

        return (screenWidth, screenHeight);
    }
}