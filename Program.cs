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
        int iterations_until_randwalk = 10;

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
            // fill blocks with the numbers that are not fixed
            for(int i=0;i<9;i++)
            {
                blocks[i].Fill();
            }
            // fill cells with the numbers that are not fixed
            // loop over blocks and set cells to the cells in the blocks
            for(int i=0;i<9;i++)
            {
                for(int j=0;j<9;j++)
                {
                    cells[i, j] = blocks[i / 3 * 3 + j / 3].cells[i % 3, j % 3];
                }
            }
            // print score
            Console.WriteLine(Score());
            int prev_score = 0;
            int iterationssame = 0;
            while (Score() > 0)
            {
                // select random block and try all swaps, pick the best one
                Random rnd = new Random();
                int block = rnd.Next(0, 9);
                int bestscore = Score();
                int besti = 0;
                int bestj = 0;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        blocks[block].Swap(i, j);
                        if (Score() < bestscore)
                        {
                            bestscore = Score();
                            besti = i;
                            bestj = j;
                        }
                        blocks[block].Swap(i, j);
                    }
                }
                blocks[block].Swap(besti, bestj);
                // print score
                Console.WriteLine(Score());
                // if score is the same as before check if we should do a random walk
                if (Score() == prev_score)
                {
                    iterationssame++;
                    if (iterationssame >= iterations_until_randwalk)
                    {
                        // TODO random walk
                        break;
                        iterationssame = 0;
                    }
                }
                else
                {
                    iterationssame = 0;
                }
                prev_score = Score();
            }
        }

        public int Score()
        {
            // loop over rows and columns and check how many numbers are missing
            int score = 0;
            // rows
            for (int i = 0; i < 9; i++)
            {
                int[] row = new int[9];
                for (int j = 0; j < 9; j++)
                {
                    row[j] = cells[i, j].value;
                }
                score += 9 - row.Distinct().Count();
            }
            // columns
            for (int i = 0; i < 9; i++)
            {
                int[] column = new int[9];
                for (int j = 0; j < 9; j++)
                {
                    column[j] = cells[j, i].value;
                }
                score += 9 - column.Distinct().Count();
            }
            return score;
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

        public void Swap(int i, int j)
        {
            // swap cells[i,j] and cells[j,i] if they are not fixed
            if(!cells[i / 3, i % 3].isFixed && !cells[j / 3, j % 3].isFixed)
            {
                int temp = cells[i / 3, i % 3].value;
                cells[i / 3, i % 3].value = cells[j / 3, j % 3].value;
                cells[j / 3, j % 3].value = temp;
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