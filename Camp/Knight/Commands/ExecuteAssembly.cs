using Internal = SharedArsenal.Internal;
using Knight.Models.Raven;

namespace Knight.Commands
{
    public class ExecuteAssembly : KnightCommand
    {
        public override string Name => "execute-assembly";


        public override string Execute(TaskMessage task)
        {            
            var result = Internal.Execute.ExecuteAssembly(task.File, task.Arguments);
            //var result = Internal.Execute.ExecuteAssembly(task.FileBytes, task.Arguments);
            return result;
        }
    }
}