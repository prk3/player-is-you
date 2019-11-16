using UnityEngine;

namespace States
{
    public class Gameplay : MonoBehaviour
    {
        private StateTransition _transition;
        private bool _paused;
        private GameObject _overlay;
        private GameObject _quickMenu;
        private GameObject _lose;

        void Start()
        {
            _transition = gameObject.AddComponent<StateTransition>();

            {
                var gameBoard = new GameObject("map");
                gameBoard.transform.parent = gameObject.transform;
                var map = gameBoard.AddComponent<Map>();
                map.levelId = GameStore.Level;
            }

            {
                _overlay = GameObject.CreatePrimitive(PrimitiveType.Plane);
                _overlay.name = "overlay";
                _overlay.SetActive(false);
                _overlay.transform.parent = gameObject.transform;
                _overlay.transform.localPosition = new Vector3(0, 0, -1);
                _overlay.transform.localScale = new Vector3(3, 1, 3);

                var rotation = _overlay.transform.localRotation.eulerAngles;
                rotation.x = -90;
                _overlay.transform.localRotation = Quaternion.Euler(rotation);

                _overlay.GetComponent<MeshRenderer>().material = Resources.Load<Material>("pause_menu_overlay");
            }

            {
                _quickMenu = new GameObject("quick menu");
                _quickMenu.SetActive(false);
                _quickMenu.transform.parent = gameObject.transform;
                _quickMenu.transform.localPosition = new Vector3(0, 2, -2);

                var menuComp = _quickMenu.AddComponent<Menu>();
                menuComp.AddItem("Continue", CloseQuickMenu);
                menuComp.AddItem("Restart", Restart);
                menuComp.AddItem("Exit", Exit);
            }
        }

        void Update()
        {
            if (_transition.IsStateActive())
            {
                if (_paused)
                {
                    if (Input.GetKeyDown(KeyCode.Escape)) CloseQuickMenu();
                    if (Input.GetKeyDown(KeyCode.R)) Restart();
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Escape)) OpenQuickMenu();
                    if (Input.GetKeyDown(KeyCode.R)) Restart();
                }
            }
        }

        /**
         * Whether user in in control of the map.
         */
        public bool IsPlaying()
        {
            return !_paused;
        }

        /**
         * Restarts currently shown level.
         */
        public void Restart()
        {
            _transition.TransitionTo("Gameplay");
        }

        /**
         * Exit to start menu.
         */
        private void Exit()
        {
            _transition.TransitionTo("StartMenu");
        }

        /**
         * Show lose message and redirect to level select menu.
         */
        public void Lose()
        {
            _paused = true;
            _overlay.SetActive(true);

            {
                var lose = new GameObject("lose");
                lose.transform.parent = gameObject.transform;
                lose.transform.localPosition = new Vector3(0, 3, -2);

                var loseText = lose.AddComponent<AnimatedText>();
                loseText.text = "You Lost. Try Again!";
                loseText.align = Align.Center;
            }

            {
                var loseMenu = new GameObject("lose menu");
                loseMenu.transform.parent = gameObject.transform;
                loseMenu.transform.localPosition = new Vector3(0, 0, -2);

                var menu = loseMenu.AddComponent<Menu>();

                menu.AddItem("Restart", () => _transition.TransitionTo("Gameplay"));
                menu.AddItem("Exit", () => _transition.TransitionTo("StartMenu"));
            }
        }

        /**
         * Show won message and redirect to level select menu (if possible puts arrow on next level).
         */
        public void Win()
        {
            _paused = true;
            _overlay.SetActive(true);

            var unlockedLevels = PlayerPrefs.GetString("unlocked_levels");
            var hasNextLevel = GameStore.Level + 1 < unlockedLevels.Length;

            if (hasNextLevel)
            {
                var isNextLevelLocked = unlockedLevels[GameStore.Level + 1] == '0';

                if (isNextLevelLocked)
                {
                    PlayerPrefs.SetString("unlocked_levels",
                        unlockedLevels.Substring(0, GameStore.Level + 1) +
                        '1' +
                        unlockedLevels.Substring(GameStore.Level + 2));
                    PlayerPrefs.Save();
                }
            }

            {
                var win = new GameObject("win");
                win.transform.parent = gameObject.transform;
                win.transform.localPosition = new Vector3(0, 3, -2);

                var winText = win.AddComponent<AnimatedText>();
                winText.text = $"You completed level ${GameStore.Level + 1}!";
                winText.align = Align.Center;
            }

            {
                var winMenu = new GameObject("win menu");
                winMenu.transform.parent = gameObject.transform;
                winMenu.transform.localPosition = new Vector3(0, 0, -2);

                var menu = winMenu.AddComponent<Menu>();

                if (hasNextLevel)
                {
                    menu.AddItem("Next level", () =>
                    {
                        GameStore.Level++;
                        _transition.TransitionTo("Gameplay");
                    });
                }
                menu.AddItem("Exit", () => _transition.TransitionTo("StartMenu"));
            }
        }

        public void OpenQuickMenu()
        {
            _paused = true;
            _overlay.SetActive(true);
            _quickMenu.SetActive(true);
            AudioPlayer.PlaySound("pause");
        }

        private void CloseQuickMenu()
        {
            _paused = false;
            _overlay.SetActive(false);
            _quickMenu.SetActive(false);
        }
    }
}
