using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;

namespace JC.QuickMove
{
    internal class Program
    {
        const string BillingBin = "C:\\Versioning\\billing\\bin";
        const string Binaries = "C:\\Program Files\\Smokeball\\binaries";

        [Flags]
        private enum Options
        {
            None = 0,
            WPF = 1 << 0,
            IncludeSharedDesktop = 1 << 8
        }

        private enum FileAsType { Dll, Pdb, Xml }
        private enum FileAsDirectory { Bin, Binaries }

        static Options RunOptions { get; set; }

        static void Main(string[] args)
        {
            SmokeballProcessController controller = new SmokeballProcessController();

            if (args.Length == 0)
            {
                Console.WriteLine("No parameters detected.");
                return;
            }
            else if (
                args.Select(a => a.Trim()).FirstOrDefault(a => a == "--help" || a == "?" || a == "/?" || a == "--?") != null)
            {
                Console.WriteLine("qw.exe [wpf billing domain (e.g. Fees) moves wpf dll, pdb and xml to binaries]");
                Console.WriteLine("[--sd] moves Billing.Shared.Desktop ");

                return;
            }
            SetupOptions(args);

            controller.Kill();

            if (RunOptions.HasFlag(Options.WPF))
                MoveBillingWpfFiles(args);
            if (RunOptions.HasFlag(Options.IncludeSharedDesktop))
                MoveSharedDesktop();

            controller.Start(SmokeballProcessController.SmokeballProcess.Service);
        }

        private static void MoveSharedDesktop()
        {
            MoveDllAndSupportingFile("Billing.Shared.Desktop");
        }

        private static void MoveBillingWpfFiles(string[] args)
        {
            foreach (var item in args)
            {
                if (!item.StartsWith("--"))
                {
                    string fileNameBase = $"Billing.{item}.Wpf";

                    MoveDllAndSupportingFile(item);
                }
            }
        }

        private static void MoveDllAndSupportingFile(string fileNameBase)
        {
            if (Move(
                FileAs(fileNameBase, FileAsType.Dll, FileAsDirectory.Bin),
                FileAs(fileNameBase, FileAsType.Dll, FileAsDirectory.Binaries)
                ))
            {
                Move(
                    FileAs(fileNameBase, FileAsType.Pdb, FileAsDirectory.Bin),
                    FileAs(fileNameBase, FileAsType.Pdb, FileAsDirectory.Binaries)
                    );

                Move(
                    FileAs(fileNameBase, FileAsType.Xml, FileAsDirectory.Bin),
                    FileAs(fileNameBase, FileAsType.Xml, FileAsDirectory.Binaries)
                    );
            }
        }

        private static bool Move(string source, string destination)
        {
            if (!File.Exists(source))
            {
                Console.WriteLine($"{source} does not exists. Ignoring");
                return false;
            }

            try
            {
                File.Copy(source, destination, true);

                Console.WriteLine($"Moved to {destination}: Last modified {new FileInfo(destination).LastWriteTime}");

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private static string FileAs(string file, FileAsDirectory directory) =>
            Path.Combine(
                directory == FileAsDirectory.Bin ? (BillingBin) : (Binaries), file);

        private static string FileAs(string fileBase, FileAsType type) =>
            $"{fileBase}.{type.ToString().ToLower()}";

        private static string FileAs(string fileBase, FileAsType type, FileAsDirectory directory)
        {
            return Path.Combine(
                directory == FileAsDirectory.Bin ? (BillingBin) : (Binaries),
                FileAs(fileBase, type)
                );
        }

        private static void SetupOptions(string[] args)
        {
            RunOptions = Options.WPF;

            foreach (var item in args)
            {
                if (item.StartsWith("--"))
                {
                    switch (item.ToLower())
                    {
                        case "--sd":
                            RunOptions = RunOptions | Options.IncludeSharedDesktop; break;
                    }
                }
            }
        }
    }
}
