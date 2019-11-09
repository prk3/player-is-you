using UnityEngine;

namespace States
{
    public class Gameplay : MonoBehaviour
    {
        private StateTransition _transition;
        private bool _paused;
        private GameObject _overlay;
        private GameObject _menu;
        private GameObject _lose;

        private bool _redirecting;
        private float _redirectingTime;
        private float _redirectingDuration = 3f;
        
        void Start()
        {
            _transition = gameObject.AddComponent<StateTransition>();

            var gameBoard = new GameObject("map");
            gameBoard.transform.parent = gameObject.transform;
            var map = gameBoard.AddComponent<Map>();
            map.levelId = GameStore.Level;

            _overlay = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _overlay.name = "overlay";
            _overlay.SetActive(false);
            _overlay.transform.parent = gameObject.transform;
            _overlay.transform.localPosition = new Vector3(0, 0, -1);
            _overlay.transform.localScale = new Vector3(3, 1, 3);
        
            var rotation = _overlay.transform.localRotation.eulerAngles;
            rotation.x = -90;
            _overlay.transform.localRotation = Quaternion.Euler(rotation);
        
            _overlay.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 0.5f);
            _overlay.GetComponent<MeshRenderer>().material.shader = Shader.Find("UI/Default");

            _menu = new GameObject("quick menu");
            _menu.SetActive(false);
            _menu.transform.parent = gameObject.transform;
            _menu.transform.localPosition += Vector3.back * 2.0f  + Vector3.up * 2.0f;

            var menuComp = _menu.AddComponent<Menu>();
            menuComp.AddItem("Continue", CloseQuickMenu);
            menuComp.AddItem("Restart", Restart);
            menuComp.AddItem("Exit", Exit);
        }

        void Update()
        {
            if (_redirecting)
            {
                _redirectingTime += Time.deltaTime;

                if (_redirectingTime > _redirectingDuration)
                {
                    _redirecting = false;
                    _transition.TransitionTo("LevelSelectMenu");
                }
            }

            else if (_transition.IsStateActive())
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

        public bool IsPlaying()
        {
            return !_paused;
        }

        public void Restart()
        {
            _transition.TransitionTo("Gameplay");
        }

        private void Exit()
        {
            _transition.TransitionTo("StartMenu");
        }

        public void Lose()
        {
            _paused = true;
            _overlay.SetActive(true);
            
            var lose = new GameObject("win");
            lose.transform.parent = gameObject.transform;
            lose.transform.localPosition += Vector3.back * 2.0f  + Vector3.up * 2.0f;

            var loseText = lose.AddComponent<AnimatedText>();
            loseText.text = "You Lost. Try Again!";
            loseText.align = Align.Center;

            GameStore.SelectedLevel = GameStore.Level;
            _redirecting = true;
            _redirectingTime = 0;
        }

        public void Win()
        {
            _paused = true;
            _overlay.SetActive(true);

            var unlockedLevels = PlayerPrefs.GetString("unlocked_levels");
            if (GameStore.Level + 1 < unlockedLevels.Length && unlockedLevels[GameStore.Level + 1] == '0')
            {
                PlayerPrefs.SetString("unlocked_levels",
                    unlockedLevels.Substring(0, GameStore.Level + 1) +
                    '1' +
                    unlockedLevels.Substring(GameStore.Level + 2));
                PlayerPrefs.Save();
            }
            
            var win = new GameObject("win");
            win.transform.parent = gameObject.transform;
            win.transform.localPosition += Vector3.back * 2.0f  + Vector3.up * 2.0f;

            var winText = win.AddComponent<AnimatedText>();
            winText.text = $"You completed level ${GameStore.Level + 1}!";
            winText.align = Align.Center;

            GameStore.SelectedLevel = GameStore.Level + 1 >= unlockedLevels.Length ? -1 : GameStore.Level + 1;
            _redirecting = true;
            _redirectingTime = 0;
        }

        public void OpenQuickMenu()
        {
            _paused = true;
            _overlay.SetActive(true);
            _menu.SetActive(true);
        }

        private void CloseQuickMenu()
        {
            _paused = false;
            _overlay.SetActive(false);
            _menu.SetActive(false);
        }
    }
}
