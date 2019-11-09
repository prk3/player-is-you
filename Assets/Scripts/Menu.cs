using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Menu : MonoBehaviour
{
    private struct Item
    {
        public readonly string Title;
        public readonly Action Action;
        public readonly bool Locked;

        public Item(string title, Action action, bool locked)
        {
            Title = title;
            Action = action;
            Locked = locked;
        }
    }
    private StateTransition _transition;
    private GameObject _arrow;
    private List<Item> _items = new List<Item>();
    private List<float> _widths = new List<float>();
    private int _selectedItem;
    
    void Start()
    {
        _transition = gameObject.GetComponent<StateTransition>() ?? gameObject.GetComponentInParent<StateTransition>();

        for (int i = 0; i < _items.Count; i++)
        {
            var obj = new GameObject();
            obj.transform.parent = gameObject.transform;
            obj.transform.localPosition = new Vector3(0, -1.5f * i, 0);

            var textComponent = obj.AddComponent<AnimatedText>();
            textComponent.text = _items[i].Title;
            textComponent.align = Align.Center;

            if (_items[i].Locked)
            {
                textComponent.color = new Color(0.3f, 0.3f, 0.3f, 1);
            }

            _widths.Add(textComponent.GetWidth());
        }

        _arrow = new GameObject();
        _arrow.transform.parent = gameObject.transform;
        MoveArrow(_selectedItem);

        var tex = Resources.Load<Texture2D>("Textures/arrow");
        tex.filterMode = FilterMode.Point;

        var ren = _arrow.AddComponent<SpriteRenderer>();
        ren.sprite = Sprite.Create(
            tex,
            new Rect(0, 0, 32, 32),
            new Vector2(0, 0),
            32
        );

        _arrow.AddComponent<AnimatedSprite>();
    }

    void Update()
    {
        if (_transition.IsStateActive())
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                var oldItem = _selectedItem;

                do
                {
                    _selectedItem = (_items.Count + _selectedItem - 1) % _items.Count;
                } while (_selectedItem != oldItem && _items[_selectedItem].Locked);
                
                MoveArrow(_selectedItem);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                var oldItem = _selectedItem;

                do
                {
                    _selectedItem = (_selectedItem + 1) % _items.Count;
                } while (_selectedItem != oldItem && _items[_selectedItem].Locked);
                
                MoveArrow(_selectedItem);
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                var action = _items[_selectedItem].Action;
                action();
            }
        }
    }
    
    public void AddItem(string text, Action d, bool locked = false)
    {
        _items.Add(new Item(text, d, locked));
    }

    public void SelectItem(int index)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(index < _items.Count);
        Debug.Assert(_items[index].Locked == false);
        _selectedItem = index;
    }

    void MoveArrow(int index)
    {
         _arrow.transform.localPosition = new Vector3(
            (_widths[index] / -2.0f) - 2.0f,
            -1.5f * index - 1,
            0);
    }
}