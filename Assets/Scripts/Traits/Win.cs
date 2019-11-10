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
    }
}
