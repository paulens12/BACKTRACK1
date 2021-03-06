﻿using System;
using System.Collections.Generic;
using System.Text;

namespace backtrack1
{
    public enum MoveStatus { Free, Wall, Thread, Error, Terminal };
    public class Maze
    {
        private class Position
        {
            public int x, y;
            public Position(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static Position operator +(Position a, Position b)
            {
                return new Position(a.x + b.x, a.y + b.y);
            }

            public static Position operator -(Position a, Position b)
            {
                return new Position(a.x - b.x, a.y - b.y);
            }
        }

        private Stack<int> _appliedMoves = new Stack<int>();

        private int[,] _db;
        private Position[] _moves =
        {
            new Position(-1, 0),
            new Position(0, -1),
            new Position(1, 0),
            new Position(0, 1),
        };
        private Position _currentPos;
        public int CurrentStep { get; private set; }
        private Position _initialPos;
        private int[] _stepArray = null;
        public int MoveCount
        {
            get
            {
                return _moves.Length;
            }
        }

        public Maze(int[,] db)
        {
            _db = db;
            for(int i=0; i<_db.GetLength(0); i++)
            {
                for(int j = 0; j<_db.GetLength(1); j++)
                {
                    if (_db[i, j] == 2)
                        _initialPos = _currentPos = new Position(j, i);
                }
            }
            CurrentStep = 2;
        }

        public int GetCurrentX()
        {
            return _currentPos.x + 1;
        }

        public int GetCurrentY()
        {
            return _currentPos.y + 1;
        }
        
        public MoveStatus ApplyMove(int i, out int x, out int y)
        {
            x = 0;
            y = 0;
            if (i < 0 || i >= _moves.Length)
                return MoveStatus.Error;

            Position newPos = _currentPos + _moves[i];

            x = newPos.x + 1;
            y = newPos.y + 1;

            if ((newPos.x == 0 || newPos.x == _db.GetLength(1) - 1 || newPos.y == 0 || newPos.y == _db.GetLength(0) - 1) && _db[newPos.y, newPos.x] == 0)
            {
                ApplyMove(i, newPos);
                return MoveStatus.Terminal;
            }

            if (_db[newPos.y, newPos.x] == 1)
                return MoveStatus.Wall;

            if (_db[newPos.y, newPos.x] != 0)
                return MoveStatus.Thread;

            ApplyMove(i, newPos);
            return MoveStatus.Free;
        }

        private void ApplyMove(int i, Position newPos)
        {
            _appliedMoves.Push(i);
            _currentPos = newPos;
            _db[newPos.y, newPos.x] = ++CurrentStep;
        }

        public bool UndoMove()
        {
            _db[_currentPos.y, _currentPos.x] = -1;
            if (_appliedMoves.Count == 0)
                return false;
            _currentPos -= _moves[_appliedMoves.Pop()];
            //_appliedMoves.Pop();
            CurrentStep--;
            return true;
        }
        
        public void Print(WriteAction wr)
        {
            wr("   ^ Y, V\n");
            StringBuilder sb = new StringBuilder();
            List<object> args = new List<object>();
            int width = _db.GetLength(1);
            for (int i = _db.GetLength(0) - 1; i >= 0; i--)
            {
                sb.AppendFormat("{0,3}|", i + 1);
                for (int j = 0; j < width; j++)
                {
                    sb.Append("{" + j + ",5}");
                    args.Add(_db[i, j]);
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

        private void GetStepArray()
        {
            if (_stepArray == null)
            {
                _stepArray = _appliedMoves.ToArray();
                Array.Reverse(_stepArray);
            }
        }

        public void PrintRules(WriteAction wr)
        {
            GetStepArray();
            bool first = true;
            for (int i = 0; i < _stepArray.Length; i++)
            {
                if (!first)
                    wr(", ");
                wr("R{0}", _stepArray[i] + 1);
                first = false;
            }
            wr(".\n");
        }

        public void PrintNodes(WriteAction wr)
        {
            GetStepArray();
            Position current = _initialPos;
            wr("[X={0},Y={1}]", current.x + 1, current.y + 1);
            for (int i = 0; i < _stepArray.Length; i++)
            {
                current += _moves[_stepArray[i]];
                wr(", [X={0},Y={1}]", current.x + 1, current.y + 1);
            }
            wr(".\n");
        }
    }
}
