using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Traits
{
    public class Sink : Trait
    {
        public override int GetInteractionOrder()
        {
            return 400;
        }

        /**
         * If stack has other entities with the same float layer, removes self and those entities.
         */
        public override RuleApplicationOutcome ApplyRuleOnStack(List<Entity> stack)
        {
            Entity thisEntity = gameObject.GetComponent<Entity>();
            bool thisEntityIsFloating = gameObject.GetComponent<Float>() != null;

            foreach (var entity in stack)
            {
                bool entityIsFloating = entity.GetComponent<Float>() != null;
                if (thisEntityIsFloating == entityIsFloating && entity.GetComponent<Sink>() == null)
                {
                    var map = gameObject.GetComponentInParent<Map>();
                    stack.Remove(entity);
                    Destroy(entity.gameObject);
                    stack.Remove(thisEntity);
                    Destroy(thisEntity.gameObject);
                    map.RefreshStacks(new List<Vector2Int> { new Vector2Int(thisEntity.x, thisEntity.y)});
                    AudioPlayer.PlaySound("sink");
                    return RuleApplicationOutcome.Refresh;
                }
            }

            return RuleApplicationOutcome.Continue;
        }
    }
}
