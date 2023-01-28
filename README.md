# SudokuSolver
A computational intelligence-based Sudoku solver for Utrecht University.

## Input
81 numbers separated by spaces (0 for empty)
## Output:
Solved Sudoku puzzle

## How to use
Open the Program.cs file and enter your sudoku's

## How it works
1. Reads the Sudoku puzzle
2. Keeps track of fixed cells
3. Fills 3x3 blocks with numbers they don't contain
4. Selects a random block
5. Tries all possible swaps of 2 non-fixed numbers in the block
6. Selects the best swap if it results in a lower or equal score
7. If the score remains unchanged after multiple iterations, performs a random walk using search operators and returns to selecting random blocks
8. Prints the solved Sudoku puzzle

## Score calculation 
Sum of all missing numbers in rows and columns. A solved Sudoku has a score of 0.
