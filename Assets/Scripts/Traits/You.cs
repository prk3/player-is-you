using System.Collections.Generic;
using States;
using Entities;
using UnityEngine;

namespace Traits
{
    public class You : Trait
    {
        private List<Entity> _movedEntities;
        private Stack<Entity> _movingEntities;
        private StateTransition _stateTransition;
        private Gameplay _gameplay;

        void Start()
        {
            _stateTransition = gameObject.GetComponentInParent<StateTransition>();
            _gameplay = gameObject.GetComponentInParent<Gameplay>();
        }

        /**
         * Checks keyboard input and calls TryMove if necessary.
         */
        void Update()
        {
            if (!MoveFinalized())
            {
                if (MapUpdateNeeded())
                {
                    UpdateAfterMove();
                }
            }
            else if (_gameplay.IsPlaying() && _stateTransition.IsStateActive())
            {
                int up = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) ? 1 : 0;
                int down = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ? 1 : 0;
                int right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) ? 1 : 0;
                int left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) ? 1 : 0;

                // only one is pressed
                if (up + down + right + left == 1)
                {
                    if (up > 0)
                    {
                        TryMove(MoveDirection.Up);
                    }
                    else if (down > 0)
                    {
                        TryMove(MoveDirection.Down);
                    }
                    else if (left > 0)
                    {
                        TryMove(MoveDirection.Left);
                    }
                    else if (right > 0)
                    {
                        TryMove(MoveDirection.Right);
                    }
                }
            }
        }

        public override int GetInteractionOrder()
        {
            return 100;
        }

        /**
         * Tries moving entity if that's possible.
         */
        private void TryMove(MoveDirection dir)
        {
            var thisEntity = gameObject.GetComponent<Entity>();
            if (thisEntity.CanMoveTo(dir))
            {
                var startingPosition = new Vector2Int(thisEntity.x, thisEntity.y);
                _movedEntities = new List<Entity>();
                _movingEntities = new Stack<Entity>();

                void RegisterMove(Entity s)
                {
                    _movedEntities.Add(s);
                    _movingEntities.Push(s);
                }

                thisEntity.MoveTo(dir, RegisterMove);

                var positions = new List<Vector2Int>(_movedEntities.Count + 1) {startingPosition};

                for (int i = _movedEntities.Count - 1; i >= 0; i--)
                {
                    positions.Add(new Vector2Int(_movedEntities[i].x, _movedEntities[i].y));
                }

                var map = gameObject.GetComponentInParent<Map>();
                map.RefreshStacks(positions);
            }
        }

        /**
         * Whether all blocks finished moving.
         */
        private bool MoveFinalized()
        {
            return _movedEntities == null;
        }

        /**
         * Whether map update is needed because all blocks stopped moving.
         */
        private bool MapUpdateNeeded()
        {
            if (_movedEntities == null) return false;

            while (_movingEntities.Count > 0 && !_movingEntities.Peek().IsMoving())
            {
                _movingEntities.Pop();
            }

            if (_movingEntities.Count == 0) return true;

            return false;
        }

        /**
         * Updates map after all blocks stop moving.
         */
        private void UpdateAfterMove()
        {
            _movedEntities = null;
            _movingEntities = null;

            var map = gameObject.GetComponentInParent<Map>();
            map.UpdateRules();
            map.ApplyRules();
        }
    }
}
