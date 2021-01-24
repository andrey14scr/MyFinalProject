using System;
using System.IO;

namespace IngredientsFormat
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines;
            using (var sr = new StreamReader(args[0]))
            {
                lines = sr.ReadToEnd().Split("\n");
            }

            using (var sw = new StreamWriter("out.txt"))
            {
                foreach (var item in lines)
                {
                    sw.Write(item.Replace(", ", "|"));
                }
            }

            Console.WriteLine("Done.");
        }
    }
}
