using System;
using System.Collections.Generic;
using Entities;
using UnityEngine;

/**
 * Possible results of enter hook:
 * PullDown - move entity down the stack
 * Break - stop evaluating rules
 * Continue - continue evaluation
 */
public enum OnEnterOutcome
{
    PullDown,
    Break,
    Continue,
}

/**
 * Possible outcomes fo stack update:
 * Refresh - evaluating stack again
 * Break - stop evaluating the stack
 * Continue - proceed to next entries/traits
 */
public enum RuleApplicationOutcome
{
    Refresh,
    Break,
    Continue,
}

/**
 * Defines how an entity behaves when moving and when interacting with other entities.
 */
namespace Traits
{
    public abstract class Trait : MonoBehaviour
    {

        /**
         * Defines the order of trait evaluation - higher values get evaluated first.
         */
        public abstract int GetInteractionOrder();

        /**
         * Can you walk onto this entry (moving in direction dir).
         */
        public virtual bool CanEnter(Entity entering, MoveDirection dir)
        {
            return true;
        }

        /*
         * React to entity walking onto entity with this trait.
         */
        public virtual OnEnterOutcome OnEnter(Entity entering, MoveDirection dir, Action<Entity> registerMove)
        {
            return OnEnterOutcome.Continue;
        }

        /**
         * Update stack according to this trait's logic.
         */
        public virtual RuleApplicationOutcome ApplyRuleOnStack(List<Entity> stack)
        {
            return RuleApplicationOutcome.Continue;
        }
    }
}
