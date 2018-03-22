//#define _DEBUG

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace backtrack1
{
    public delegate void WriteAction(string format, params object[] arg);
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
        
        public static int[,] ReadFile(string file)
        {
            int[,] res;
            using (StreamReader reader = new StreamReader(file))
            {
                string[] words = reader.ReadToEnd().Split('\n');
                int[] size = (from w in words[0].Split(' ') select Int32.Parse(w)).ToArray();
                res = new int[size[1],size[0]];
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

            Write("\nPART 2. Trace\n");
            /*
            for (int i = 0; i < 1000000; i++)
                Write(i.ToString() + "\n");
            */
            Backtrack1();

            _maze.Print(Write);

            _writer.Dispose();
        }

        private static int step = 1;

        static bool Backtrack1()
        {
            StringBuilder sb = new StringBuilder();
            int u, v;
            int mStep = _maze.CurrentStep;
            for(int i=0; i<_maze.MoveCount; i++)
            {
                Write("{0,5}) ", step++);
                for (int j = 2; j < mStep; j++)
                {
                    sb.Append("-");
                }
                MoveStatus ms = _maze.ApplyMove(i, out u, out v);
                sb.AppendFormat("R{0}. U={1}, V={2}. ", i + 1, u, v);
                switch(ms)
                {
                    case MoveStatus.Thread:
                        sb.Append("Thread.\n");
                        Write(sb.ToString());
                        break;
                    case MoveStatus.Wall:
                        sb.Append("Wall.\n");
                        Write(sb.ToString());
                        break;
                    case MoveStatus.Free:
                        sb.AppendFormat("Free. L:=L+1={0}. LAB[{1},{2}]:={3}.\n", _maze.CurrentStep, u, v, _maze.CurrentStep);
                        Write(sb.ToString());
                        if (Backtrack1())
                            return true;
                        break;
                    case MoveStatus.Terminal:
                        sb.AppendFormat("Free. L:=L+1={0}. LAB[{1},{2}]:={3}. Terminal.\n", _maze.CurrentStep, u, v, _maze.CurrentStep);
                        Write(sb.ToString());
                        return true;
                    default:
                        Write("unknown error.\n");
                        return true;
                }
                sb.Clear();
            }
            u = _maze.GetCurrentX();
            v = _maze.GetCurrentY();
            _maze.UndoMove();
            sb.Append("       ");
            for (int i = 2; i < mStep; i++)
            {
                sb.Append("-");
            }
            sb.AppendFormat("Backtrack from X={0}, Y={1}, L={2}. LAB[{0},{1}]:=-1. L:=L-1={3}.\n", u, v, _maze.CurrentStep + 1, _maze.CurrentStep);
            Write(sb.ToString());
            sb.Clear();
            return false;
        }
    }
}
