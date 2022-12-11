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
        int [] rows = new int[9];
        int [] columns = new int[9];
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
                        // TODO new score update function, score will be update
                        if(Update_Score(blockrandom.x, blockrandom.y, i, j))
                        {
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
            int score = 0;
    
            for (int i = 0; i < 9; i++)
            {
                int[] row = new int[9];
                for (int j = 0; j < 9; j++)
                {
                    row[j] = cells[i, j].value;
                }
                rows[i] = 9 - row.Distinct().Count();
                score += rows[i];
            }
            // columns
            for (int i = 0; i < 9; i++)
            {
                int[] column = new int[9];
                for (int j = 0; j < 9; j++)
                {
                    column[j] = cells[j, i].value;
                }
                columns[i] = 9 - column.Distinct().Count();
                score += columns[i];
            }
            return score;

        }

        bool Update_Score(int block_x, int block_y, int cell_1, int cell_2)
        {
            int[] column = new int[9];
            int[] row = new int[9];

            int potential_x_value;
            int potential_y_value;
            int difference;

            int x_coordinaat_1 = block_x * 3 + cell_1 % 3;
            int y_coordinaat_1 = block_y * 3 + cell_1 / 3;

            int x_coordinaat_2 = block_x * 3 + cell_2 % 3;
            int y_coordinaat_2 = block_y * 3 + cell_2 / 3;

            // calculate the potential new scores for the available rows
            if (x_coordinaat_1 != x_coordinaat_2)
            {
                //calculate the score for both columns
                for (int j = 0; j < 9; j++)
                {
                    column[j] = cells[x_coordinaat_1, j].value;
                }
                potential_x_value = 9 - column.Distinct().Count();
                
                column = new int[9];
                for (int j = 0; j < 9; j++)
                {
                    column[j] = cells[x_coordinaat_2, j].value;
                }
                potential_x_value += 9 - column.Distinct().Count();

                difference = potential_x_value - columns[x_coordinaat_1] - columns[x_coordinaat_2];
            }
            else
            {
                //calculate the score for one column
                column = new int[9];
                for (int j = 0; j < 9; j++)
                {
                    column[j] = cells[x_coordinaat_1, j].value;
                }
                potential_x_value = 9 - column.Distinct().Count();

                difference = potential_x_value - columns[x_coordinaat_1];
            }

            // calculate the potential new scores for the available columns
            if (y_coordinaat_1 != y_coordinaat_2)
            {
                //calculate the score for both rows
                for (int j = 0; j < 9; j++)
                {
                    row[j] = cells[y_coordinaat_1, j].value;
                }
                potential_y_value = 9 - row.Distinct().Count();
                
                row = new int[9];
                for (int j = 0; j < 9; j++)
                {
                    column[j] = cells[y_coordinaat_2, j].value;
                }
                potential_y_value += 9 - row.Distinct().Count();

                difference += potential_y_value - rows[y_coordinaat_1] - rows[y_coordinaat_2];

            }
            else
            {
                //calculate the score for one row
                row = new int[9];
                for (int j = 0; j < 9; j++)
                {
                    row[j] = cells[y_coordinaat_1, j].value;
                }
                potential_y_value = 9 - row.Distinct().Count();

                difference += potential_y_value - rows[y_coordinaat_1];
            }

            //check if the new score is better, if so, return true
            if (difference < 0)
            {
                //Moeten deze hier al aangepast worden?
                // columns[x_coordinaat_1] = potential_x_value;
                // columns[x_coordinaat_2] = potential_x_value;
                // rows[y_coordinaat_1] = potential_y_value;
                // rows[y_coordinaat_2] = potential_y_value;

                return true;
            }
            else
            {
                return false;
            }
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