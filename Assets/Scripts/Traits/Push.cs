using System;
using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Traits
{
    public class Push: Trait
    {
        public override int GetInteractionOrder()
        {
            return 500;
        }

        public override bool CanEnter(Entity entering, MoveDirection dir)
        {
            Entity thisEntity = gameObject.GetComponent<Entity>();
            return thisEntity.CanMoveTo(dir);
        }

        public override OnEnterOutcome OnEnter(Entity entering, MoveDirection dir, Action<Entity> registerMove)
        {
            Entity thisEntity = gameObject.GetComponent<Entity>();
            thisEntity.MoveTo(dir, registerMove);

            return OnEnterOutcome.Break;
        }
    }
}
