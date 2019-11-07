using System.Collections.Generic;
using Subjects;
using UnityEngine;

namespace Traits
{
    public class You : Trait
    {
        private Subject _subject;
        private List<Subject> _movedSubjects;
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
            
            if (!_subject.IsMoving())
            {
                int up = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) ? 1 : 0;
                int down = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ? 1 : 0;
                int right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)  ? 1 : 0;
                int left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)  ? 1 : 0;

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
                var startingPosition = new Vector2Int(thisSubject.x, thisSubject.y);
                _movedSubjects = new List<Subject>();
                _movingSubjects = new Stack<Subject>();

                void RegisterMove(Subject s)
                {
                    _movedSubjects.Add(s);
                    _movingSubjects.Push(s);
                }
                
                thisSubject.MoveTo(dir, RegisterMove);
                
                var positions = new List<Vector2Int>(_movedSubjects.Count + 1) {startingPosition};
                
                for (int i = _movedSubjects.Count - 1; i >= 0; i--)
                {
                    _subject = _movedSubjects[i];
                    positions.Add(new Vector2Int(_subject.x, _subject.y));
                    _subject.AfterMoveEarly();
                }
                
                var map = gameObject.GetComponentInParent<Map>();
                map.RefreshPositions(positions);
            }
        }

        private void CheckMoveEnd()
        {
            if (_movedSubjects == null) return;
            
            while (_movingSubjects.Count > 0 && !_movingSubjects.Peek().IsMoving())
            {
                _movingSubjects.Pop();
            }

            if (_movingSubjects.Count != 0) return;

            for (int i = _movedSubjects.Count - 1; i >= 0; i--)
            {
                _subject = _movedSubjects[i];
                _subject.AfterMoveLate();
            }

            _movedSubjects = null;
            _movingSubjects = null;
            
            var map = gameObject.GetComponentInParent<Map>();
            map.UpdateRules();
        }
    }
}

