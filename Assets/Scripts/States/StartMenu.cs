using UnityEngine;

namespace States
{
    public class StartMenu : MonoBehaviour
    {
        void Start()
        {
            var transition = gameObject.AddComponent<StateTransition>();
        
            var obj = new GameObject("menu");
            obj.transform.parent = gameObject.transform;

            var menu = obj.AddComponent<Menu>();
            menu.AddItem("Play", () => transition.TransitionTo("Gameplay"));
            menu.AddItem("Quit", () => Application.Quit());
        }
    }
}
