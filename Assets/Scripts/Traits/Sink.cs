using System;
using Entities;

namespace Traits
{
    public class Sink : Trait
    {
        public override int GetInteractionOrder()
        {
            return 300;
        }

        public override OnEnterOutcome OnEnter(Entity entering, MoveDirection dir, Action<Entity> registerMove)
        {
            Destroy(entering.gameObject);
            return OnEnterOutcome.Break;
        }
    }
}
