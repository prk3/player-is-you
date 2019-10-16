using System;
using UnityEngine;
using UnityEngine.Experimental.U2D;

public class AnimatedText : MonoBehaviour
{
    public float scale = 1.0f;
    public string text = "Hello, world!";

    // Start is called before the first frame update
    private void Start()
    {
        float offset = 0.0f;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ')
            {
                offset += 20.0f * 0.1f;
                continue;
            }
            
            var positionAndWidth = Alphabet.PositionAndWidthOf(text[i]);
            var (position, width) = positionAndWidth ?? Alphabet.PositionAndWidthOf('?');
            
            var obj = new GameObject();
            
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite.Create(Alphabet.GetTexture(), new Rect(position, 0, width, 32), new Vector2(0, 0));

            obj.AddComponent<AnimatedSprite>();
            
            obj.transform.Translate(new Vector3(offset, 0, 0));
            obj.transform.parent = gameObject.transform;
            offset += width * 0.01f;
        }
        
        gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }
}