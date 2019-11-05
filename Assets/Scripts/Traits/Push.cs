using System;
using System.Collections.Generic;
using Subjects;
using UnityEngine;

namespace Traits
{
    public class Push: Trait
    {
        public override int GetInteractionOrder()
        {
            return 500;
        }

        public override bool CanEnter(Subject entering, MoveDirection dir)
        {
            Map map = gameObject.GetComponentInParent<Map>();
            Subject thisSubject = gameObject.GetComponent<Subject>();
            var thisPos = new Vector2Int(thisSubject.x, thisSubject.y);
            var to = thisPos + Subject.DirectionToVector(dir);

            if (!map.IsValidSpot(to)) return false;
            
            List<Subject> targetStack = map.stacks[to.y][to.x];
            return targetStack.TrueForAll(subject => subject.CanMoveTo(dir));
        }

        public override OnEnterOutcome OnEnter(Subject entering, MoveDirection dir, Action<Subject> registerMove)
        {
            Subject thisSubject = gameObject.GetComponent<Subject>();
            thisSubject.MoveTo(dir, registerMove);

            return OnEnterOutcome.Break;
        }
    }
}
