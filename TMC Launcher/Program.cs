using System.Diagnostics;
using System.Security.Cryptography;

namespace TMC_Launcher
{
    internal class Program
    {
        private static string fileHash = HashRetriever.String();
        private static bool externalFile = false;

        private static string? currentDirectory;
        private static string? appExePath;
        static void Main(string[] args)
        {
            if (!DoesConhostExist()) ConhostNotFound();

            currentDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty) ?? "";
            appExePath = Path.Combine(currentDirectory, "The Mighty Console.exe");

            bool fileNotFoundLoop = true;
            bool isExeLoop = true;
            bool hashLoop = true;
            do
            {
                if (!File.Exists(appExePath))
                {
                    fileNotFoundLoop = true;
                    ApplicationNotFound();
                    continue;
                } else fileNotFoundLoop = false;

                if (!appExePath.EndsWith(".exe"))
                {
                    isExeLoop = true;
                    NotExe();
                    continue;
                } else isExeLoop = false;

                if (!MatchesHash())
                {
                    hashLoop = true;
                    HashNotIncluded();
                    continue;
                } else hashLoop = false;
            } while (fileNotFoundLoop || isExeLoop || hashLoop);

            string quotedExePath = $"\"{appExePath}\"";

            var process = Process.Start("conhost.exe", $"cmd.exe /k {quotedExePath} launched");
            Environment.Exit(0);
        }

        private static bool DoesConhostExist()
        {
            string conhostPath = Path.Combine(Environment.SystemDirectory, "conhost.exe");
            return File.Exists(conhostPath);
        }
        private static void ConhostNotFound()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\"conhost.exe\" not found in C\\Windows\\System32.");
            Console.WriteLine();
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static void ApplicationNotFound()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            if (!externalFile) Console.WriteLine("\"The Mighty Console.exe\" not found in same directory.");
            else Console.WriteLine("File does not exist.");
            externalFile = true;
            NewFileRequest();
        }
        private static void NotExe()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("File isn't an executable.");
            NewFileRequest();
        }
        private static bool MatchesHash()
        {
            string calculatedHash = CalculateFileHash(appExePath ?? "");
            return calculatedHash == fileHash;
        }
        private static string CalculateFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                byte[] hashBytes = sha256.ComputeHash(fileBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        private static void HashNotIncluded()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            if (externalFile) Console.WriteLine("Incorrect executable.");
            else Console.WriteLine("\"The Mighty Console.exe\" is of wrong version.");
            externalFile = true;
            NewFileRequest();
        }

        private static void NewFileRequest()
        {
            Console.WriteLine();
            Console.Write("Drag the executable in and press enter: ");
            appExePath = (Console.ReadLine() ?? "").Replace("\"", "");
        }
    }
}