
using System;

namespace Stage0
{
    partial class Program
    {
        private static void Main(string[] args)
        {
            welcome0118();
            welcome5210();
            Console.ReadKey();

        }

        static partial void welcome5210();
        private static void welcome0118()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first consloe application", name);
        }
    }
}