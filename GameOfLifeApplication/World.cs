using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeApplication
{
    public class World
    {
        public enum State { Dead = 0, Live = 1 }

        public readonly int Rows;
        public readonly int Columns;
        public readonly State[,] Grid;
        public readonly List<int[]> Survived;

        public World(int rows, int columns, int initLiveCells, List<int[]> initCells = null)
        {
            Rows = rows;
            Columns = columns;
            Grid = new State[rows, columns];

            Survived = new List<int[]>();
            if (initCells != null)
            {
                Survived.AddRange(initCells);
            }
            else
            {
                Randomise(initLiveCells);
            }

            UpdateGrid();
        }

        public bool Evolve()
        {
            var future = new List<int[]>();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    var currentCell = Grid[i, j];
                    // Count live neighbours of the current cell
                    var liveNeighbours = CountLiveNeighbours(i, j);
                    // Apply rules
                    var result = ApplyRules(currentCell, liveNeighbours);
                    // Save result
                    if (result == State.Live)
                        future.Add(new int[] { i, j });
                }
            }
            // Check if evolution is finished
            var finished = false;
            if (Survived != null)
            {
                finished = Survived.Zip(future, CompareArrays)
                                   .All(x => x);
            }
            Survived.Clear();
            Survived.AddRange(future);
            // Update the grid
            ClearGrid();
            UpdateGrid();

            return finished;
        }

        void Randomise(int initLiveCells)
        {
            var rnd = new Random();
            for (int i = 0; i < initLiveCells; i++)
            {
                var m = rnd.Next(Rows);
                var n = rnd.Next(Columns);
                var tuple = new int[] { m, n };
                if (Survived.Any(x => CompareArrays(tuple, x)))
                {
                    i--;
                    continue;
                }
                Survived.Add(tuple);
            }
        }
        internal int CountLiveNeighbours(int x, int y)
        {
            var neighbours = 0;
            for (int i = -1; i < 2; i++)
            {
                var k = x + i;

                for (int j = -1; j < 2; j++)
                {
                    var l = y + j;

                    if (k >= 0 && k < Rows && l >= 0 && l < Columns && (k != x || l != y))
                    {
                        neighbours += (int)Grid[k, l];
                    }
                }
            }
            return neighbours;
        }

        internal State ApplyRules(State currentCell, int liveNeighbours)
        {
            var result = State.Dead;
            if ((currentCell == State.Live && liveNeighbours == 2) || liveNeighbours == 3)
            {
                result = State.Live;
            }
            return result;
        }
        void ClearGrid()
        {
            Array.Clear(Grid, 0, Rows * Columns);
        }
        void UpdateGrid()
        {
            foreach (int[] indices in Survived)
            {
                Grid[indices[0], indices[1]] = State.Live;
            }
        }
        bool CompareArrays(int[] a1, int[] a2)
        {
            return a1.Zip(a2, (x, y) => x == y).All(x => x);
        }
    }
}
