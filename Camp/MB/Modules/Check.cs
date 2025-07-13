using MB1.Checks;
using MB1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MB1.Modules
{
    public static class Check
    {
        public static bool PerformChecks()
        {
            List<Checks.Check> allChecks = LoadChecks();

            foreach (var check in allChecks)
            {
                // load check
                var loadedCheck = allChecks.FirstOrDefault(c => c.Name.Equals(check.Name, StringComparison.OrdinalIgnoreCase));
                if (loadedCheck == null)
                {
                    WriteDebug.Error($"Failed to load Check: {check.Name} | Continuing...");
                    continue; ;
                }

                // perform check
                if (PerformCheck(loadedCheck))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool PerformChecks(string[] checkNames)
        {
            List<Checks.Check> allChecks = LoadChecks();

            foreach (var check in checkNames)
            {
                // load check
                var loadedCheck = allChecks.FirstOrDefault(c => c.Name.Equals(check, StringComparison.OrdinalIgnoreCase));
                if (loadedCheck == null)
                {
                    WriteDebug.Error($"Failed to load Check: {check} | Continuing...");
                    continue; ;
                }

                // perform check
                if (PerformCheck(loadedCheck))
                {
                    return true;
                }
            }

            return false;
        }
        private static bool PerformCheck(Checks.Check loadedCheck)
        {
            WriteDebug.Info($"Running Check: {loadedCheck.Name}");
            try
            {
                bool result = loadedCheck.Execute();
                return result;
            }
            catch (Exception ex)
            {
                WriteDebug.Error("PerformCheck Failed: " + ex.ToString());
                return false; // return false on error
            }
        }

        private static List<Checks.Check> LoadChecks()
        {
            List<Checks.Check> allChecks = new List<Checks.Check>();
            var self = System.Reflection.Assembly.GetExecutingAssembly();

            foreach (var type in self.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Check)))
                {
                    var instance = (Checks.Check)Activator.CreateInstance(type);
                    allChecks.Add(instance);
                }
            }

            return allChecks;
        }
    }
}