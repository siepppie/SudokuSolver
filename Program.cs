using System;
using System.Linq;

namespace SudokuSolver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a new Sudoku based on user input
            Sudoku sudoku = new Sudoku(Console.ReadLine());
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
        int [] row_scores = new int[9];
        int [] column_scores = new int[9];
        int score = 0;

        class TrySwap
        {
            private int OriginalValue { get; set; }
            public int BestValue { get; set; }
            public int Besti { get; set; }
            public int Bestj { get; set; }

            public int[] rowcolumnindexes = new int[4];
            private int[] oldValues = new int[4];
            private int[] newValues = new int[4];

            // Constructor
            public TrySwap(int Value)
            {
                OriginalValue = Value;
                BestValue = Value;
                Besti = 0;
                Bestj = 0;
            }

            // Update rowcolumns
            public void UpdateRowColumns(int block, int i, int j)
            {
                rowcolumnindexes[0] = (block / 3) * 3 + i / 3;
                rowcolumnindexes[1] = (block % 3) * 3 + i % 3;
                rowcolumnindexes[2] = (block / 3) * 3 + j / 3;
                rowcolumnindexes[3] = (block % 3) * 3 + j % 3;
            }

            // Save the old values
            public void SaveOldValues(int[] rowValues, int[] columnValues)
            {
                oldValues[0] = rowValues[rowcolumnindexes[0]];
                oldValues[1] = columnValues[rowcolumnindexes[1]];
                oldValues[2] = rowValues[rowcolumnindexes[2]];
                oldValues[3] = columnValues[rowcolumnindexes[3]];
            }

            // Save the new values
            public void SaveNewValues(int a, int b, int c, int d)
            {
                newValues[0] = a;
                newValues[1] = b;
                newValues[2] = c;
                newValues[3] = d;
            }

            // Execute the swap
            public void ExecuteSwap(ref int[] rowValues, ref int[] columnValues)
            {
                rowValues[rowcolumnindexes[0]] = newValues[0];
                columnValues[rowcolumnindexes[1]] = newValues[1];
                rowValues[rowcolumnindexes[2]] = newValues[2];
                columnValues[rowcolumnindexes[3]] = newValues[3];
            }

            // Evaluate the new value
            public void EvaluateNewValue(int i, int j)
            {
                int newValue = OriginalValue - oldValues[0] - oldValues[1] - oldValues[2] - oldValues[3] + newValues[0] + newValues[1] + newValues[2] + newValues[3];

                // If the new value is better than the old value, save the new value
                if (newValue < BestValue)
                {
                    BestValue = newValue;
                    Besti = i;
                    Bestj = j;
                }
            }

            public int CalculateNewValue()
            {
                int newValue = BestValue - oldValues[0] - oldValues[1] - oldValues[2] - oldValues[3] + newValues[0] + newValues[1] + newValues[2] + newValues[3];
                return newValue;
            }

        }

        // Constructor
        public Sudoku(string input)
        {
            // Create int array of 81 characters where the input is split on spaces
            int[] values = input.Split(' ').Select(int.Parse).ToArray();

            // Loop through the array and assign the values to the cells
            for (int i = 0; i < 81; i++)
            {
                int row = i / 9;
                int col = i % 9;
                cells[row, col] = new Cell { Value = values[i], IsFixed = values[i] != 0 };
            }

            // Fill in the rest of the board by entering missing values in each block of 3x3 cells
            for (int block = 0; block < 9; block++)
            {
                int row = (block / 3) * 3;
                int col = (block % 3) * 3;

                // Create bool array of 9 elements
                bool[] numbers = new bool[9];

                // Check for each number in the block
                for (int i = 0; i < 9; i++)
                {
                    // If the cell is not empty, set the corresponding element in the array to true
                    if (cells[row + i / 3, col + i % 3].Value != 0)
                        numbers[cells[row + i / 3, col + i % 3].Value - 1] = true;
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
                    // If the cell is empty, fill in the missing value
                    if (cells[row + i / 3, col + i % 3].Value == 0)
                    {
                        // Loop through the array and find the missing value
                        for (int j = 0; j < 9; j++)
                        {
                            // If the value is not found, fill in the missing value
                            if (!numbers[j])
                            {
                                cells[row + i / 3, col + i % 3].Value = j + 1;
                                numbers[j] = true;
                                break;
                            }
                        }
                    }
                }
            }

        }

        public void Solve()
        {
            int bestscore = score;
            int besti = 0;
            int bestj = 0;
            // for every cell in block
            for (int i = 0; i < 9; i++)
            {
                TrySwap trySwap = new TrySwap(value);

                // Select a random block
                int block = new Random().Next(0, 9);

                // Try all the different swap combinations in the block and execute the one that gives the lowest evaluation value
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        // calculate new score and check if it is better
                        int newscore = Score_After_Swap(x, y, i, j);
                        if (newscore < bestscore)
                        {
                            // Save the row and column values after the swap
                            trySwap.SaveNewValues(EvaluationRowColumn(trySwap.rowcolumnindexes[0], true), EvaluationRowColumn(trySwap.rowcolumnindexes[1], false), EvaluationRowColumn(trySwap.rowcolumnindexes[2], true), EvaluationRowColumn(trySwap.rowcolumnindexes[3], false));
                            trySwap.EvaluateNewValue(i, j);
                            SwapCells(block, i, j);
                        }
                    }
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

        private int EvaluationValue()
        {
            int value = 0;

            // Loop through the rows and columns and add the evaluation values
            for (int i = 0; i < 9; i++)
            {
                rowValues[i] = EvaluationRowColumn(i, true);
                columnValues[i] = EvaluationRowColumn(i, false);
            }

            // Loop through row and column values and add them to the total evaluation value
            for (int i = 0; i < 9; i++)
            {
                value += rowValues[i];
                value += columnValues[i];
            }

            return value;
        }

        private int EvaluationRowColumn(int x, bool isRow)
        {
            int value = 0;

            // Create bool array of 9 elements
            bool[] numbers = new bool[9];

            // Loop through the row or column
            for (int i = 0; i < 9; i++)
            {
                // If the cell is not empty, set the corresponding element in the array to true
                if (isRow)
                {
                    if (cells[x, i].Value != 0)
                        numbers[cells[x, i].Value - 1] = true;
                }
                else
                {
                    if (cells[i, x].Value != 0)
                        numbers[cells[i, x].Value - 1] = true;
                }
            }

            // Loop through the array and count the number of missing values
            for (int i = 0; i < 9; i++)
            {
                if (!numbers[i])
                    value++;
            }

            return value;
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
                    Console.Write(cells[row, col].Value + " ");
                }
                // edge of the board
                Console.WriteLine("|");
            }
            // edge of the board
            Console.WriteLine(" -----------------------");
            Console.WriteLine();
        }
    }
}