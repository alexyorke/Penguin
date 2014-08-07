using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplescriptPort
{
    class Program
    {
        static void Main(string[] args)
        {
            Script script = new Script();
            int[] ids = script.Run();

            Console.ReadKey();
        }
    }
}
