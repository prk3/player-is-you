using System;
using System.Linq;
using UnityEngine;

public enum Align
{
    Left, Right, Center
}

public class AnimatedText : MonoBehaviour
{
    public string text = "";
    public Align align = Align.Left;

    private float _cachedWidth;
    
    private void Start()
    {
        float offset = 0.0f;
        float startX = 0.0f;

        if (align == Align.Center)
        {
            startX = GetWidth() / -2.0f;
        }
        else if (align == Align.Right)
        {
            startX = GetWidth() * -1.0f;
        }
        
        foreach (var c in text)
        {
            if (c == ' ')
            {
                offset += 20.0f / 32.0f;
                continue;
            }
            
            var (position, width) = Alphabet.PositionAndWidthOf(c) ?? Alphabet.PositionAndWidthOf('?');
            
            var obj = new GameObject("char");
            obj.gameObject.transform.parent = gameObject.transform;
            obj.gameObject.transform.localPosition = new Vector3(startX + offset, -1, 0);
            obj.gameObject.transform.localScale = Vector3.one;
            
            var ren = obj.AddComponent<SpriteRenderer>();
            
            ren.sprite = Sprite.Create(
                Alphabet.GetTexture(),
                new Rect(position, 0, width, 32),
                new Vector2(0, 0),
                32
            );

            obj.AddComponent<AnimatedSprite>();

            offset += width / 32.0f;
        }
    }

    public float GetWidth()
    {
        if (_cachedWidth == 0.0f)
        {
            _cachedWidth = text.Aggregate(0.0f, (acc, c) => acc + (c == ' '
                ? 20.0f / 32.0f
                : (Alphabet.PositionAndWidthOf(c) ??
                   Alphabet.PositionAndWidthOf('?')).Item2 / 32.0f
            ));
        }

        return _cachedWidth;
    }
}