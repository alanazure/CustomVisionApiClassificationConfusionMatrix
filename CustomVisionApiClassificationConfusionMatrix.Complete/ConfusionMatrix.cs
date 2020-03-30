using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomVisionApiClassificationConfusionMatrix.Complete
{
    class ConfusionMatrix
    {
        public List<string> Classes { get; set; }
        public int[,] Predictions { get; set; }

        public ConfusionMatrix(List<string> classes)
        {
            Classes = classes;
            Predictions = new int[Classes.Count, Classes.Count];
        }

        public void WriteToConsole()
        {
            var temp = Console.ForegroundColor;
            var maxClassLength = Classes.Select(q => q.Length).Max();

            Console.Write("".PadRight(maxClassLength + 1));
            foreach (var tag in Classes)
            {
                Console.Write(tag.Substring(0, 5).PadLeft(6));
            }
            Console.WriteLine();

            for (int y = 0; y < Classes.Count; y++)
            {
                Console.ForegroundColor = temp;
                Console.Write(Classes[y].PadRight(maxClassLength + 1));
                for (int x = 0; x < Classes.Count; x++)
                {
                    Console.ForegroundColor = x == y ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.Write(Predictions[x, y].ToString().PadLeft(6));
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = temp;
        }


    }
}
