using System;
using System.Collections.Generic;
using Entities;
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

public enum RuleApplicationOutcome
{
    Refresh,
    Break,
    Continue,
}

namespace Traits
{
    public abstract class Trait : MonoBehaviour
    {

        public abstract int GetInteractionOrder();

        public virtual bool CanEnter(Entity entering, MoveDirection dir)
        {
            return true;
        }

        public virtual OnEnterOutcome OnEnter(Entity entering, MoveDirection dir, Action<Entity> registerMove)
        {
            return OnEnterOutcome.Continue;
        }

        /*
        public virtual AfterEnterOutcome AfterEnterEarly()
        {
            return AfterEnterOutcome.Continue;
        }

        public virtual AfterEnterOutcome AfterEnterLate()
        {
            return AfterEnterOutcome.Continue;
        }
        */

        public virtual RuleApplicationOutcome ApplyRuleOnStack(List<Entity> stack)
        {
            return RuleApplicationOutcome.Continue;
        }
    }
}
