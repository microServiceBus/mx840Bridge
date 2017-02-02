using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var bride = new mx840Bridge.Bridge();
            bride.Invoke(new { operation= "test", host= "169.254.190.130", port=0 });
            Console.ReadLine();
        }
    }
}
