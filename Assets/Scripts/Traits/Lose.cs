using States;

namespace Traits
{
    public class Lose : Trait
    {
        public override int GetInteractionOrder()
        {
            return 200;
        }

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
    }
}
