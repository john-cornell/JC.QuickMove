using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JC.QuickMove
{
    internal class FileMover
    {
        const string BillingBin = "C:\\Versioning\\billing\\bin";
        const string Binaries = "C:\\Program Files\\Smokeball\\binaries";

        public void MoveSharedDesktop()
        {
            MoveDllAndSupportingFile("Billing.Shared.Desktop");
        }

        public void MoveBillingWpfFiles(string[] args)
        {
            foreach (var item in args)
            {
                if (!item.StartsWith("--"))
                {
                    string fileNameBase = $"Billing.{item}.Wpf";

                    MoveDllAndSupportingFile(fileNameBase);
                }
            }
        }

        private bool Move(string source, string destination)
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

        private string FileAs(string file, FileAsDirectory directory) =>
            Path.Combine(
                directory == FileAsDirectory.Bin ? (BillingBin) : (Binaries), file);

        private string FileAs(string fileBase, FileAsType type) =>
            $"{fileBase}.{type.ToString().ToLower()}";

        private string FileAs(string fileBase, FileAsType type, FileAsDirectory directory)
        {
            return Path.Combine(
                directory == FileAsDirectory.Bin ? (BillingBin) : (Binaries),
                FileAs(fileBase, type)
                );
        }

        private void MoveDllAndSupportingFile(string fileNameBase)
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
    }
}
