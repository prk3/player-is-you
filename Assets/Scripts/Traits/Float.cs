using System;
using Entities;

namespace Traits
{
    public class Float : Trait
    {
        public override int GetInteractionOrder()
        {
            return 700;
        }

        public override bool CanEnter(Entity entering, MoveDirection dir)
        {
            return true;
        }

        public override OnEnterOutcome OnEnter(Entity entering, MoveDirection dir, Action<Entity> registerMove)
        {
            return OnEnterOutcome.PullDown;
        }
    }
}
