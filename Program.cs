using System;
using System.Collections.Generic;
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
            tp.filename = args[0];
            tp.Extract(true);
        }

        static void PrintUsage()
        {
            Console.WriteLine("Danganronpa S USC .tp Extraction Tool by LinkOFF");
            Console.WriteLine("");
            Console.WriteLine("Usage: drag .tp file to extract it.");
            Console.WriteLine("");
        }
    }
}
