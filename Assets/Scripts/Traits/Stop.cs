using Entities;

namespace Traits
{
    public class Stop : Trait
    {
        public override int GetInteractionOrder()
        {
            return 600;
        }

        public override bool CanEnter(Entity initiator, MoveDirection dir)
        {
            return false;
        }
    }
}
