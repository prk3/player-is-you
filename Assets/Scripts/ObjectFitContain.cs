using UnityEngine;

/**
 * Centers object on screen so that:
 * - aspect ratio of item is respected
 * - item takes full screen width/height an remaining space is equally divided
 */
public class ObjectFitContain : MonoBehaviour
{
    private int _prevScreenWidth;
    private int _prevScreenHeight;

    private float _prevItemWidth;
    private float _prevItemHeight;

    public bool Contain(float itemWidth, float itemHeight)
    {
        var (screenWidthPx, screenHeightPx) = Utils.GetScreenSizePx();

        if (
            screenWidthPx == _prevScreenWidth &&
            screenHeightPx == _prevScreenHeight &&
            itemWidth == _prevItemWidth &&
            itemHeight == _prevItemHeight)
        {
            return false;
        }

        var (screenWidth, screenHeight) = Utils.GetScreenSize();
    
        var screenRatio = screenWidth / screenHeight;
        var itemRatio = itemWidth / itemHeight;

        // if screen is "wider" than map (unused space on left and right)
        if (screenRatio > itemRatio)
        {
            var scale = screenHeight / itemHeight;
            gameObject.transform.localScale = new Vector3(scale, scale, 1);
            gameObject.transform.localPosition = new Vector3((-itemWidth / 2) *scale, (-itemHeight/2) * scale, 0);
        }
        // if screen is "narrower" than map (unused space on top and bottom)
        else
        {
            var scale = screenWidth / itemWidth;
            gameObject.transform.localScale = new Vector3(scale, scale, 1);
            gameObject.transform.localPosition = new Vector3((-itemWidth / 2) * scale, (-itemHeight/2) * scale, 0);
        }

        _prevScreenWidth = screenWidthPx;
        _prevScreenHeight = screenHeightPx;

        _prevItemWidth = itemWidth;
        _prevItemHeight = itemHeight;

        return true;
    }
}
