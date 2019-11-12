using System;
using System.Collections.Generic;
using Entities;
using States;
using UnityEngine;

namespace Traits
{
    public class Sink : Trait
    {
        public override int GetInteractionOrder()
        {
            return 300;
        }

        /*
        public override OnEnterOutcome OnEnter(Entity entering, MoveDirection dir, Action<Entity> registerMove)
        {
            Destroy(entering.gameObject);
            return OnEnterOutcome.Break;
        }
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
