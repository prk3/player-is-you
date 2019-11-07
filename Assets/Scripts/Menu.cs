using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Menu : MonoBehaviour
{
    private GameObject _arrow;
    private List<(string, Action)> _items = new List<(string, Action)>();
    private List<float> _widths = new List<float>();
    private int _selectedItem;

    public void AddItem(string text, Action d)
    {
        _items.Add((text, d));
    }

    void Start()
    {
        var children = new List<GameObject>(_items.Count);

        for (int i = 0; i < _items.Count; i++)
        {
            string text = _items[i].Item1;
            var obj = new GameObject();
            obj.transform.parent = gameObject.transform;
            obj.transform.localPosition = new Vector3(0, -1.5f * i, 0);

            var textComponent = obj.AddComponent<AnimatedText>();
            textComponent.SetText(text);

            children.Add(obj);
            _widths.Add(textComponent.GetWidth());
        }

        float maxWidth = _widths.Max();

        for (int i = 0; i < children.Count; i++)
        {
            children[i].transform.localPosition = new Vector3(
                2 + (maxWidth - _widths[i]) / 2.0f,
                children[i].transform.localPosition.y,
                0);
        }

        _arrow = new GameObject();
        _arrow.transform.parent = gameObject.transform;
        _arrow.transform.Translate((maxWidth - _widths[0]) / 2, -1, 0);

        var tex = Resources.Load<Texture2D>("Textures/arrow");
        tex.filterMode = FilterMode.Point;

        var ren = _arrow.AddComponent<SpriteRenderer>();
        ren.sprite = Sprite.Create(
            tex,
            new Rect(0, 0, 16, 16),
            new Vector2(0, 0),
            16
        );
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            _selectedItem -= 1;
            if (_selectedItem < 0) _selectedItem = _items.Count - 1;
            MoveArrow(_selectedItem);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            _selectedItem += 1;
            if (_selectedItem >= _items.Count) _selectedItem = 0;
            MoveArrow(_selectedItem);
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            var action = _items[_selectedItem].Item2;
            action();
        }
    }

    void MoveArrow(int index)
    {
        float maxWidth = _widths.Max();
        _arrow.transform.localPosition = new Vector3(
            (maxWidth - _widths[index]) / 2.0f,
            -1.5f * index - 1,
            0
        );
    }
}