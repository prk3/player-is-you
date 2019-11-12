using Entities;

namespace Traits
{
    public class Stop : Trait
    {
        public override int GetInteractionOrder()
        {
            return 600;
        }

        /**
         * Can't walk onto this entity, even when float.
         */
        public override bool CanEnter(Entity initiator, MoveDirection dir)
        {
            return false;
        }
    }
}
