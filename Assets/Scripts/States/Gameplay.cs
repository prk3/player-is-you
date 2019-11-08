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

            var gameBoard = new GameObject("map");
            gameBoard.transform.parent = gameObject.transform;
            var map = gameBoard.AddComponent<Map>();
            map.levelId = 0;

            _overlay = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _overlay.name = "overlay";
            _overlay.SetActive(false);
            _overlay.transform.parent = gameObject.transform;
            _overlay.transform.localPosition = new Vector3(0, 0, -1);
            _overlay.transform.localScale = new Vector3(2, 1, 2);
        
            var rotation = _overlay.transform.localRotation.eulerAngles;
            rotation.x = -90;
            _overlay.transform.localRotation = Quaternion.Euler(rotation);
        
            _overlay.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 0.5f);
            _overlay.GetComponent<MeshRenderer>().material.shader = Shader.Find("UI/Default");

            _menu = new GameObject("quick menu");
            _menu.SetActive(false);
            _menu.transform.parent = gameObject.transform;
            _menu.transform.localPosition += Vector3.back + Vector3.back;
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
