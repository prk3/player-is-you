using UnityEngine;

namespace Traits
{
    public class You : Trait
    {
        private Subject _subject;
    
        void Start()
        {
            _subject = gameObject.GetComponent<Subject>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!_subject.IsMoving())
            {
                int up = Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
                int down = Input.GetKey(KeyCode.DownArrow) ? 1 : 0;
                int right = Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
                int left = Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;

                // only one is pressed
                if (up + down + right + left == 1)
                {
                    Map m = gameObject.transform.parent.gameObject.GetComponent<Map>();
                    Subject e = _subject;

                    if (up > 0 && e.y > 0)
                    {
                        m.spots[e.y][e.x].Remove(e);
                        m.spots[e.y - 1][e.x].Add(e);
                        e.MoveTo(new Vector2(e.x, e.y - 1));
                    }
                    else if (down > 0 && e.y < m.spots.Length - 1)
                    {
                        m.spots[e.y][e.x].Remove(e);
                        m.spots[e.y + 1][e.x].Add(e);
                        e.MoveTo(new Vector2(e.x, e.y + 1));
                    }
                    else if (left > 0 && e.x > 0)
                    {
                        m.spots[e.y][e.x].Remove(e);
                        m.spots[e.y][e.x - 1].Add(e);
                        e.MoveTo(new Vector2(e.x - 1, e.y));
                    }
                    else if (right > 0 && e.x < m.spots[0].Length - 1)
                    {
                        m.spots[e.y][e.x].Remove(e);
                        m.spots[e.y][e.x + 1].Add(e);
                        e.MoveTo(new Vector2(e.x + 1, e.y));
                    }
                }
            }
        }
    }
}

