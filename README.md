# SudokuSolver
A computational intelligence-based Sudoku solver for Utrecht University.

# Input
81 numbers separated by spaces (0 for empty)
# Output:
Solved Sudoku puzzle

# How to use
Open the Program.cs file and enter your sudoku's

# How it works
Reads the Sudoku puzzle
Keeps track of fixed cells
Fills 3x3 blocks with numbers they don't contain
Selects a random block
Tries all possible swaps of 2 non-fixed numbers in the block
Selects the best swap if it results in a lower or equal score
If the score remains unchanged after multiple iterations, performs a random walk using search operators and returns to selecting random blocks
Prints the solved Sudoku puzzle
Score calculation: Sum of all missing numbers in rows and columns. A solved Sudoku has a score of 0.
