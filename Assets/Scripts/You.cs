using UnityEngine;
using UnityEngine.Experimental.U2D;

public class You : MonoBehaviour
{
    private Entity e;
    
    void Start()
    {
        e = gameObject.GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!e.IsMoving())
        {
            bool up = Input.GetKeyDown(KeyCode.UpArrow);
            bool down = Input.GetKeyDown(KeyCode.DownArrow);
            bool right = Input.GetKeyDown(KeyCode.RightArrow);
            bool left = Input.GetKeyDown(KeyCode.LeftArrow);

            if (up | down | right | left)
            {
                Map m = gameObject.transform.parent.gameObject.GetComponent<Map>();

                if (up && e.y > 0)
                {
                    m.spots[e.y][e.x].Remove(e);
                    m.spots[e.y - 1][e.x].Add(e);
                    e.MoveTo(new Vector2(e.x, e.y - 1));
                }
                else if (down && e.y < m.spots.Length - 1)
                {
                    m.spots[e.y][e.x].Remove(e);
                    m.spots[e.y + 1][e.x].Add(e);
                    e.MoveTo(new Vector2(e.x, e.y + 1));
                }
                else if (left && e.x > 0)
                {
                    m.spots[e.y][e.x].Remove(e);
                    m.spots[e.y][e.x - 1].Add(e);
                    e.MoveTo(new Vector2(e.x - 1, e.y));
                }
                else if (right && e.x < m.spots[0].Length - 1)
                {
                    m.spots[e.y][e.x].Remove(e);
                    m.spots[e.y][e.x + 1].Add(e);
                    e.MoveTo(new Vector2(e.x + 1, e.y));
                }
            }
        }
    }
}
