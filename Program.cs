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
    [Flags]
    public enum Options
    {
        None = 0,
        WPF = 1 << 0,
        IncludeSharedDesktop = 1 << 8
    }

    public enum FileAsType { Dll, Pdb, Xml }
    public enum FileAsDirectory { Bin, Binaries }
    
    internal class Program
    {
        static Options RunOptions { get; set; }

        static void Main(string[] args)
        {
            SmokeballProcessController controller = new SmokeballProcessController();
            FileMover fileMover = new FileMover();

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
                fileMover.MoveBillingWpfFiles(args);
            if (RunOptions.HasFlag(Options.IncludeSharedDesktop))
                fileMover.MoveSharedDesktop();

            controller.Start(SmokeballProcessController.SmokeballProcess.Service);
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
