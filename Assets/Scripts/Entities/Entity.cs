using System;
using System.Collections.Generic;
using System.Linq;
using Traits;
using UnityEngine;

namespace Entities
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum EntityType
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

    public class Entity : MonoBehaviour
    {
        // will be initialized in Awake
        private static Texture2D _texture;

        private static readonly Dictionary<EntityType, EntityType> SubjectToEntityMap = new Dictionary<EntityType,EntityType>
        {
            { EntityType.SubjectPlayer, EntityType.Player },
            { EntityType.SubjectRock,   EntityType.Rock },
            { EntityType.SubjectWater,  EntityType.Water },
            { EntityType.SubjectWall,   EntityType.Wall },
            { EntityType.SubjectFlag,   EntityType.Flag },
            { EntityType.SubjectSkull,  EntityType.Skull },
            { EntityType.SubjectCloud,  EntityType.Cloud }
        };

        private static readonly Dictionary<EntityType, Type> TraitToBehaviorMap = new Dictionary<EntityType, Type>
        {
            { EntityType.TraitYou, typeof(You) },
            { EntityType.TraitPush, typeof(Push) },
            { EntityType.TraitSink, typeof(Sink) },
            { EntityType.TraitStop, typeof(Stop) },
            { EntityType.TraitWin, typeof(Win) },
            { EntityType.TraitLose, typeof(Lose) },
            { EntityType.TraitFloat, typeof(Float) }
        };

        // will be initialized in Awake
        private static Dictionary<EntityType, Texture2D> _entityTypeToModTilemap;

        public static Texture2D GetTexture()
        {
            return _texture;
        }

        public static Texture2D GetModTilemap(EntityType type)
        {
            _entityTypeToModTilemap.TryGetValue(type, out var output);
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

        public static bool IsSubject(EntityType t)
        {
            return SubjectToEntityMap.ContainsKey(t);
        }

        public static EntityType GetEntityTypeFromSubject(EntityType t)
        {
            return SubjectToEntityMap[t];
        }

        public static bool IsTrait(EntityType t)
        {
            return TraitToBehaviorMap.ContainsKey(t);
        }

        public static Type GetBehaviorFromTrait(EntityType t)
        {
            return TraitToBehaviorMap[t];
        }

        private EntityType _type;
        public int x;
        public int y;
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
                _texture = Resources.Load<Texture2D>("Textures/entities");
                _texture.filterMode = FilterMode.Point;
            }

            if (_entityTypeToModTilemap == null)
            {
                _entityTypeToModTilemap = new Dictionary<EntityType, Texture2D>()
                {
                    { EntityType.Water, Resources.Load<Texture2D>("Textures/water_mod") },
                    { EntityType.Wall,  Resources.Load<Texture2D>("Textures/wall_mod") }
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
                }
                else
                {
                    // ease-in based on y-flipped parabola
                    float PositionFunc(float x) => x * (2 - x);

                    var tmpPos = _moveFrom + ((_moveTo - _moveFrom) * (PositionFunc(_moveTime / _moveDuration)));
                    gameObject.transform.localPosition = new Vector3(
                        tmpPos.x,
                        -tmpPos.y - 1,
                        gameObject.transform.localPosition.z);
                }
            }
        }

        public int z
        {
            get { return _z; }
            set
            {
                _z = value;
                var currentPos = gameObject.transform.localPosition;
                gameObject.transform.localPosition = new Vector3(
                    currentPos.x,
                    currentPos.y,
                    value / 1000f);
            }
        }

        public EntityType GetEntityType()
        {
            return _type;
        }

        public void SetEntityType(EntityType type)
        {
            _type = type;
        }

        public void AnimateMove(Vector2Int from, Vector2Int to, float time = 0.18f)
        {
            _moveFrom = from;
            _moveTo = to;
            _moveDuration = time;
            _moveTime = 0;
            _moving = true;
        }

        public bool IsMoving()
        {
            return _moving;
        }

        public bool CanMoveTo(MoveDirection dir)
        {
            Map map = gameObject.GetComponentInParent<Map>();
            var thisPos = new Vector2Int(x, y);
            var to = thisPos + DirectionToVector(dir);

            if (!map.IsValidSpot(to)) return false;
            List<Entity> newStack = map.stacks[to.y][to.x];

            return newStack.TrueForAll(
                entity => entity.GetComponents<Trait>().ToList().TrueForAll(
                    trait => trait.CanEnter(this, dir)));
        }

        public void MoveTo(MoveDirection dir, Action<Entity> registerMove)
        {
            Map map = gameObject.GetComponentInParent<Map>();
            var thisPos = new Vector2Int(x, y);
            var to = thisPos + DirectionToVector(dir);

            List<Entity> oldStack = map.stacks[y][x];
            List<Entity> newStack = map.stacks[to.y][to.x];

            oldStack.Remove(this);
            int newStackPosition = 0;
            newStack.Insert(newStackPosition, this);
            registerMove(this);

            for (int i = 1; i < newStack.Count; i++)
            {
                var entity = newStack[i];
                foreach (var trait in entity.GetComponents<Trait>().OrderByDescending(t => t.GetInteractionOrder()))
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
                            goto afterEntityLoop;
                        case OnEnterOutcome.Continue:
                            break;
                    }
                }
                afterTraitLoop: ;
            }
            afterEntityLoop: ;

            x = to.x;
            y = to.y;
            AnimateMove(thisPos, to);
        }

        /*
        public void AfterMoveEarly()
        {
            Map map = gameObject.GetComponentInParent<Map>();

            List<Entity> stack = map.stacks[y][x];

            int pos = stack.FindIndex(entity => entity == this);

            if (pos >= 0)
            {
                for (int i = pos + 1; i < stack.Count; i++)
                {
                    var entity = stack[i];
                    foreach (var trait in entity.GetComponents<Trait>().OrderByDescending(t => t.GetInteractionOrder()))
                    {
                        var outcome = trait.AfterEnterEarly();
                        switch (outcome)
                        {
                            case AfterEnterOutcome.Break:
                                goto afterEntityLoop;
                            case AfterEnterOutcome.Continue:
                                break;
                        }
                    }
                    afterTraitLoop: ;
                }
                afterEntityLoop: ;
            }
        }
        */

        /*
        public void AfterMoveLate()
        {
            Map map = gameObject.GetComponentInParent<Map>();

            List<Entity> stack = map.stacks[y][x];

            int pos = stack.FindIndex(entity => entity == this);

            if (pos >= 0)
            {
                for (int i = pos + 1; i < stack.Count; i++)
                {
                    var entity = stack[i];
                    foreach (var trait in entity.GetComponents<Trait>().OrderByDescending(t => t.GetInteractionOrder()))
                    {
                        var outcome = trait.AfterEnterLate();
                        switch (outcome)
                        {
                            case AfterEnterOutcome.Break:
                                goto afterEntityLoop;
                            case AfterEnterOutcome.Continue:
                                break;
                        }
                    }
                    afterTraitLoop: ;
                }
                afterEntityLoop: ;
            }
        }
        */
    }
}
