using System;
using System.Linq;
using System.Collections.Generic;

namespace SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] inputpuzzle = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
            Sudoku sudoku = new Sudoku(inputpuzzle);
            sudoku.Solve();
            sudoku.Print();
        }
    }
    class Sudoku
    {
        Cell[,] cells;
        Block[] blocks;

        public Sudoku(int[] inputpuzzle)
        {
            cells = new Cell[9, 9];
            blocks = new Block[9];
            for (int i = 0; i < 9; i++)
            {
                blocks[i] = new Block();
            }
            // fill cells
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (inputpuzzle[i * 9 + j] != 0)
                    {
                        cells[i, j] = new Cell(inputpuzzle[i * 9 + j], true);
                    }
                    else
                    {
                        cells[i, j] = new Cell();
                    }
                }
            }
            // fill blocks
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    blocks[i / 3 * 3 + j / 3].cells[i % 3, j % 3] = cells[i, j];
                }
            }
        }

        // TODO: Solve method
        public void Solve()
        {
            for(int i=0;i<9;i++)
            {
                blocks[i].Fill();
            }
        }

        public void Print()
        {
            for (int row = 0; row < 9; ++row)
            {
                if (row % 3 == 0)
                    Console.WriteLine(" -----------------------");

                for (int col = 0; col < 9; ++col)
                {
                    if (col % 3 == 0)
                        Console.Write("| ");

                    Console.Write(cells[row, col].value + " ");
                }

                Console.WriteLine("|");
            }

            Console.WriteLine(" -----------------------");
            Console.WriteLine();
        }
    }
    class Block
    {
        // 3x3 block of cells
        public Cell[,] cells = new Cell[3,3];
        public Block(Cell[,] cells)
        {
            this.cells = cells;
        }
        public Block()
        {
        }
        // fill with possible values
        public void Fill()
        {
            List<int> possibleValues = new List<int>();
            for (int i = 1; i < 10; i++)
            {
                possibleValues.Add(i);
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (cells[i, j].value != 0)
                    {
                        possibleValues.Remove(cells[i, j].value);
                    }
                }
            }
            foreach (Cell cell in cells)
            {
                if (cell.value == 0)
                {
                    // set value and remove from list
                    cell.value = possibleValues[0];
                    possibleValues.RemoveAt(0);
                }
            }
        }

        // only for debugging
        public void Print()
        {
            for (int row = 0; row < 3; ++row)
            {
                for (int col = 0; col < 3; ++col)
                {
                    Console.Write(cells[row, col].value + " ");
                }
                Console.WriteLine();
            }
        }
    }
    class Cell
    {
        public int value;
        public bool isFixed;
        public Cell()
        {
            value = 0;
            isFixed = false;
        }
        public Cell(int input)
        {
            value = input;
            isFixed = false;
        }
        public Cell(int input, bool isFixed)
        {
            this.isFixed = isFixed;
            value = input;
        }
    }
}