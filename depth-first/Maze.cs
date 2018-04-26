using System;
using System.Collections.Generic;
using System.Text;

namespace breadth_first
{
    public delegate void WriteAction(string format, params object[] arg);
    public enum MoveStatus { Free, Wall, Thread, Error, Terminal };
    public class Maze
    {
        public class Position
        {
            public int X { get; private set; }
            public int Y { get; private set; }
            public Position(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static Position operator +(Position a, Position b)
            {
                return new Position(a.X + b.X, a.Y + b.Y);
            }

            public static Position operator -(Position a, Position b)
            {
                return new Position(a.X - b.X, a.Y - b.Y);
            }
        }

        private Position lastVisited;
        private int[,] db;
        public List<Position> OpenCells { get; private set; }
        private List<Position> NextOpenCells;
        public bool Full
        {
            get
            {
                return OpenCells.Count == 0 && NextOpenCells.Count == 0;
            }
        }
        public int Wave { get; private set; }

        private Position[] _moves =
        {
            new Position(-1, 0),
            new Position(0, -1),
            new Position(1, 0),
            new Position(0, 1),
        };
        public int MoveCount { get { return _moves.Length; } }

        public Maze(int[,] db)
        {
            Wave = 1;
            OpenCells = new List<Position>();
            NextOpenCells = new List<Position>();
            this.db = db;

            for(int i=0; i < db.GetLength(0); i++)
            {
                for(int j = 0; j < db.GetLength(1); j++)
                {
                    //this.db[i, j] = db[j, i];
                    if (this.db[i, j] == 2)
                        OpenCells.Add(new Position(j, i));
                }
            }
        }
        
        public void Print(WriteAction wr)
        {
            wr("   ^ Y, V\n");
            StringBuilder sb = new StringBuilder();
            List<object> args = new List<object>();
            int width = db.GetLength(1);
            for (int i = db.GetLength(0) - 1; i >= 0; i--)
            {
                sb.AppendFormat("{0,3}|", i + 1);
                for (int j = 0; j < width; j++)
                {
                    sb.Append("{" + j + ",5}");
                    args.Add(db[i, j]);
                }
                sb.Append("\n");
                wr(sb.ToString(), args.ToArray());
                sb.Clear();
                args.Clear();
            }
            sb.EnsureCapacity(width * 5 + 3);
            sb.Append("   ");
            for (int i = 0; i < width * 5 + 3; i++)
            {
                sb.Append("-");
            }
            sb.Append("> X, U\n    ");
            for(int i=0; i<width; i++)
            {
                sb.Append(String.Format("{0,5}", i + 1));
            }
            sb.Append("\n");
            wr(sb.ToString());
        }

        public void FlushOpenCells()
        {
            OpenCells = NextOpenCells;
            NextOpenCells = new List<Position>();
            Wave++;
        }

        public MoveStatus ApplyMove(Position oldPosition, int move, out int x, out int y)
        {
            if (!OpenCells.Contains(oldPosition))
                throw new ArgumentException();

            Position newPosition = oldPosition + _moves[move];

            x = newPosition.X + 1;
            y = newPosition.Y + 1;

            if (newPosition.X >= db.GetLength(1) ||
              newPosition.Y >= db.GetLength(0) ||
              newPosition.Y < 0 ||
              newPosition.X < 0 ||
              db[newPosition.Y, newPosition.X] == 1)
                return MoveStatus.Wall;

            if (db[newPosition.Y, newPosition.X] != 0)
                return MoveStatus.Thread;

            db[newPosition.Y, newPosition.X] = Wave + 2;
            if (!NextOpenCells.Contains(newPosition))
                NextOpenCells.Add(newPosition);
            lastVisited = newPosition;

            if (newPosition.X == db.GetLength(1) - 1 || newPosition.Y == db.GetLength(0) - 1 || newPosition.X == 0 || newPosition.Y == 0)
                return MoveStatus.Terminal;
            return MoveStatus.Free;
        }

        public void PrintRules(WriteAction wr)
        {
            int[] rules = GetRules();

            bool first = true;
            foreach (int rule in rules)
            {
                if (!first)
                    wr(", ");
                wr("R{0}", rule + 1);
                first = false;
            }
            wr(".\n");
        }

        public void PrintNodes(WriteAction wr)
        {
            Position[] positions = GetPositions();

            bool first = true;
            foreach(Position pos in positions)
            {
                if (!first)
                    wr(", ");
                wr("[X={0},Y={1}]", pos.X + 1, pos.Y + 1);
                first = false;
            }
            wr(".\n");
        }

        private int[] GetRules()
        {
            Position backup = lastVisited;

            List<int> rules = new List<int>();
            while (db[lastVisited.Y, lastVisited.X] != 2)
                for (int i = 0; i < 4; i++)
                {
                    Position pos = lastVisited + _moves[i];
                    try
                    {
                        if (db[pos.Y, pos.X] < db[lastVisited.Y, lastVisited.X] && db[pos.Y, pos.X] > 1)
                        {
                            lastVisited = pos;
                            rules.Add((i + 2) % 4);
                            break;
                        }
                    }
                    catch { }
                }

            int[] array = rules.ToArray();
            Array.Reverse(array);

            lastVisited = backup;
            return array;
        }

        private Position[] GetPositions()
        {
            Position backup = lastVisited;
            List<Position> positions = new List<Position>();
            positions.Add(lastVisited);
            while (db[lastVisited.Y, lastVisited.X] != 2)
                foreach (Position move in _moves)
                {
                    Position pos = lastVisited + move;
                    try
                    {
                        if (db[pos.Y, pos.X] < db[lastVisited.Y, lastVisited.X] && db[pos.Y, pos.X] > 1)
                        {
                            lastVisited = pos;
                            positions.Add(lastVisited);
                            break;
                        }
                    }
                    catch { }
                }

            Position[] array = positions.ToArray();
            Array.Reverse(array);

            lastVisited = backup;
            return array;
        }
    }
}
