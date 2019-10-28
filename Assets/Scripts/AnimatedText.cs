using System;
using System.Linq;
using UnityEngine;

public enum Align
{
    Left, Right, Center
}

public class AnimatedText : MonoBehaviour
{
    private string _text = "";
    private Align _align = Align.Left;
    
    // Start is called before the first frame update
    private void Start()
    {
        float offset = 0.0f;
        foreach (var c in _text)
        {
            if (c == ' ')
            {
                offset += 20.0f / 32.0f;
                continue;
            }
            
            var (position, width) = Alphabet.PositionAndWidthOf(c) ?? Alphabet.PositionAndWidthOf('?');
            
            var obj = new GameObject();
            obj.gameObject.transform.parent = gameObject.transform;
            obj.gameObject.transform.localPosition = new Vector3(offset, -1, 0);
            
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

    public void SetText(string text)
    {
        _text = text;
    }

    public void SetAlign(Align align)
    {
        _align = align;
    }

    public float GetWidth()
    {
        return _text.Aggregate(0.0f, (acc, c) => acc + (c == ' '
            ? 20.0f / 32.0f
            : (Alphabet.PositionAndWidthOf(c) ?? Alphabet.PositionAndWidthOf('?')).Item2 / 32.0f
        ));
    }
}