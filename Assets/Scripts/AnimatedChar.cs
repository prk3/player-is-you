using System;
using UnityEngine;

public class AnimatedChar : MonoBehaviour
{
    private int _offset;
    private int _width;
    private int _spriteVersion;

    public void SetChar(char c)
    {
        if (c == ' ')
        {
            var renderer = gameObject.GetComponent<SpriteRenderer>();
            if (renderer) Destroy(renderer);

            (_offset, _width) = (0, 20);
        }
        else
        {
            var renderer = gameObject.GetComponent<SpriteRenderer>();
            if (!renderer) renderer = gameObject.AddComponent<SpriteRenderer>();
            
            Rect view = new Rect(_offset, 32 * _spriteVersion, _width, 32); 
            renderer.sprite = Sprite.Create(Alphabet.GetTexture(), view, new Vector2(0, 0));
            
            (_offset, _width) = Alphabet.PositionAndWidthOf(c);
        }
    }

    public void NextSprite()
    {
        // if number of frames equals 2**n we can use bitwise and instead of coditions/modulo
        _spriteVersion = (_spriteVersion + 1) & 3;


        var renderer = gameObject.GetComponent<SpriteRenderer>();
        if (renderer)
        {
            var view = new Rect(_offset, 32 * _spriteVersion, _width, 32);
            renderer.sprite = Sprite.Create(Alphabet.GetTexture(), view, new Vector2(0, 0));
        }
    }

    public int GetWidth()
    {
        return _width;
    }
}