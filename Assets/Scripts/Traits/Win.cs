using System.Collections.Generic;
using States;
using Entities;

namespace Traits
{
    public class Win : Trait
    {
        public override int GetInteractionOrder()
        {
            return 100;
        }

        /*
        public override AfterEnterOutcome AfterEnterLate()
        {
            var map = gameObject.GetComponentInParent<Map>();
            var thisEntity = gameObject.GetComponent<Entity>();

            var stack = map.stacks[thisEntity.y][thisEntity.x];

            foreach (var entity in stack)
            {
                if (entity.gameObject.GetComponent<You>())
                {
                    gameObject.GetComponentInParent<Gameplay>().Win();
                    return AfterEnterOutcome.Break;
                }
            }

            return AfterEnterOutcome.Continue;
        }
        */

        public override RuleApplicationOutcome ApplyRuleOnStack(List<Entity> stack)
        {
            bool thisEntityIsFloating = gameObject.GetComponent<Float>() != null;

            foreach (var entity in stack)
            {
                bool entityIsFloating = entity.GetComponent<Float>() != null;
                if (thisEntityIsFloating == entityIsFloating && entity.GetComponent<You>() != null)
                {
                    gameObject.GetComponentInParent<Gameplay>().Win();
                    return RuleApplicationOutcome.Break;
                }
            }

            return RuleApplicationOutcome.Continue;
        }
    }
}
