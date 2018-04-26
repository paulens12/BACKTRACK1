using System;
using System.Collections.Generic;
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
            #if DEBUG
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
            Maze.Position pos = _maze.OpenCells[0];
            Write("\n  1.2. Initial position X={0}, Y={1}, NEWN=1\n", pos.X + 1, pos.Y + 1);
            Write("\nPART 2. Trace\n");

            Write("WAVE 0, label L=\"2\". Initial position X={0}, Y={1}, NEWN=1.\n", pos.X + 1, pos.Y + 1);
            bool success = Solve();

            Write("\nPART 3. Results\n\n");
            if (success)
            {
                Write("  3.1. Path is found.\n\n");
                Write("  3.2. Path graphically\n\n");
                _maze.Print(Write);
                Write("\n  3.3. Rules: ");
                _maze.PrintRules(Write);
                Write("\n  3.4. Nodes: ");
                _maze.PrintNodes(Write);
            }
            else
            {
                Write("Path not found.\n\n");
                _maze.Print(Write);
            }
        }
        
        private static bool Solve()
        {
            int newn = 2;
            int close = 1;
            int newx, newy;
            while(!_maze.Full)
            {
                Write("WAVE {0}, label L=\"{1}\"\n", _maze.Wave, _maze.Wave + 2);
                List<Maze.Position> positions = _maze.OpenCells;
                for(int i=0; i<positions.Count; i++)
                {
                    Write("    Close CLOSE={0}, X={1}, Y={2}.\n", close++, positions[i].X + 1, positions[i].Y + 1);
                    for (int j=0; j<_maze.MoveCount; j++)
                    {
                        MoveStatus ms = _maze.ApplyMove(positions[i], j, out newx, out newy);
                        Write("        R{0}. X={1}, Y={2}. ", j + 1, newx, newy);
                        switch(ms)
                        {
                            case MoveStatus.Free:
                                Write("Free. NEWN={0}.\n", newn++);
                                break;
                            case MoveStatus.Thread:
                                Write("CLOSED or OPEN.\n");
                                break;
                            case MoveStatus.Wall:
                                Write("Wall.\n");
                                break;
                            case MoveStatus.Terminal:
                                Write("Free. NEWN={0}. Terminal.\n", newn++);
                                return true;
                        }
                    }
                    Write("\n");
                }
                _maze.FlushOpenCells();
                //break;
            }
            return false;
        }
    }
}
