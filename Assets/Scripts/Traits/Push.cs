using System.Collections.Generic;
using System.Linq;
using Subjects;

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
            int deltaX = thisSubject.x - entering.x;
            int deltaY = thisSubject.y - entering.y;

            int nextX = thisSubject.x + deltaX;
            int nextY = thisSubject.y + deltaY;
            Map map = gameObject.GetComponentInParent<Map>();

            if (!map.IsValidSpot(nextX, nextY)) return false;
            
            List<Subject> stackToPush = map.spots[nextY][nextX];
            return stackToPush.TrueForAll(subject =>
                {
                    return subject.GetComponents<Trait>().ToList().TrueForAll(trait => trait.CanEnter(thisSubject));
                }
            );
        }

        public override OnEnterOutcome OnEnter(Subject entering)
        {
            Subject thisSubject = gameObject.GetComponent<Subject>();
            int deltaX = thisSubject.x - entering.x;
            int deltaY = thisSubject.y - entering.y;

            int toX = thisSubject.x + deltaX;
            int toY = thisSubject.y + deltaY;
            Map map = gameObject.GetComponentInParent<Map>();

            List<Subject> oldStack = map.spots[thisSubject.y][thisSubject.x];
            List<Subject> newStack = map.spots[toY][toX];
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

            thisSubject.TransitionPosition((toX, toY));

            return OnEnterOutcome.Break;
        }
    }
}
