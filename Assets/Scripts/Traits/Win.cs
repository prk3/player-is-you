using Subjects;
using UnityEngine;

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
            var thisSubject = gameObject.GetComponent<Subject>();
            
            var stack = map.stacks[thisSubject.y][thisSubject.x];

            foreach (var subject in stack)
            {
                if (subject.gameObject.GetComponent<You>())
                {
                    Debug.Log("WON!");
                    return AfterEnterOutcome.Break;
                }
            }

            return AfterEnterOutcome.Continue;
        }
    }
}