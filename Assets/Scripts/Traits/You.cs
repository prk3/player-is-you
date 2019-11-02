using System.Collections.Generic;
using System.Linq;
using Subjects;
using UnityEngine;

namespace Traits
{
    public class You : Trait
    {
        private Subject _subject;

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
            if (!_subject.IsMoving())
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
            if (CanMoveTo(dir))
            {
                Move(dir);
            }
        }

        private bool CanMoveTo(MoveDirection dir)
        {
            var map = gameObject.GetComponentInParent<Map>();
            var thisSubject = gameObject.GetComponent<Subject>();
            
            var (deltaX, deltaY) = Subject.DirectionToVector(dir);
            var (toX, toY) = (thisSubject.x + deltaX, thisSubject.y + deltaY);

            if (!map.IsValidSpot(toX, toY)) return false;

            var stack = map.stacks[toY][toX];
            return stack.TrueForAll(subject =>
            {
                return subject.GetComponents<Trait>().ToList().TrueForAll(trait => trait.CanEnter(thisSubject));
            });
        }

        private void Move(MoveDirection dir)
        {
            Map map = gameObject.GetComponentInParent<Map>();
            Subject thisSubject = gameObject.GetComponent<Subject>();
            
            var (deltaX, deltaY) = Subject.DirectionToVector(dir);
            int toX = thisSubject.x + deltaX;
            int toY = thisSubject.y + deltaY;

            List<Subject> oldStack = map.stacks[thisSubject.y][thisSubject.x];
            List<Subject> newStack = map.stacks[toY][toX];
            thisSubject.z = newStack.Count == 0 ? 0 : newStack.First().z + 1;

            oldStack.Remove(thisSubject);
            int newStackPosition = 0;
            newStack.Insert(newStackPosition, thisSubject);

            for (int i = 1; i < newStack.Count; i++)
            {
                var subject = newStack[1];
                foreach (var trait in subject.GetComponents<Trait>().OrderByDescending(t => t.GetInteractionOrder()))
                {
                    var outcome = trait.OnEnter(thisSubject);
                    switch (outcome)
                    {
                        case OnEnterOutcome.PullDown:
                            newStack.RemoveAt(newStackPosition);
                            newStackPosition = i;
                            newStack.Insert(newStackPosition, thisSubject);
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

            thisSubject.TransitionPosition((toX, toY));
            map.UpdateRules();
        }
    }
}

