# SudokuSolver
SudokuSolver for Computational Intelligence at Utrecht University

input: 81 numbers seperated by spaces (0 for empty)
output: solved sudoku

# How to use
simply run the program.cs and enter the puzzle

# How it works
1: read the sudoku\
2: keep track of which cells are fixed\
3: fill the 3x3 blocks with the numbers they don't contain\
4: choose a random block\
5: try all possible swaps in the block (of 2 non fixed numbers)\
6: choose the best swap if it gives a higher or equal score than before the swap\
7: if the score is the same after x iterations of doing this, do a randomwalk of S times a searchoperator (and return to chosing random blocks\
8: Print the solved sudoku

score calculation:\
sum of all the missing numbers of the rows and columns. A solved sudoku has a score of 0.
