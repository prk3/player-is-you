using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    public int numberOfSprites = 4;
    public float updateFrequency = 4;

    private float _time;
    private int _currentSpriteIndex;
    private Sprite[] _spritesUntouched;
    private Sprite[] _sprites;

    public void Start()
    {
        _time = Time.time;
        var ren = gameObject.GetComponent<SpriteRenderer>();
        var sprite = ren.sprite;
        var oldRect = sprite.textureRect;

        _spritesUntouched = new Sprite[numberOfSprites];
        for (int i = 0; i < numberOfSprites; i++)
        {
            _spritesUntouched[i] = Sprite.Create(
                sprite.texture,
                new Rect(oldRect.x, oldRect.height * i, oldRect.width, oldRect.height),
                sprite.pivot,
                sprite.pixelsPerUnit
            );
        }

        _sprites = _spritesUntouched;
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

    private void NextSprite()
    {
        _currentSpriteIndex = _currentSpriteIndex >= numberOfSprites - 1 ? 0 : _currentSpriteIndex + 1;
        gameObject.GetComponent<SpriteRenderer>().sprite = _sprites[_currentSpriteIndex];
    }

    public void Mod(Texture2D mod, Vector2Int modPosition)
    {
        Vector2Int size = new Vector2Int(_spritesUntouched[0].texture.width, _spritesUntouched[0].texture.height);
        
        for (int i = 0; i < _spritesUntouched.Length; i++)
        {
            var newTexture = TileMod.MakeModdedTexture(
                _spritesUntouched[i].texture,
                new Vector2Int((int) _spritesUntouched[i].textureRect.x, (int) _spritesUntouched[i].textureRect.y),
                mod,
                modPosition,
                size);

            newTexture.filterMode = _spritesUntouched[i].texture.filterMode;
            
            _sprites[i] = Sprite.Create(
                newTexture,
                new Rect(0, 0, size.x, size.y),
                _spritesUntouched[0].pivot,
                _spritesUntouched[0].pixelsPerUnit);
        }
    }
}