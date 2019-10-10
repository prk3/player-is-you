using UnityEngine;

public class AnimatedText : MonoBehaviour
{
    private float _time;
    private float _updateTime;
    public float scale = 1.0f;
    public string text = "Hello, world!";

    public AnimatedText()
    {
        _updateTime = 0.25f;
    }
        
    // Start is called before the first frame update
    private void Start()
    {
        float offset = 0.0f;
        for (int i = 0; i < text.Length; i++)
        {
            var obj = new GameObject();
            var charComp = obj.AddComponent<AnimatedChar>();
            charComp.SetChar(text[i]);
            obj.transform.Translate(new Vector3(offset, 0, 0));
            obj.transform.parent = gameObject.transform;
            offset += charComp.GetWidth() * 0.01f;
        }
        
        gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }

    private void Update()
    {
        _time += Time.deltaTime;

        if (_time > _updateTime)
        {
            var charComps = gameObject.GetComponentsInChildren<AnimatedChar>();
            foreach (var animatedChar in charComps)
            {
                animatedChar.NextSprite();
            }
            _time -= _updateTime;
        }
    }
}