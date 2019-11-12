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

    /**
     * Positions and scales game object according to object-fit-contain rules.
     * Returns true if changes were applied (false when window size and object size did not change).
     */
    public bool Contain(float itemWidth, float itemHeight)
    {
        var screenSizePx = Utils.GetScreenSizePx();

        if (
            screenSizePx.x == _prevScreenWidth &&
            screenSizePx.y == _prevScreenHeight &&
            itemWidth == _prevItemWidth &&
            itemHeight == _prevItemHeight)
        {
            return false;
        }

        var screenSize = Utils.GetScreenSize();

        var screenRatio = screenSize.x / screenSize.y;
        var itemRatio = itemWidth / itemHeight;

        // if screen is "wider" than map (unused space on left and right)
        if (screenRatio > itemRatio)
        {
            var scale = screenSize.y / itemHeight;
            gameObject.transform.localScale = new Vector3(scale, scale, 1);
            gameObject.transform.localPosition = new Vector3((-itemWidth / 2) *scale, (-itemHeight/2) * scale, 0);
        }
        // if screen is "narrower" than map (unused space on top and bottom)
        else
        {
            var scale = screenSize.x / itemWidth;
            gameObject.transform.localScale = new Vector3(scale, scale, 1);
            gameObject.transform.localPosition = new Vector3((-itemWidth / 2) * scale, (-itemHeight/2) * scale, 0);
        }

        _prevScreenWidth = screenSizePx.x;
        _prevScreenHeight = screenSizePx.y;

        _prevItemWidth = itemWidth;
        _prevItemHeight = itemHeight;

        return true;
    }
}
