using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplayTrees
{
    class Program
    {
       /// <summary>
       /// Three arguments expected - name of the input file, name of the output file and type of the tree to be built - STANDARD, NAIVE or OPTIMAL.
       /// </summary>
       /// <param name="args">Arguments of the program.</param>
       static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Three args expected, name of input and output file, and tree type - STANDARD or NAIVE.");
                return;
            }
            SplayTreeType type = SplayTreeType.NAIVE;
            if (args[2] == "STANDARD") type = SplayTreeType.STANDARD;
            else if (args[2] == "NAIVE") type = SplayTreeType.NAIVE;
            else if (args[2] == "OPTIMAL") type = SplayTreeType.OPTIMAL;
            else
            {
                Console.WriteLine("STANDARD, NAIVE, or OPTIMAL expected as the third argument");
            }
            TreeGenerator generator = new TreeGenerator(args[0], args[1], type);
            generator.CreateTree();
            generator.FinishCurrentTree();
            generator.writer.Close();
        }
    }
}
