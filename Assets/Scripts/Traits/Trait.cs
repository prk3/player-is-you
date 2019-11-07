using System;
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
    Break,
    Continue,
}

namespace Traits
{
    public abstract class Trait : MonoBehaviour
    {

        public abstract int GetInteractionOrder();
        
        public virtual bool CanEnter(Subject entering, MoveDirection dir)
        {
            return true;
        }

        public virtual OnEnterOutcome OnEnter(Subject entering, MoveDirection dir, Action<Subject> registerMove)
        {
            return OnEnterOutcome.Continue;
        }

        public virtual AfterEnterOutcome AfterEnterEarly()
        {
            return AfterEnterOutcome.Continue;
        }

        public virtual AfterEnterOutcome AfterEnterLate()
        {
            return AfterEnterOutcome.Continue;
        }
    }
}
