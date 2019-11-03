using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Subjects
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum SubjectType
    {
        // decoration blocks
        DecorationBush = 1,
        DecorationTile,
        DecorationUnknown,
    
        // is connector in "[subject] is [trait]" rules
        ConnectorIs,
    
        // regular entities
        Player,
        Rock,
        Water,
        Wall,
        Flag,
        Skull,
        Cloud,
    
        // subjects in "[subject] is [trait]" rules
        SubjectPlayer,
        SubjectRock,
        SubjectWater,
        SubjectWall,
        SubjectFlag,
        SubjectSkull,
        SubjectCloud,
    
        // traits in "[subject] is [trait]" rules
        TraitYou,
        TraitPush,
        TraitSink,
        TraitStop,
        TraitWin,
        TraitLose,
        TraitFloat,
    }

    public class Subject : MonoBehaviour
    {
        // will be initialized in Awake
        private static Texture2D _texture;
    
        private static readonly Dictionary<SubjectType, SubjectType> SubjectToEntityMap = new Dictionary<SubjectType,SubjectType>
        {
            { SubjectType.SubjectPlayer, SubjectType.Player },
            { SubjectType.SubjectRock,   SubjectType.Rock },
            { SubjectType.SubjectWater,  SubjectType.Water },
            { SubjectType.SubjectWall,   SubjectType.Wall },
            { SubjectType.SubjectFlag,   SubjectType.Flag },
            { SubjectType.SubjectSkull,  SubjectType.Skull },
            { SubjectType.SubjectCloud,  SubjectType.Cloud }
        };
    
        private static readonly Dictionary<SubjectType, Type> TraitToBehaviorMap = new Dictionary<SubjectType, Type>
        {
            { SubjectType.TraitYou, typeof(Traits.You) },
            { SubjectType.TraitPush, typeof(Traits.Push) },
            { SubjectType.TraitSink, typeof(Traits.Sink) },
            { SubjectType.TraitStop, typeof(Traits.Stop) },
            { SubjectType.TraitWin, typeof(Traits.Win) },
            { SubjectType.TraitLose, typeof(Traits.Lose) },
            { SubjectType.TraitFloat, typeof(Traits.Float) }
        };

        // will be initialized in Awake
        private static Dictionary<SubjectType, Texture2D> _subjectTypeToModTilemap;
        
        public static Texture2D GetTexture()
        {
            return _texture;
        }

        public static Texture2D GetModTilemap(SubjectType type)
        {
            _subjectTypeToModTilemap.TryGetValue(type, out var output);
            return output;
        }
        
        public static Vector2Int DirectionToVector(MoveDirection dir)
        {
            switch (dir)
            {
                // remember than our map has y growing downwards
                case MoveDirection.Up:     return Vector2Int.down;
                case MoveDirection.Down:   return Vector2Int.up;
                case MoveDirection.Left:   return Vector2Int.left;
                case MoveDirection.Right:  return Vector2Int.right;
                default:                   return Vector2Int.zero;
            }
        }
        
        public static bool IsSubject(SubjectType t)
        {
            return SubjectToEntityMap.ContainsKey(t);
        }

        public static SubjectType GetSubjectType(SubjectType t)
        {
            return SubjectToEntityMap[t];
        }

        public static bool IsTrait(SubjectType t)
        {
            return TraitToBehaviorMap.ContainsKey(t);
        }
    
        public static Type GetTraitBehavior(SubjectType t)
        {
            return TraitToBehaviorMap[t];
        }
    
        private SubjectType _type;
        private int _y;
        private int _x;
        private int _z;

        private bool _moving;
        private Vector2 _moveFrom;
        private Vector2 _moveTo;
        private float _moveTime;
        private float _moveDuration;

        void Awake()
        {
            if (_texture == null)
            {
                _texture = Resources.Load<Texture2D>("Textures/subjects");
                _texture.filterMode = FilterMode.Point;
            }
            
            if (_subjectTypeToModTilemap == null)
            {
                _subjectTypeToModTilemap = new Dictionary<SubjectType, Texture2D>()
                {
                    { SubjectType.Water, Resources.Load<Texture2D>("Textures/water_mod") },
                    { SubjectType.Wall,  Resources.Load<Texture2D>("Textures/wall_mod") }
                };
            }
        }
        
        void Start()
        {
            gameObject.transform.localPosition = new Vector3(x, -y - 1, 0);
        }
    
        void Update()
        {
            if (_moving)
            {
                _moveTime += Time.deltaTime;
                if (_moveTime > _moveDuration)
                {
                    _moving = false;
                    x = (int)_moveTo.x;
                    y = (int)_moveTo.y;
                }
                else
                {
                    // ease-in based on y-flipped parabola
                    float PositionFunc(float x) => x * (2 - x);

                    var tmpPos = _moveFrom + ((_moveTo - _moveFrom) * (PositionFunc(_moveTime / _moveDuration)));
                    gameObject.transform.localPosition = new Vector3(tmpPos.x, -tmpPos.y - 1, 0);
                }
            }
        }
        
        public int x
        {
            get { return _x; }
            set
            {
                _x = value;
                gameObject.transform.localPosition = new Vector3(value, -_y - 1, 0);
            }
        }
    
        public int y
        {
            get { return _y; }
            set
            {
                _y = value;
                gameObject.transform.localPosition = new Vector3(_x, -value - 1, 0);
            }
        }

        public int z
        {
            get { return _z; }
            set
            {
                _z = value;
                var r = gameObject.GetComponent<SpriteRenderer>();
                if (r)
                {
                    r.sortingOrder = value;
                }
            }
        }

        public SubjectType GetSubjectType()
        {
            return _type;
        }

        public void SetEntityType(SubjectType type)
        {
            _type = type;
        }
        
        public void Move(Vector2Int newPosition, float time = 0.18f)
        {
            _moveFrom = new Vector2(x, y);
            _moveTo = newPosition;
            _moveDuration = time;
            _moveTime = 0;
            _moving = true;
        }
        
        public bool IsMoving()
        {
            return _moving;
        }
    }
}