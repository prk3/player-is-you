using System.Collections.Generic;
using Entities;
using States;

namespace Traits
{
    public class Lose : Trait
    {
        public override int GetInteractionOrder()
        {
            return 200;
        }

        /*
        public override AfterEnterOutcome AfterEnterLate()
        {
            var map = gameObject.GetComponentInParent<Map>();
            var thisSubject = gameObject.GetComponent<Entities.Entity>();

            var stack = map.stacks[thisSubject.y][thisSubject.x];

            foreach (var entity in stack)
            {
                if (entity.gameObject.GetComponent<You>())
                {
                    gameObject.GetComponentInParent<Gameplay>().Lose();
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
                    AudioPlayer.PlaySound("lose");
                    gameObject.GetComponentInParent<Gameplay>().Lose();
                    return RuleApplicationOutcome.Break;
                }
            }

            return RuleApplicationOutcome.Continue;
        }
    }
}
