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

        /**
         * Checks if YOU is on the same stack and float layer.
         * If so, wins the game.
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
