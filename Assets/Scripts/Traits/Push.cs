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
            Subject thisSubject = gameObject.GetComponent<Subject>();
            return thisSubject.CanMoveTo(dir);
        }

        public override OnEnterOutcome OnEnter(Subject entering, MoveDirection dir, Action<Subject> registerMove)
        {
            Subject thisSubject = gameObject.GetComponent<Subject>();
            thisSubject.MoveTo(dir, registerMove);

            return OnEnterOutcome.Break;
        }
    }
}
