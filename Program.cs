using System;
using System.Linq;
using System.IO;

namespace SudokuSolver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a new text file to write the performance tests to
            string filePath = "PerformanceTests.csv";
            File.Create(filePath).Close();
            StreamWriter sw = new StreamWriter(filePath);

            // Create string array for the 5 different sudoku's
            string[] sudokuStrings = {
            "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0",
            "2 0 0 0 8 0 3 0 0 0 6 0 0 7 0 0 8 4 0 3 0 5 0 0 2 0 9 0 0 0 1 0 5 4 0 8 0 0 0 0 0 0 0 0 0 4 0 2 7 0 6 0 0 0 3 0 1 0 0 7 0 4 0 7 2 0 0 4 0 0 6 0 0 0 4 0 1 0 0 0 3",
            "0 0 0 0 0 0 9 0 7 0 0 0 4 2 0 1 8 0 0 0 0 7 0 5 0 2 6 1 0 0 9 0 4 0 0 0 0 5 0 0 0 0 0 4 0 0 0 0 5 0 7 0 0 9 9 2 0 1 0 8 0 0 0 0 3 4 0 5 9 0 0 0 5 0 7 0 0 0 0 0 0",
            "0 3 0 0 5 0 0 4 0 0 0 8 0 1 0 5 0 0 4 6 0 0 0 0 0 1 2 0 7 0 5 0 2 0 8 0 0 0 0 6 0 3 0 0 0 0 4 0 1 0 9 0 3 0 2 5 0 0 0 0 0 9 8 0 0 1 0 2 0 6 0 0 0 8 0 0 6 0 0 2 0",
            "0 2 0 8 1 0 7 4 0 7 0 0 0 0 3 1 0 0 0 9 0 0 0 2 8 0 5 0 0 9 0 4 0 0 8 7 4 0 0 2 0 8 0 0 3 1 6 0 0 3 0 2 0 0 3 0 2 7 0 0 0 6 0 0 0 5 6 0 0 0 0 8 0 7 6 0 5 1 0 9 0"
            };

            // Create two int arrays to store the different values for the amount of iterations before random walk and the random walk distance
            int[] iterationsBeforeRandomWalk = { 5, 10, 20, 30, 40, 50 };
            int[] randomWalkDistance = { 2, 3, 4, 5, 8, 10};

            // Write the header of the csv file
            sw.WriteLine("Sudoku, Iterations before random walk, Random walk distance, Average loops");

            for (int i = 0; i < iterationsBeforeRandomWalk.Length; i++)
            {
                for (int j = 0; j < randomWalkDistance.Length; j++)
                {
                    Console.WriteLine(j);
                    for (int k = 0; k < sudokuStrings.Length; k++)
                    {
                        int averageLoops = 0;
                        for (int l = 0; l < 10; l++)
                        {
                            Sudoku sudoku = new Sudoku(sudokuStrings[k]);
                            sudoku.IterationsBeforeRandomWalk = iterationsBeforeRandomWalk[i];
                            sudoku.RandomWalkDistance = randomWalkDistance[j];
                            int loops = sudoku.Solve();
                            averageLoops += loops;
                        }
                        averageLoops /= 10;

                        sw.WriteLine(k + 1 + ", " + iterationsBeforeRandomWalk[i] + ", " + randomWalkDistance[j] + ", " + averageLoops);
                    }
                }
            }
            
            sw.Close();
        }
    }

    class Sudoku
    {
        private int[] rowValues = new int[9];
        private int[] columnValues = new int[9];
        private Cell[,] cells = new Cell[9, 9];
        public int IterationsBeforeRandomWalk = 10;
        public int RandomWalkDistance = 5;

        class Cell
        {
            public int Value { get; set; }
            public bool IsFixed { get; set; }

        }

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

                // Loop through the array and fill in the missing values
                for (int i = 0; i < 9; i++)
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

        public int Solve()
        {
            int value = EvaluationValue();
            int oldValue = value;
            int iterations = 0;
            int loopCounter = 0;

            // While the sudoku is not solved
            while (value != 0)
            {
                TrySwap trySwap = new TrySwap(value);

                // Select a random block
                int block = new Random().Next(0, 9);

                // Try all the different swap combinations in the block and execute the one that gives the lowest evaluation value
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        // Save the row and column index of the cells
                        trySwap.UpdateRowColumns(block, i, j);

                        // Save the row and column values before the swap
                        trySwap.SaveOldValues(rowValues, columnValues);

                        if (SwapCells(block, i, j) && i != j)
                        {
                            // Save the row and column values after the swap
                            trySwap.SaveNewValues(EvaluationRowColumn(trySwap.rowcolumnindexes[0], true), EvaluationRowColumn(trySwap.rowcolumnindexes[1], false), EvaluationRowColumn(trySwap.rowcolumnindexes[2], true), EvaluationRowColumn(trySwap.rowcolumnindexes[3], false));
                            trySwap.EvaluateNewValue(i, j);
                            SwapCells(block, i, j);
                        }
                    }
                }

                // If the best swap was found, execute it
                if (trySwap.BestValue < value)
                    SwapCells(block, trySwap.Besti, trySwap.Bestj);
                    
                    // Update the row and column values
                    trySwap.UpdateRowColumns(block, trySwap.Besti, trySwap.Bestj);
                    trySwap.SaveNewValues(EvaluationRowColumn(trySwap.rowcolumnindexes[0], true), EvaluationRowColumn(trySwap.rowcolumnindexes[1], false), EvaluationRowColumn(trySwap.rowcolumnindexes[2], true), EvaluationRowColumn(trySwap.rowcolumnindexes[3], false));
                    trySwap.ExecuteSwap(ref rowValues, ref columnValues);

                    // Update the evaluation value
                    value = trySwap.BestValue;

                // If the evaluation value is the same as the previous iteration, increase the number of iterations
                if (value == oldValue)
                    iterations++;
                else
                    iterations = 0;

                // If the number of iterations is equal to 10, do a random walk
                if (iterations == IterationsBeforeRandomWalk)
                {
                    trySwap.BestValue = value;

                    // Execute 5 random walks
                    for (int walk = 0; walk < RandomWalkDistance; walk++)
                    {
                        // Select a random block
                        int rndblock = new Random().Next(0, 9);

                        // Select a random cell in the block
                        int i = new Random().Next(0, 9);

                        // Select a random cell in the block
                        int j = new Random().Next(0, 9);

                        // Save the row and column index of the cells
                        trySwap.UpdateRowColumns(rndblock, i, j);

                        // Save the row and column values before the swap
                        trySwap.SaveOldValues(rowValues, columnValues);

                        // Swap the two cells
                        SwapCells(rndblock, i, j);

                        trySwap.SaveNewValues(EvaluationRowColumn(trySwap.rowcolumnindexes[0], true), EvaluationRowColumn(trySwap.rowcolumnindexes[1], false), EvaluationRowColumn(trySwap.rowcolumnindexes[2], true), EvaluationRowColumn(trySwap.rowcolumnindexes[3], false));
                        trySwap.ExecuteSwap(ref rowValues, ref columnValues);
                        value = trySwap.CalculateNewValue();
                        trySwap.BestValue = value;
                    }
                    iterations = 0;
                }

                if (loopCounter > 100000)
                    return loopCounter;

                oldValue = value;
                loopCounter++;
            }

            return loopCounter;
        }

        // Swap two cells in the same block, only allowing the swap if the cells are not fixed
        private bool SwapCells(int block, int i, int j)
        {
            int row = (block / 3) * 3;
            int col = (block % 3) * 3;

            // If the cells are not fixed, swap the values
            if (!cells[row + i / 3, col + i % 3].IsFixed && !cells[row + j / 3, col + j % 3].IsFixed)
            {
                int temp = cells[row + i / 3, col + i % 3].Value;
                cells[row + i / 3, col + i % 3].Value = cells[row + j / 3, col + j % 3].Value;
                cells[row + j / 3, col + j % 3].Value = temp;
                return true;
            }

            return false;
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