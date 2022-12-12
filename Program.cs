﻿using System;
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

            // test the score_after_swap function
            Console.WriteLine(sudoku.Score_After_Swap(0, 0, 0, 1));
        }
    }
    class Sudoku
    {
        Cell[,] cells = new Cell[9, 9];
        Block[,] blocks = new Block[3, 3];
        int iterations_until_randwalk = 100;
        int randwalkLength = 5;
        int [] row_scores = new int[9];
        int [] column_scores = new int[9];
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
                DoBestSwap(blocks[blockrandom.x, blockrandom.y], blockrandom.x, blockrandom.y);
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

        // find best swap in block
        void DoBestSwap(Block block, int x, int y)
        {
            int bestscore = score;
            int besti = 0;
            int bestj = 0;
            // for every cell in block
            for (int i = 0; i < 9; i++)
            {
                // swap with every other cell
                for (int j = 0; j < 9; j++)
                {
                    block.Swap(i, j);
                    // calculate new score and check if it is better
                    int newscore = Score_After_Swap(x, y, i, j);
                    if (newscore < bestscore)
                    {
                        bestscore = newscore;
                        besti = i;
                        bestj = j;
                    }
                    block.Swap(i, j);
                }
            }
            // do the best swap
            block.Swap(besti, bestj);
            // update score
            score = bestscore;
            // update row and column scores
            row_scores[y * 3 + besti / 3] = Calc_Row(y * 3 + besti / 3);
            column_scores[x * 3 + besti % 3] = Calc_Column(x * 3 + besti % 3);
            row_scores[y * 3 + bestj / 3] = Calc_Row(y * 3 + bestj / 3);
            column_scores[x * 3 + bestj % 3] = Calc_Column(x * 3 + bestj % 3);
            Console.WriteLine(score);
        }

        int Score()
        {
            int score = 0;
            // row_scores
            for (int i = 0; i < 9; i++)
            {
                row_scores[i] = Calc_Row(i);
                score += row_scores[i];
            }
            // column_scores
            for (int i = 0; i < 9; i++)
            {
                column_scores[i] = Calc_Column(i);
                score += column_scores[i];
            }
            return score;
        }

        int Score_After_Swap(int block_x, int block_y, int cell_1, int cell_2)
        {
            int potential_x_value;
            int potential_y_value;
            int difference = 0;

            int x1 = block_x * 3 + cell_1 % 3;
            int y1 = block_y * 3 + cell_1 / 3;

            int x2 = block_x * 3 + cell_2 % 3;
            int y2 = block_y * 3 + cell_2 / 3;

            // calculate the potential new scores for the available row_scores
            if (x1 != x2)
            {
                //calculate the score for both column_scores
                potential_x_value = Calc_Column(x1);
                potential_x_value += Calc_Column(x2);
                difference += potential_x_value - column_scores[x1] - column_scores[x2];
            }
            else
            {
                //calculate the score for one column
                potential_x_value = Calc_Column(x1);
                difference += potential_x_value - column_scores[x1];
            }
            // calculate the potential new scores for the available column_scores
            if (y1 != y2)
            {
                //calculate the score for both row_scores
                potential_y_value = Calc_Row(y1);
                potential_y_value += Calc_Row(y2);
                difference += potential_y_value - row_scores[y1] - row_scores[y2];
            }
            else
            {
                //calculate the score for one row
                potential_y_value = Calc_Row(y1);
                difference += potential_y_value - row_scores[y1];
            }
            return score + difference;
        }

        public int Calc_Row(int row)
        {
            int[] rowarray = new int[9];
            for (int j = 0; j < 9; j++)
            {
                rowarray[j] = cells[row, j].value;
            }
            return 9 - rowarray.Distinct().Count();
        }
        public int Calc_Column(int column)
        {
            int[] columnarray = new int[9];
            for (int j = 0; j < 9; j++)
            {
                columnarray[j] = cells[j, column].value;
            }
            return 9 - columnarray.Distinct().Count();
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
                if (!blocks[blockrandom.x, blockrandom.y].Swap(cell1random, cell2random))
                {
                    i--;
                }
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

        public bool Swap(int i, int j)
        {
            // swap cells[i,j] and cells[j,i] if they are not fixed
            if(!cells[i / 3, i % 3].isFixed && !cells[j / 3, j % 3].isFixed)
            {
                int temp = cells[i / 3, i % 3].value;
                cells[i / 3, i % 3].value = cells[j / 3, j % 3].value;
                cells[j / 3, j % 3].value = temp;
                return true;
            }
            return false;
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