using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateTransition : MonoBehaviour
{
    private GameObject _circle;
    
    private float _time;
    private const float TransitionDuration = 0.7f;

    private bool _fadingIn = true;
    private bool _fadingOut;
    private string _targetScene;
    
    public void TransitionTo(string sceneName)
    {
        _time = 0;
        _fadingOut = true;
        _targetScene = sceneName;
    }

    void Start()
    {
        _circle = new GameObject();
        
        var texture = Resources.Load<Texture2D>("Textures/circle");
        texture.filterMode = FilterMode.Point;

        var ren = _circle.AddComponent<SpriteRenderer>();
        ren.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, 32, 32),
            new Vector2(0.5f, 0.5f),
            32
        );
        ren.sortingOrder = 1000;
    }

    void Update()
    {
        if (_fadingIn)
        {
            _time += Time.deltaTime;
            if (_time < TransitionDuration)
            {
                float tempScale = CalcCircleMaxScale() * (1 - _time / TransitionDuration);
                _circle.transform.localScale = new Vector3(tempScale, tempScale, 0);
            }
            else
            {
                _circle.transform.localScale = new Vector3(0, 0, 0);
                _fadingIn = false;
            }
        }

        else if (_fadingOut)
        {
            _time += Time.deltaTime;
            if (_time < TransitionDuration)
            {
                float tempScale = CalcCircleMaxScale() * (_time / TransitionDuration);
                _circle.transform.localScale = new Vector3(tempScale, tempScale, 0);
            }
            else
            {
                SceneManager.LoadScene(_targetScene);
            }
        }
    }

    private float CalcCircleMaxScale()
    {
        var (w, h) = Utils.GetScreenSize();
        return (float)Math.Sqrt(w * w + h * h);
    }
}