using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSA_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            bool goFlag = true;
            while (goFlag)
            {
                string input = "";
                Console.Write("Enter Message:");
                input = Console.ReadLine();
                int message = Convert.ToInt32(input);

                Console.Write("Enter e value:");
                input = Console.ReadLine();
                int e = Convert.ToInt32(input);

                Console.Write("Enter n value:");
                input = Console.ReadLine();
                int n = Convert.ToInt32(input);

                
                int c = -1;
                
                c = Convert.ToInt32((Math.Pow(message, e)) % n);
                Console.WriteLine("Encrypted message: " + c);
                
                
                Console.Write("Encrypt another message [y/n]?");
                string userInput = Console.ReadLine();
                if (userInput.ToLower() == "y")
                {
                    Console.WriteLine();
                    goFlag = true;
                }
                else
                {
                    Console.WriteLine("Terminating Program.");
                    goFlag = false;
                }
                
                
            }

        }
    }
}
