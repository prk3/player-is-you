using Subjects;
using UnityEngine;

public enum OnEnterOutcome
{
    PullDown,
    Break,
    Continue,
}

public enum AfterEnterOutcome
{
    InteractionContinue,
    InteractionStop,
}

namespace Traits
{
    public abstract class Trait : MonoBehaviour
    {

        public abstract int GetInteractionOrder();
        
        public virtual bool CanEnter(Subject entering)
        {
            return true;
        }

        public virtual OnEnterOutcome OnEnter(Subject entering)
        {
            return OnEnterOutcome.Continue;
        }

        public virtual AfterEnterOutcome AfterEnter(Subject entering)
        {
            return AfterEnterOutcome.InteractionContinue;
        }
    }
}
