using System;
using Subjects;

namespace Traits
{
    public class Sink : Trait
    {
        public override int GetInteractionOrder()
        {
            return 300;
        }

        public override OnEnterOutcome OnEnter(Subject entering, MoveDirection dir, Action<Subject> registerMove)
        {
            Destroy(entering.gameObject);
            return OnEnterOutcome.Break;
        }
    }
}