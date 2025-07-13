
namespace MB1.Checks
{
    public abstract class Check
    {
        public abstract string Name { get; }
        public abstract bool Execute();
    }
}
