using UnityEngine;

namespace States
{
    public class Gameplay : MonoBehaviour
    {
        private StateTransition _transition;
        private bool _quickMenuOpened;
        private GameObject _overlay;
        private GameObject _menu;
        
        void Start()
        {
            _transition = gameObject.AddComponent<StateTransition>();
            
            var gameBoard = new GameObject();
            gameBoard.transform.parent = gameObject.transform;

            var map = gameBoard.AddComponent<Map>();
            map.levelId = 0;
            
            _overlay = new GameObject();
            _overlay.SetActive(false);
            _overlay.transform.parent = gameObject.transform;
            _overlay.transform.localPosition += Vector3.forward;
            _overlay.transform.localScale = Vector3.one * 30;

            var overlayRen = _overlay.AddComponent<SpriteRenderer>();
            overlayRen.sprite = Sprite.Create(
                Resources.Load<Texture2D>("Textures/overlay"),
                new Rect(0, 0, 1, 1), 
                new Vector2(0.5f, 0.5f));

            _menu = new GameObject();
            _menu.SetActive(false);
            var menuComp = _menu.AddComponent<Menu>();
            menuComp.AddItem("Continue", CloseQuickMenu);
            menuComp.AddItem("Restart", Restart);
            menuComp.AddItem("Skip level", Skip);
            menuComp.AddItem("Exit", Exit);
        }

        void Update()
        {
            if (_transition.IsStateActive())
            {
                if (_quickMenuOpened)
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
            return !_quickMenuOpened;
        }

        public void Restart()
        {
            _transition.TransitionTo("Gameplay");
        }

        private void Skip()
        {
            _transition.TransitionTo("Gameplay");
        }

        private void Exit()
        {
            _transition.TransitionTo("StartMenu");
        }

        public void Lose()
        {
            Debug.Log("Lost");
        }

        public void Win()
        {
            Debug.Log("WON");
        }

        public void OpenQuickMenu()
        {
            _overlay.SetActive(true);
            _menu.SetActive(true);
            _quickMenuOpened = true;
        }

        private void CloseQuickMenu()
        {
            _overlay.SetActive(false);
            _menu.SetActive(false);
            _quickMenuOpened = false;
        }
    }
}
