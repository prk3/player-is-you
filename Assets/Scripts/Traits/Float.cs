using System;
using Subjects;

namespace Traits
{
    public class Float : Trait
    {
        public override int GetInteractionOrder()
        {
            return 700;
        }

        public override bool CanEnter(Subject entering, MoveDirection dir)
        {
            return true;
        }

        public override OnEnterOutcome OnEnter(Subject entering, MoveDirection dir, Action<Subject> registerMove)
        {
            return OnEnterOutcome.PullDown;
        }
    }
}