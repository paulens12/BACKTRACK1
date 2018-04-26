using System;
using System.IO;
using System.Linq;

namespace breadth_first
{
    class Program
    {
        private static StreamWriter _writer;
        private static Maze _maze;

        public static void Write(string format, params object[] arg)
        {
            #if _DEBUG
                Console.Write(format, arg);
            #else
                _writer.Write(format, arg);
            #endif
        }

        private static int[,] ReadFile(string file)
        {
            int[,] res;
            using (StreamReader reader = new StreamReader(file))
            {
                string[] words = reader.ReadToEnd().Split('\n');
                int[] size = (from w in words[0].Split(' ') select Int32.Parse(w)).ToArray();
                res = new int[size[1], size[0]];
                for (int i = 0; i < size[1]; i++)
                {
                    for (int j = 0; j < size[0]; j++)
                    {
                        //string a = words[i + 2][j].ToString();
                        res[size[1] - i - 1, j] = Int32.Parse(words[i + 2][j].ToString());
                    }
                }
            }
            return res;
        }

        static void Main(string[] args)
        {
            string fin, fout;
            try
            {
                fin = args[0];
                fout = args[1];
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Incorrect arguments.\n1. input file name\n2. output file name");
                return;
            }
            _writer = new StreamWriter(fout);
            _maze = new Maze(ReadFile(fin));

            Write("PART 1. Data\n  1.1. Labyrinth\n\n");
            _maze.Print(Write);
            Write("\n  1.2. Initial position X={0}, Y={1}, L={2}\n", _maze.GetCurrentX(), _maze.GetCurrentY(), _maze.CurrentStep);

        }
    }
}
