using System;
using Entities;

namespace Traits
{
    public class Push: Trait
    {
        public override int GetInteractionOrder()
        {
            return 500;
        }

        /**
         * Push block is enterable if stack in the direction dir is enterable.
         */
        public override bool CanEnter(Entity entering, MoveDirection dir)
        {
            Entity thisEntity = gameObject.GetComponent<Entity>();
            return thisEntity.CanMoveTo(dir);
        }

        /**
         * On enter push this entity to the stack in direction dir.
         */
        public override OnEnterOutcome OnEnter(Entity entering, MoveDirection dir, Action<Entity> registerMove)
        {
            Entity thisEntity = gameObject.GetComponent<Entity>();
            thisEntity.MoveTo(dir, registerMove);
            AudioPlayer.PlaySound("push");

            return OnEnterOutcome.Break;
        }
    }
}
