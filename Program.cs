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
        Cell[,] cells = new Cell[9, 9];
        Block[,] blocks = new Block[3, 3];
        int iterations_until_randwalk = 100;
        int randwalkLength = 5;
        int score = 0;

        public Sudoku(int[] inputpuzzle)
        {
            // create block objects
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    blocks[i, j] = new Block();
                }
            }
            // fill cells
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    // cells that are given in the puzzle are fixed
                    if (inputpuzzle[i * 9 + j] != 0)
                    {
                        cells[i, j] = new Cell(inputpuzzle[i * 9 + j], isFixed : true);
                    }
                    else
                    {
                        cells[i, j] = new Cell();
                    }
                    // add cell to block
                    blocks[i / 3, j / 3].cells[i % 3, j % 3] = cells[i, j];
                }
            }
        }

        // TODO: Solve method
        public void Solve()
        {
            // fill blocks with the numbers that are not fixed
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    blocks[i, j].Fill();
                }
            }
            // intialize score
            score = Score();
            Console.WriteLine(score);
            int prev_score = 0;
            int iterationssame = 0;
            while (score > 0)
            {
                // select random block and try all swaps, pick the best one
                Random rnd = new Random();
                (int x, int y) blockrandom = (rnd.Next(0, 3), rnd.Next(0, 3));
                int bestscore = score;
                int besti = 0;
                int bestj = 0;
                // for every cell in block
                for (int i = 0; i < 9; i++)
                {
                    // swap with every other cell
                    for (int j = 0; j < 9; j++)
                    {
                        blocks[blockrandom.x, blockrandom.y].Swap(i, j);
                        // calculate score and check if it is better
                        int newscore = Score();
                        if (newscore < bestscore)
                        {
                            bestscore = newscore;
                            besti = i;
                            bestj = j;
                        }
                        blocks[blockrandom.x, blockrandom.y].Swap(i, j);
                    }
                }
                // do the best swap
                blocks[blockrandom.x, blockrandom.y].Swap(besti, bestj);
                score = bestscore;
                Console.WriteLine(score);
                // if score is the same as before check if we should do a random walk
                if (score == prev_score)
                {
                    iterationssame++;
                    if (iterationssame >= iterations_until_randwalk)
                    {
                        RandomWalk();
                        iterationssame = 0;
                    }
                }
                else
                {
                    // if score is changed reset counter
                    iterationssame = 0;
                }
                prev_score = score;
            }
        }

        int Score()
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

        public void RandomWalk()
        {
            // do a random walk
            Random rnd = new Random();
            for (int i = 0; i < randwalkLength; i++)
            {
                (int x, int y) blockrandom = (rnd.Next(0, 3), rnd.Next(0, 3));
                int cell1random = rnd.Next(0, 9);
                int cell2random = rnd.Next(0, 9);
                blocks[blockrandom.x, blockrandom.y].Swap(cell1random, cell2random);
            }
            score = Score();
        }

        public void Print()
        {
            for (int row = 0; row < 9; ++row)
            {
                // divider between blocks
                if (row % 3 == 0)
                    Console.WriteLine(" -----------------------");

                for (int col = 0; col < 9; ++col)
                {
                    if (col % 3 == 0)
                    // divider between blocks
                        Console.Write("| ");

                    // print cell
                    Console.Write(cells[row, col].value + " ");
                }
                // edge of the board
                Console.WriteLine("|");
            }
            // edge of the board
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
            // add all possible values to the list
            List<int> possibleValues = new List<int>();
            for (int i = 1; i < 10; i++)
            {
                possibleValues.Add(i);
            }
            // remove the values that are already in the block
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
            // fill the empty cells with the possible values
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