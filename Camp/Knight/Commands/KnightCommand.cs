using Knight.Models.Raven;

namespace Knight.Commands
{
    public abstract class KnightCommand
    {
        public abstract string Name { get; }

        protected Knight Knight{ get; private set; }

        public void Init(Knight knight)
        {
            this.Knight = knight;
        }

        public abstract string Execute(TaskMessage task);
    }
}