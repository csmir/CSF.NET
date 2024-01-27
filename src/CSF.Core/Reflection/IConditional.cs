using CSF.Preconditions;

namespace CSF.Reflection
{

    public interface IConditional : INameable
    {

        public string[] Aliases { get; }


        public PreconditionAttribute[] Preconditions { get; }


        public bool HasPreconditions { get; }
    }
}
