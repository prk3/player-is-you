using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Shows animation before entering/leaving a game state.
 */
public class StateTransition : MonoBehaviour
{
    private GameObject _circle;
    private bool _stateActive;

    private float _time;
    private const float TransitionDuration = 0.7f;
    private bool _fadingIn = true;
    private bool _fadingOut;
    private string _targetScene;

    void Start()
    {
        _circle = new GameObject("transition circle");
        _circle.transform.parent = gameObject.transform;

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
                _stateActive = true;
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

    /**
     * Calling this method starts transition.
     */
    public void TransitionTo(string sceneName)
    {
        _time = 0;
        _fadingOut = true;
        _targetScene = sceneName;
        _stateActive = false;
    }

    /**
     * Whether transition has ended. Useful for canceling input on game element.
     */
    public bool IsStateActive()
    {
        return _stateActive;
    }


    /**
     * Calculates the maximum scale of the animated circle based on screen size.
     */
    private float CalcCircleMaxScale()
    {
        var screenSize = Utils.GetScreenSize();
        return (float)Math.Sqrt(screenSize.x * screenSize.x + screenSize.y * screenSize.y) + 1;
    }
}
