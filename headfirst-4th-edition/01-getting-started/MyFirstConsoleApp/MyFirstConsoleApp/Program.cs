using System;

namespace MyFirstConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            OperatorExamples();
            OperatorExamples2();
        }

        public static void OperatorExamples()
        {

            Console.WriteLine("Hello, World!");
            Console.WriteLine("This is my first console application in C#.");
            Console.WriteLine("Press any key to exit...");

            int width = 3;
            width++;
            int height = 2 + 4;
            int area = width * height;
            Console.WriteLine(area);

            string result = "The area";
            result = result + " is " + area;
            Console.WriteLine(result);

            bool truthValue = true;
            Console.WriteLine(truthValue);

        }

        public static void OperatorExamples2()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(i);
            }

            int hold = 0;
            while (hold < 5)
            {
                Console.WriteLine("hold : " + hold++);
            }

            do
            {
                Console.WriteLine("while hold " + hold++);
            } while (hold < 10);
        }
    }
}

