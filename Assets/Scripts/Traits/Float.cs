using Subjects;

namespace Traits
{
    public class Float : Trait
    {
        public override int GetInteractionOrder()
        {
            return 700;
        }

        public override bool CanEnter(Subject entering)
        {
            return true;
        }

        public override OnEnterOutcome OnEnter(Subject entering)
        {
            return OnEnterOutcome.PullDown;
        }
    }
}