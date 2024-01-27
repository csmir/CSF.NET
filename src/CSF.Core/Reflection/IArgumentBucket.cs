namespace CSF.Reflection
{

    public interface IArgumentBucket
    {

        public IArgument[] Parameters { get; }


        public bool HasParameters { get; }


        public int MinLength { get; }


        public int MaxLength { get; }
    }
}
