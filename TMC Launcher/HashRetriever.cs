using System.Reflection;

namespace TMC_Launcher
{
    internal static class HashRetriever
    {
        private static readonly string _hash;

        static HashRetriever()
        {
            _hash = InternalFileToList("executableHash.txt");
        }

        public static string String() => _hash;

        private static string InternalFileToList(string dotPath)
        {
            string output = string.Empty;
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "TMC_Launcher." + dotPath;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName) ?? Stream.Null)
            using (StreamReader reader = new StreamReader(stream))
            {
                string? line = null;
                reader.ReadLine();
                line = reader.ReadLine();
                if (line != null) output = line;
            }
            return output;
        }
    }
}