using UnityEngine;

namespace States
{
    public class LevelSelectMenu : MonoBehaviour
    {
        void Start()
        {
            var transition = gameObject.AddComponent<StateTransition>();

            var menu = new GameObject("menu");
            menu.transform.parent = gameObject.transform;
            menu.transform.Translate(0, 3, 0);

            var menuComp = menu.AddComponent<Menu>();
            var unlockedLevels = PlayerPrefs.GetString("unlocked_levels");

            void PlayLevel(int level)
            {
                GameStore.Level = level;
                transition.TransitionTo("Gameplay");
            }

            bool IsLevelLocked(int level)
            {
                return unlockedLevels[level] == '0';
            }
            
            menuComp.AddItem("Level 1", () => PlayLevel(0), IsLevelLocked(0));
            menuComp.AddItem("Level 2", () => PlayLevel(1), IsLevelLocked(1));
            menuComp.AddItem("Level 3", () => PlayLevel(2), IsLevelLocked(2));
            menuComp.AddItem("Level 4", () => PlayLevel(3), IsLevelLocked(3));
            menuComp.AddItem("Back", () => transition.TransitionTo("StartMenu"));
        }
    }
}
