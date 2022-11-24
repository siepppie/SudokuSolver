using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// dit is een test of het werkt
namespace Sudoku
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read txt file and store in array
            string[] lines = System.IO.File.ReadAllLines("Sudoku_puzzels_5.txt");

            // Delete every line that is not a puzzle
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length != 162)
                {
                    lines[i] = "";
                }
            }

            // Count the number of puzzles
            int count = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != "")
                {
                    count++;
                }
            }

            // Create a new array with only the puzzles
            string[] puzzles = new string[count];
            int j = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != "")
                {
                    puzzles[j] = lines[i].Replace(" ", "");
                    j++;
                }
            }

            // Create a new Sudoku object for every puzzle
            Sudoku[] sudoku = new Sudoku[puzzles.Length];

            for (int i = 0; i < puzzles.Length; i++)
            {
                sudoku[i] = new Sudoku(puzzles[i]);
            }

            // Solve every puzzle
            for (int i = 0; i < sudoku.Length; i++)
            {
                sudoku[i].Solve();
            }

            // Print every puzzle
            for (int i = 0; i < sudoku.Length; i++)
            {
                sudoku[i].Display();
            }
        }
    }

    class Sudoku
    {
        private int[,] board = new int[9, 9];

        public Sudoku(string board)
        {
            int k = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    this.board[i, j] = (int)Char.GetNumericValue(board[k]);
                    k++;
                }
            }
        }

        public bool Solve()
        {
            return Solve(0, 0);
        }

        private bool Solve(int row, int col)
        {
            if (row == 9)
            {
                row = 0;
                if (++col == 9)
                    return true;
            }

            if (board[row, col] != 0)
                return Solve(row + 1, col);

            for (int val = 1; val <= 9; ++val)
            {
                if (Legal(row, col, val))
                {
                    board[row, col] = val;
                    if (Solve(row + 1, col))
                        return true;
                }
            }

            board[row, col] = 0;
            return false;
        }

        private bool Legal(int row, int col, int val)
        {
            for (int k = 0; k < 9; ++k)
                if (val == board[row, k])
                    return false;

            for (int k = 0; k < 9; ++k)
                if (val == board[k, col])
                    return false;

            int boxRowOffset = (row / 3) * 3;
            int boxColOffset = (col / 3) * 3;
            for (int i = 0; i < 3; ++i)
                for (int j = 0; j < 3; ++j)
                    if (val == board[boxRowOffset + i, boxColOffset + j])
                        return false;

            return true;
        }

        public void Display()
        {
            for (int row = 0; row < 9; ++row)
            {
                if (row % 3 == 0)
                    Console.WriteLine(" -----------------------");

                for (int col = 0; col < 9; ++col)
                {
                    if (col % 3 == 0)
                        Console.Write("| ");

                    Console.Write(board[row, col] != 0 ? board[row, col] + " " : "  ");
                }

                Console.WriteLine("|");
            }

            Console.WriteLine(" -----------------------");
            Console.WriteLine();
        }
    }
}