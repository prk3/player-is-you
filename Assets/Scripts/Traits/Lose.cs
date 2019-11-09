using States;
using Subjects;
using UnityEngine;

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
            var thisSubject = gameObject.GetComponent<Subject>();
            
            var stack = map.stacks[thisSubject.y][thisSubject.x];

            foreach (var subject in stack)
            {
                if (subject.gameObject.GetComponent<You>())
                {
                    gameObject.GetComponentInParent<Gameplay>().Lose();
                    return AfterEnterOutcome.Break;
                }
            }

            return AfterEnterOutcome.Continue;
        }
    }
}