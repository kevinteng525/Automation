using System.Collections.Generic;
using GACManagerApi;

namespace Common.Windows
{
    public static class GACAssembly
    {
        public static List<AssemblyDescription> GetAllGACAssembly()
        {
            var list = new List<AssemblyDescription>();

            var assemblyEnumerator = new AssemblyCacheEnumerator();

            //  Get the first assembly.
            var assemblyName = assemblyEnumerator.GetNextAssembly();

            //  Start to loop through the assemblies.
            while (assemblyName != null)
            {
                //  The 'assemblyName' object is a COM interface, if we create an 
                //  AssemblyDescription from it, we will have access to more information.
                var assemblyDescription = new AssemblyDescription(assemblyName);

                list.Add(assemblyDescription);

                //  Move to the next assembly.
                assemblyName = assemblyEnumerator.GetNextAssembly();
            }

            return list;
        }

        public static bool IsAssemblyExist(string name, string version)
        {
            var assemblyEnumerator = new AssemblyCacheEnumerator();

            //  Get the first assembly.
            var assemblyName = assemblyEnumerator.GetNextAssembly();

            //  Start to loop through the assemblies.
            while (assemblyName != null)
            {
                //  The 'assemblyName' object is a COM interface, if we create an 
                //  AssemblyDescription from it, we will have access to more information.
                var assemblyDescription = new AssemblyDescription(assemblyName);

                if (assemblyDescription.Name.ToUpper().Trim() == name.ToUpper().Trim() && assemblyDescription.Version.Trim() == version.Trim())
                {
                    return true;
                }

                //  Move to the next assembly.
                assemblyName = assemblyEnumerator.GetNextAssembly();
            }

            return false;
        }

        public static Dictionary<string, List<string>> GetGACAssemblyVersions()
        {
            var versions = new Dictionary<string, List<string>>();

            foreach (var assembly in GetAllGACAssembly())
            {
                if(!versions.ContainsKey(assembly.Name))
                {
                    versions.Add(assembly.Name, new List<string> {assembly.Version});
                }
                else
                {
                    versions[assembly.Name].Add(assembly.Version);
                }
            }

            return versions;
        }
    }
}
