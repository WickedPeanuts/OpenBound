using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openbound_Asset_Tools.Helper
{
    public class RestartHelper
    {
        public void RestartFunction(Action action)
        {
            do
            {
                Console.WriteLine("Would you like to restart the process using the imported files? y/n");
                string answer = Console.ReadLine().ToLower();
                switch (answer)
                {
                    case "y":
                        action();
                        continue;
                    case "n":
                        break;
                    default:
                        Console.WriteLine("Invalid input");
                        continue;
                }
                break;
            } while (true);
        }
    }
}
