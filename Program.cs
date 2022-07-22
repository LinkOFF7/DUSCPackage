using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DUSCPackage
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                PrintUsage();
                return;
            }
            TPPack tp = new TPPack();
            if (args[0] == "extract")
            {
                
                tp.filename = args[1];
                tp.Extract();
                return;
            }
            else if(args[0] == "repack")
            {
                tp.filename = args[1];
                tp.Repack(args[2], args[3]);
                return;
            }
            else
            {
                PrintUsage();
                return;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Danganronpa S USC .tp Extraction Tool by LinkOFF v.0.9");
            Console.WriteLine("");
            Console.WriteLine("Usage: DUSCPackage.exe [mode] <arguments>");
            Console.WriteLine("");
            Console.WriteLine("Modes:");
            Console.WriteLine("  extract\t\tExtract all data.");
            Console.WriteLine("  repack\t\tCreate a new TP archive.");
            Console.WriteLine("");
            Console.WriteLine("Example: DUSCPackage.exe extract common-data.tp");
            Console.WriteLine("Example: DUSCPackage.exe repack common-data.tp drv3_data common-data_new.tp");
            Console.WriteLine("");
        }
    }
}
