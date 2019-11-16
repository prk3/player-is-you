using System.Collections.Generic;
using States;
using Entities;

namespace Traits
{
    public class Win : Trait
    {
        public override int GetInteractionOrder()
        {
            return 200;
        }

        /**
         * Checks if YOU is on the same stack and float layer.
         * If so, loses the game.
         */
        public override RuleApplicationOutcome ApplyRuleOnStack(List<Entity> stack)
        {
            bool thisEntityIsFloating = gameObject.GetComponent<Float>() != null;

            foreach (var entity in stack)
            {
                bool entityIsFloating = entity.GetComponent<Float>() != null;
                if (thisEntityIsFloating == entityIsFloating && entity.GetComponent<You>() != null)
                {
                    AudioPlayer.PlaySound("win");
                    gameObject.GetComponentInParent<Gameplay>().Win();
                    return RuleApplicationOutcome.Break;
                }
            }

            return RuleApplicationOutcome.Continue;
        }
    }
}
