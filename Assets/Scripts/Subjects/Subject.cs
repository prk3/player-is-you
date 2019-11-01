﻿using System;
using System.Collections.Generic;
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
        // will be initialized in Start
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
    
        private SubjectType _type;
        private int _y;
        private int _x;
        private int _z;

        private bool _moving;
        private Vector2 _moveFrom;
        private Vector2 _moveTo;
        private float _moveTime;
        private float _moveDuration;

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

        public SubjectType GetEntityType()
        {
            return _type;
        }

        public void SetEntityType(SubjectType type)
        {
            _type = type;
        }

        public static Texture2D GetTexture()
        {
            if (_texture != null) return _texture;
            _texture = Resources.Load<Texture2D>("Textures/entities");
            _texture.filterMode = FilterMode.Point;

            return _texture;
        }

        public static (int, int) DirectionToVector(MoveDirection dir)
        {
            switch (dir)
            {
                case MoveDirection.Up:    return ( 0, -1);
                case MoveDirection.Down:  return ( 0,  1);
                case MoveDirection.Left:  return (-1,  0);
                case MoveDirection.Right: return ( 1,  0);
                default:                  return ( 0,  0);
            }
        }

        public bool IsMoving()
        {
            return _moving;
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
    
        void Start()
        {
            var r = gameObject.AddComponent<SpriteRenderer>();
            r.sprite = Sprite.Create(
                GetTexture(),
                new Rect(32 * (int) (_type-1), 0, 32, 32),
                new Vector2(0, 0),
                32
            );
            gameObject.AddComponent<AnimatedSprite>();
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

        public void TransitionPosition((int, int) newPosition, float time = 0.18f)
        {
            _moveFrom = new Vector2(x, y);
            _moveTo = new Vector2(newPosition.Item1, newPosition.Item2);
            _moveDuration = time;
            _moveTime = 0;
            _moving = true;
        }
    }
}