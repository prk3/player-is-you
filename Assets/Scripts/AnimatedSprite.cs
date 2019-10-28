using System;
using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    public int numberOfSprites = 4;
    public float updateFrequency = 4;
    
    private float _time;
    private int _currentSpriteIndex;

    public void Start()
    {
        _time = Time.time;
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

        var ren = gameObject.GetComponent<SpriteRenderer>();
        var sprite = ren.sprite;
        var oldRect = sprite.textureRect;

        // texture and pivot stay the same, rect changes it's y position
        ren.sprite = Sprite.Create(
            sprite.texture,
            new Rect(oldRect.x, oldRect.height * _currentSpriteIndex, oldRect.width, oldRect.height),
            sprite.pivot,
            sprite.pixelsPerUnit
        );
    }
}