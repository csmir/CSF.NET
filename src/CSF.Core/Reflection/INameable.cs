namespace CSF.Reflection
{

    public interface INameable
    {

        public string Name { get; }


        public Attribute[] Attributes { get; }
    }
}
