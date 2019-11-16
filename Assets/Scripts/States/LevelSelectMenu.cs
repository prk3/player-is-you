using UnityEngine;

namespace States
{
    public class LevelSelectMenu : MonoBehaviour
    {
        void Start()
        {
            var transition = gameObject.AddComponent<StateTransition>();

            var unlockedLevels = PlayerPrefs.GetString("unlocked_levels");
            int lastUnlocked = unlockedLevels.LastIndexOf('1');
            var selectedLevel = lastUnlocked == -1 || lastUnlocked == unlockedLevels.Length - 1
                ? 0
                : lastUnlocked;

            var menu = new GameObject("menu");
            menu.transform.parent = gameObject.transform;
            menu.transform.Translate(0, 3, 0);

            var menuComp = menu.AddComponent<Menu>();

            void PlayLevel(int level)
            {
                GameStore.Level = level;
                transition.TransitionTo("Gameplay");
            }

            bool IsLevelLocked(int level)
            {
                return unlockedLevels[level] == '0';
            }

            menuComp.AddItem("Rock and roll", () => PlayLevel(0), IsLevelLocked(0));
            menuComp.AddItem("River of deceit", () => PlayLevel(1), IsLevelLocked(1));
            menuComp.AddItem("Prison song", () => PlayLevel(2), IsLevelLocked(2));
            menuComp.AddItem("Smoke on the water", () => PlayLevel(3), IsLevelLocked(3));
            menuComp.AddItem("Back", () => transition.TransitionTo("StartMenu"));
            menuComp.SelectItem(selectedLevel);
        }
    }
}
