using Entities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace States
{
    public class StartMenu : MonoBehaviour
    {
        private StateTransition _transition;
        private static Texture2D _controlsTexture;
        private static Texture2D _entitiesTexture;

        // ltr string with level progress - 1 indicates unlocked level, 0 - locked
        public string defaultUnlockedLevels = "1000";

        void Awake()
        {
            if (!PlayerPrefs.HasKey("unlocked_levels"))
            {
                PlayerPrefs.SetString("unlocked_levels", defaultUnlockedLevels);
                PlayerPrefs.Save();
            }

            if (_controlsTexture == null)
            {
                _controlsTexture = Resources.Load<Texture2D>("Textures/controls");
                _controlsTexture.filterMode = FilterMode.Point;
            }

            if (_entitiesTexture == null)
            {
                _entitiesTexture = Resources.Load<Texture2D>("Textures/entities");
                _entitiesTexture.filterMode = FilterMode.Point;
            }

        }

        void Start()
        {
            var player = new GameObject("player");
            DontDestroyOnLoad(player);
            player.AddComponent<AudioPlayer>();

            _transition = gameObject.AddComponent<StateTransition>();

            {
                var title = new GameObject("title");
                title.transform.parent = gameObject.transform;
                title.transform.localPosition = new Vector3(0, 4f, 0);
                title.transform.localScale = Vector3.one * 1.5f;
                var titleText = title.AddComponent<AnimatedText>();
                titleText.text = "player is you";
                titleText.align = Align.Center;
            }

            {
                var menu = new GameObject("menu");
                menu.transform.localPosition = new Vector3(0, 1f, 0);
                menu.transform.parent = gameObject.transform;

                var menuComp = menu.AddComponent<Menu>();
                menuComp.AddItem("Play", () => _transition.TransitionTo("LevelSelectMenu"));
                menuComp.AddItem("Quit", () => Application.Quit());
            }

            {
                var controls = new GameObject("controls");
                controls.transform.parent = gameObject.transform;

                var controlRen = controls.AddComponent<SpriteRenderer>();
                controlRen.sprite = Sprite.Create(
                    _controlsTexture,
                    new Rect(0, 0, 96, 64),
                    new Vector2(0, 0),
                    32);

                var controlAnim = controls.AddComponent<AnimatedSprite>();
                controlAnim.numberOfSprites = 8;
                controls.transform.localPosition = new Vector3(-2.5f, -4.5f, 1);
            }

            {
                var cIsM = MakeFakeEntity(EntityType.ConnectorIs);
                cIsM.transform.parent = gameObject.transform;
                cIsM.transform.localPosition = new Vector3(0.5f, -4.5f, 0);
            }

            {
                var move = MakeFakeEntity(EntityType.TraitMove);
                move.transform.parent = gameObject.transform;
                move.transform.localPosition = new Vector3(1.5f, -4.5f, 0);
            }

            {
                var noun = MakeFakeEntity(EntityType.SubjectNoun);
                noun.transform.parent = gameObject.transform;
                noun.transform.localPosition = new Vector3(-7f, -4.5f, 0);
            }

            {
                var nIsV = MakeFakeEntity(EntityType.ConnectorIs);
                nIsV.transform.parent = gameObject.transform;
                nIsV.transform.localPosition = new Vector3(-6f, -4.5f, 0);
            }

            {
                var verb = MakeFakeEntity(EntityType.TraitVerb);
                verb.transform.parent = gameObject.transform;
                verb.transform.localPosition = new Vector3(-5f, -4.5f, 0);
            }

            {
                var you = MakeFakeEntity(EntityType.SubjectGoal);
                you.transform.parent = gameObject.transform;
                you.transform.localPosition = new Vector3(4f, -4.5f, 0);
            }

            {
                var yIsW = MakeFakeEntity(EntityType.ConnectorIs);
                yIsW.transform.parent = gameObject.transform;
                yIsW.transform.localPosition = new Vector3(5f, -4.5f, 0);
            }

            {
                var win = MakeFakeEntity(EntityType.TraitWin);
                win.transform.parent = gameObject.transform;
                win.transform.localPosition = new Vector3(6f, -4.5f, 0);
            }
        }

        private GameObject MakeFakeEntity(EntityType type)
        {
            var obj = new GameObject(type.ToString());

            var ren = obj.AddComponent<SpriteRenderer>();
            ren.sprite = Sprite.Create(
                _entitiesTexture,
                new Rect(32 * ((int)type - 1), 0, 32, 32),
                new Vector2(0, 0),
                32);

            obj.AddComponent<AnimatedSprite>();

            return obj;
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
