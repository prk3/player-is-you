using System.Collections.Generic;
using System.Linq;
using Subjects;
using UnityEngine;

namespace Traits
{
    public class Push: Trait
    {
        public override int GetInteractionOrder()
        {
            return 500;
        }

        public override bool CanEnter(Subject entering)
        {
            Subject thisSubject = gameObject.GetComponent<Subject>();
            var thisPos = new Vector2Int(thisSubject.x, thisSubject.y);
            var enteringPos = new Vector2Int(entering.x, entering.y);
            var to = thisPos + (thisPos - enteringPos);
            
            Map map = gameObject.GetComponentInParent<Map>();

            if (!map.IsValidSpot(to)) return false;
            
            List<Subject> stackToPush = map.stacks[to.y][to.x];
            return stackToPush.TrueForAll(subject =>
                {
                    return subject.GetComponents<Trait>().ToList().TrueForAll(trait => trait.CanEnter(thisSubject));
                }
            );
        }

        public override OnEnterOutcome OnEnter(Subject entering)
        {
            Subject thisSubject = gameObject.GetComponent<Subject>();
            var thisPos = new Vector2Int(thisSubject.x, thisSubject.y);
            var enteringPos = new Vector2Int(entering.x, entering.y);
            var to = thisPos + (thisPos - enteringPos);

            Map map = gameObject.GetComponentInParent<Map>();
            List<Subject> oldStack = map.stacks[thisSubject.y][thisSubject.x];
            List<Subject> newStack = map.stacks[to.y][to.x];
            thisSubject.z = newStack.Count == 0 ? 0 : newStack.First().z + 1;

            oldStack.Remove(thisSubject);
            int newStackPosition = 0;
            newStack.Insert(newStackPosition, thisSubject);

            for (int i = 1; i < newStack.Count; i++)
            {
                var subject = newStack[1];
                foreach (var trait in subject.GetComponents<Trait>().OrderByDescending(t => t.GetInteractionOrder()))
                {
                    var outcome = trait.OnEnter(thisSubject);
                    switch (outcome)
                    {
                        case OnEnterOutcome.PullDown:
                            newStack.RemoveAt(newStackPosition);
                            newStackPosition = i;
                            newStack.Insert(newStackPosition, thisSubject);
                            break;
                        case OnEnterOutcome.Break:
                            i = 100000; // break from outer loop
                            break;
                        case OnEnterOutcome.Continue:
                            break;
                    }
                }
            }

            thisSubject.Move(to);

            return OnEnterOutcome.Break;
        }
    }
}
