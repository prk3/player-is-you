using UnityEngine;

namespace States
{
    public class StartMenu : MonoBehaviour
    {
        private StateTransition _transition;

        // ltr string with level progress - 1 indicates unlocked level, 0 - locked
        public string defaultUnlockedLevels = "1000";

        void Awake()
        {
            if (!PlayerPrefs.HasKey("unlocked_levels"))
            {
                PlayerPrefs.SetString("unlocked_levels", defaultUnlockedLevels);
                PlayerPrefs.Save();
            }
        }

        void Start()
        {
            var player = new GameObject("player");
            DontDestroyOnLoad(player);
            player.AddComponent<AudioPlayer>();

            _transition = gameObject.AddComponent<StateTransition>();

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
            menuComp.AddItem("Play", () => _transition.TransitionTo("LevelSelectMenu"));
            menuComp.AddItem("Quit", () => Application.Quit());
        }

        private void Update()
        {
            if (_transition.IsStateActive())
            {
                // reset unlocked levels when d + l held
                if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.L))
                {
                    if (PlayerPrefs.GetString("unlocked_levels") != defaultUnlockedLevels)
                    {
                        PlayerPrefs.SetString("unlocked_levels", defaultUnlockedLevels);
                        PlayerPrefs.Save();
                        AudioPlayer.PlaySound("push");
                    }
                }
            }
        }
    }
}
