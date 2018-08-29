using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CortexTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program start.");
            CortexManager cm = new CortexManager();
            cm.Init();
        }
    }
}
