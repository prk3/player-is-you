using UnityEngine;

/**
 * Changes vertical offset of a texture rect on an interval.
 */
public class AnimatedSprite : MonoBehaviour
{
    public int numberOfSprites = 4;
    public float updateFrequency = 4;

    private float _time;
    private int _currentSpriteIndex;
    private Sprite[] _spritesModed;
    private Sprite[] _sprites;

    public void Start()
    {
        _time = 0;
        var ren = gameObject.GetComponent<SpriteRenderer>();
        var sprite = ren.sprite;
        var oldRect = sprite.textureRect;

        _sprites = new Sprite[numberOfSprites];
        for (int i = 0; i < numberOfSprites; i++)
        {
            _sprites[i] = Sprite.Create(
                sprite.texture,
                new Rect(oldRect.x, oldRect.height * i, oldRect.width, oldRect.height),
                sprite.pivot,
                sprite.pixelsPerUnit
            );
        }

        _spritesModed = new Sprite[_sprites.Length];
        for (int i = 0; i < _spritesModed.Length; i++)
        {
            _spritesModed[i] = _sprites[i];
        }
    }

    public void Update()
    {
        _time += Time.deltaTime;

        if (_time > 1.0f / updateFrequency)
        {
            NextSprite();
            _time -= 1.0f / updateFrequency;
        }
    }

    /**
     * Moves to next (or first) position in a texture.
     */
    private void NextSprite()
    {
        _currentSpriteIndex = _currentSpriteIndex >= numberOfSprites - 1 ? 0 : _currentSpriteIndex + 1;
        gameObject.GetComponent<SpriteRenderer>().sprite = _spritesModed[_currentSpriteIndex];
    }

    /**
     * Applied mod to all versions of an animated sprite.
     */
    public void Mod(Texture2D mod, Vector2Int modPosition)
    {
        Vector2Int size = new Vector2Int((int)_sprites[0].textureRect.width, (int)_sprites[0].textureRect.height);

        for (int i = 0; i < _spritesModed.Length; i++)
        {
            var newTexture = TileMod.MakeModdedTexture(
                _sprites[i].texture,
                new Vector2Int((int) _sprites[i].textureRect.x, (int) _sprites[i].textureRect.y),
                mod,
                modPosition,
                size);

            newTexture.filterMode = _sprites[i].texture.filterMode;

            _spritesModed[i] = Sprite.Create(
                newTexture,
                new Rect(0, 0, size.x, size.y),
                _sprites[0].pivot,
                _sprites[0].pixelsPerUnit);
        }
    }
}
