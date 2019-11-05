using System.Collections.Generic;
using Subjects;
using UnityEngine;

namespace Traits
{
    public class You : Trait
    {
        private Subject _subject;
        private Vector2Int _startingPosition;
        private Stack<Subject> _movedSubjects;
        private Stack<Subject> _movingSubjects;

        public override int GetInteractionOrder()
        {
            return 100;
        }

        void Start()
        {
            _subject = gameObject.GetComponent<Subject>();
        }

        // Update is called once per frame
        void Update()
        {
            CheckMoveEnd();
            
            if (_subject.CanMove())
            {
                int up = Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
                int down = Input.GetKey(KeyCode.DownArrow) ? 1 : 0;
                int right = Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
                int left = Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;

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

        private void TryMove(MoveDirection dir)
        {
            var thisSubject = gameObject.GetComponent<Subject>();
            if (thisSubject.CanMoveTo(dir))
            {
                _startingPosition = new Vector2Int(thisSubject.x, thisSubject.y);
                _movedSubjects = new Stack<Subject>();
                _movingSubjects = new Stack<Subject>();

                void RegisterMove(Subject s)
                {
                    _movedSubjects.Push(s);
                    _movingSubjects.Push(s);
                }
                
                thisSubject.MoveTo(dir, RegisterMove);
            }
        }

        private void CheckMoveEnd()
        {
            if (_movedSubjects == null) return;
            
            while (_movingSubjects.Count > 0 && _movingSubjects.Peek().CanMove())
            {
                _movingSubjects.Pop();
            }

            if (_movingSubjects.Count != 0) return;

            var positions = new List<Vector2Int>(_movedSubjects.Count + 1) {_startingPosition};

            while (_movedSubjects.Count > 0)
            {
                _subject = _movedSubjects.Pop();
                positions.Add(new Vector2Int(_subject.x, _subject.y));
                _subject.AfterMove();
            }

            var map = gameObject.GetComponentInParent<Map>();
            map.RefreshPositions(positions);
            map.UpdateRules();

            _movedSubjects = null;
            _movingSubjects = null;
        }
    }
}

