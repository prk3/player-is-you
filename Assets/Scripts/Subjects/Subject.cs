using System;
using System.Collections.Generic;
using System.Linq;
using Traits;
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
        
        public void AnimateMove(Vector2Int newPosition, float time = 0.18f)
        {
            _moveFrom = new Vector2(x, y);
            _moveTo = newPosition;
            _moveDuration = time;
            _moveTime = 0;
            _moving = true;
        }
        
        public bool CanMove()
        {
            return !_moving;
        }

        public bool CanMoveTo(MoveDirection dir)
        {
            Map map = gameObject.GetComponentInParent<Map>();
            var thisPos = new Vector2Int(x, y);
            var to = thisPos + DirectionToVector(dir);
            
            if (!map.IsValidSpot(to)) return false;
            List<Subject> newStack = map.stacks[to.y][to.x];
            
            return newStack.TrueForAll(
                subject => subject.GetComponents<Trait>().ToList().TrueForAll(
                    trait => trait.CanEnter(this, dir)));
        }

        public void MoveTo(MoveDirection dir, Action<Subject> registerMove)
        {
            Map map = gameObject.GetComponentInParent<Map>();
            var thisPos = new Vector2Int(x, y);
            var to = thisPos + DirectionToVector(dir);

            List<Subject> oldStack = map.stacks[y][x];
            List<Subject> newStack = map.stacks[to.y][to.x];
            z = newStack.Count == 0 ? 0 : newStack.First().z + 1;

            oldStack.Remove(this);
            int newStackPosition = 0;
            newStack.Insert(newStackPosition, this);
            registerMove(this);

            for (int i = 1; i < newStack.Count; i++)
            {
                var subject = newStack[i];
                foreach (var trait in subject.GetComponents<Trait>().OrderByDescending(t => t.GetInteractionOrder()))
                {
                    var outcome = trait.OnEnter(this, dir, registerMove);
                    switch (outcome)
                    {
                        case OnEnterOutcome.PullDown:
                            newStack.RemoveAt(newStackPosition);
                            newStackPosition = i;
                            newStack.Insert(newStackPosition, this);
                            break;
                        case OnEnterOutcome.Break:
                            goto afterSubjectLoop;
                        case OnEnterOutcome.Continue:
                            break;
                    }
                }
                afterTraitLoop: ;
            }
            afterSubjectLoop: ;

            AnimateMove(to);
            map.UpdateRules();
        }

        public void AfterMove()
        {
            Map map = gameObject.GetComponentInParent<Map>();

            List<Subject> stack = map.stacks[y][x];

            int pos = stack.FindIndex(subject => subject == this);

            if (pos >= 0)
            {
                for (int i = pos + 1; i < stack.Count; i++)
                {
                    var subject = stack[i];
                    foreach (var trait in subject.GetComponents<Trait>().OrderByDescending(t => t.GetInteractionOrder()))
                    {
                        var outcome = trait.AfterEnter();
                        switch (outcome)
                        {
                            case AfterEnterOutcome.Break:
                                goto afterSubjectLoop;
                            case AfterEnterOutcome.Continue:
                                break;
                        }
                    }
                    afterTraitLoop: ;
                }
                afterSubjectLoop: ;
            }
        }

        public void Refresh()
        {
            var modComp = gameObject.GetComponent<TileMod>();
            if (modComp)
            {
                var map = gameObject.GetComponentInParent<Map>();
                modComp.ApplyMod(map.CollectNeighborsByte(this, true));
            }
        }
    }
}