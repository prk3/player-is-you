using UnityEngine;

namespace States
{
    public class MainMenu : MonoBehaviour
    {
        void Start()
        {
            var transition = gameObject.AddComponent<StateTransition>();
        
            var obj = new GameObject();
            obj.transform.parent = gameObject.transform;

            var menu = obj.AddComponent<Menu>();
            menu.AddItem("Play", () => transition.TransitionTo("GameplayScene"));
            menu.AddItem("Quit", () => Application.Quit());
        }
    }
}
