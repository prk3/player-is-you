using Subjects;
using UnityEngine;

namespace Traits
{
    public class Stop : Trait
    {
        public override int GetInteractionOrder()
        {
            return 600;
        }

        public override bool CanEnter(Subject initiator)
        {
            return false;
        }
    }
}