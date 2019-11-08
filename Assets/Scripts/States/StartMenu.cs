using UnityEngine;

namespace States
{
    public class StartMenu : MonoBehaviour
    {
        void Awake()
        {
            if (!PlayerPrefs.HasKey("unlocked_levels"))
            {
                PlayerPrefs.SetString("unlocked_levels", "1000");
                PlayerPrefs.Save();
            }
        }
        
        void Start()
        {
            var transition = gameObject.AddComponent<StateTransition>();

            var title = new GameObject("title");
            title.transform.parent = gameObject.transform;
            title.transform.Translate(0, 3, 0);
            title.transform.localScale = Vector3.one * 1.5f;
            var titleText = title.AddComponent<AnimatedText>();
            titleText.text = "player is you";
            titleText.align = Align.Center;

            var menu = new GameObject("menu");
            menu.transform.parent = gameObject.transform;

            var menuComp = menu.AddComponent<Menu>();
            menuComp.AddItem("Play", () => transition.TransitionTo("LevelSelectMenu"));
            menuComp.AddItem("Quit", () => Application.Quit());
        }
    }
}
