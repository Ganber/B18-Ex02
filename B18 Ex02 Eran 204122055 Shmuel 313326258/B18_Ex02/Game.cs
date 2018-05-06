using System;

namespace B18_Ex02
{
    public class Game
    {
        public const int BIG_BOARD_SIZE = 10;
        public const int MEDIUM_BOARD_SIZE = 8;
        public const int SMALL_BOARD_SIZE = 6;

        public enum Pieces { White = 'O', WhiteKing = 'U', Black = 'X', BlackKing = 'K', EmptyPiece = ' ' };

        public enum opponent { Computer = 1, Human = 2 };

        Board m_Checkers = new Board();
        bool m_GameOver = false;
        private HumanPlayer m_playerOne = new HumanPlayer();
        private HumanPlayer m_playerTwo = new HumanPlayer();

        public void run()
        {
            initGame();
            HumanPlayer CurrentPlayer = m_playerOne;
            HumanPlayer OpponentPlayer = m_playerTwo;
            m_Checkers.drawBoard();
            int OpponentPlayerSolidersCounter = 0;
            Move CurrentMove = null;
            bool isSequenceEating = false;

            while (!m_GameOver)
            {
                UI.DisplayCurrentPlayerMessage(CurrentPlayer);

                string nextMoveInputString = Console.ReadLine();
                Move lastMove = CurrentMove;
                CurrentMove = new Move(nextMoveInputString);

                while (!isStringInputLegal(nextMoveInputString) || !isValidMove(CurrentMove, lastMove, isSequenceEating, CurrentPlayer))
                {
                    if (!isStringInputLegal(nextMoveInputString))
                        UI.DisplayIncorrectInputMessage();
                    else
                        UI.DisplayCantMoveHereMessage();

                    nextMoveInputString = Console.ReadLine();
                    CurrentMove = new Move(nextMoveInputString);
                }

                OpponentPlayerSolidersCounter = getPlayerSolidersCount(OpponentPlayer);
                makeMove(CurrentMove);

                m_Checkers.clearBoard();
                m_Checkers.drawBoard();

                isSequenceEating = false;
                if (OpponentPlayerSolidersCounter == getPlayerSolidersCount(OpponentPlayer))
                {
                    //not an Eat move.
                    swapCurrentPlayer(ref CurrentPlayer, ref OpponentPlayer);
                }
                else if (!isAbleToEat(CurrentMove.NextCoulmn, CurrentMove.NextRow, true))
                {
                    //cant eat more.
                    swapCurrentPlayer(ref CurrentPlayer, ref OpponentPlayer);
                }
                else
                {
                    //we made eat move, and we can eat more.
                    isSequenceEating = true;
                }
            }

            UI.GameOverMessage(); //TODO: exit game
        }

        private void makeMove(Move i_currentMove)
        {
            int currentRow = i_currentMove.CurrentRow;
            int currentCoulmn = i_currentMove.CurrentCoulmn;
            int nextRow = i_currentMove.NextRow;
            int nextCoulmn = i_currentMove.NextCoulmn;

            if (m_Checkers.GameBoard[currentCoulmn, currentRow].Value == Pieces.White)
            {
                if (nextRow == m_Checkers.GameBoard.GetLength(0) - 1)
                    m_Checkers.GameBoard[currentCoulmn, currentRow].Value = Pieces.WhiteKing;
            }

            if (m_Checkers.GameBoard[currentCoulmn, currentRow].Value == Pieces.Black)
            {
                if (nextRow == 0)
                    m_Checkers.GameBoard[currentCoulmn, currentRow].Value = Pieces.BlackKing;
            }

            m_Checkers.GameBoard[nextCoulmn, nextRow].Value = m_Checkers.GameBoard[currentCoulmn, currentRow].Value;
            m_Checkers.GameBoard[currentCoulmn, currentRow].Value = Pieces.EmptyPiece;

            if (Math.Abs(i_currentMove.CurrentCoulmn - i_currentMove.NextCoulmn) == 2)
            { //this mean we eat, and we need now to delete the opponent.
                m_Checkers.GameBoard[(nextCoulmn + currentCoulmn) / 2, (nextRow + currentRow) / 2].Value = Pieces.EmptyPiece;
            }


        }

        private bool isAbleToEat(int currentCoulmn, int currentRow, bool i_IsIntEatingSequence)
        {
            bool ableToEat = false;
            Cell currentCell = m_Checkers.GameBoard[currentCoulmn, currentRow];
            int boardSize = m_Checkers.GameBoard.GetLength(0);

            if (currentCell.isWhite() || currentCell.isKing() || i_IsIntEatingSequence)
            {
                if (currentRow + 2 < boardSize && currentCoulmn + 2 < boardSize)
                {
                    if (checkIfOpponent(currentCell, m_Checkers.GameBoard[currentCoulmn + 1, currentRow + 1]) &&
                        m_Checkers.GameBoard[currentCoulmn + 2, currentRow + 2].isEmpty())
                        ableToEat = true;
                }

                if (currentRow + 2 < boardSize && currentCoulmn >= 2)
                {
                    if (checkIfOpponent(currentCell, m_Checkers.GameBoard[currentCoulmn - 1, currentRow + 1]) &&
                        m_Checkers.GameBoard[currentCoulmn - 2, currentRow + 2].isEmpty())
                        ableToEat = true;
                }
            }

            if (currentCell.isBlack() || currentCell.isKing() || i_IsIntEatingSequence)
            {
                if (currentRow >= 2 && currentCoulmn >= 2)
                {
                    if (checkIfOpponent(currentCell, m_Checkers.GameBoard[currentCoulmn - 1, currentRow - 1]) &&
                        m_Checkers.GameBoard[currentCoulmn - 2, currentRow - 2].isEmpty())
                        ableToEat = true;
                }

                if (currentRow >= 2 && currentCoulmn + 2 < boardSize)
                {
                    if (checkIfOpponent(currentCell, m_Checkers.GameBoard[currentCoulmn + 1, currentRow - 1]) &&
                        m_Checkers.GameBoard[currentCoulmn + 2, currentRow - 2].isEmpty())
                        ableToEat = true;
                }
            }

            return ableToEat;
        }
        private bool checkIfOpponent(Cell i_cellOne, Cell i_cellTwo)
        {
            bool res = false;
            if (i_cellOne.isBlack() && i_cellTwo.isWhite())
                res = true;

            if (i_cellTwo.isBlack() && i_cellOne.isWhite())
                res = true;

            return res;
        }

        private bool isValidMove(Move i_currentMove, Move i_lastMove, bool i_SequenceEat, HumanPlayer i_currentPlayer)
        {
            bool isThisValidMove = false;

            Cell currentCell = m_Checkers.GameBoard[i_currentMove.CurrentCoulmn, i_currentMove.CurrentRow];
            if (i_SequenceEat)
            {
                if (i_lastMove.NextCoulmn == i_currentMove.CurrentCoulmn && i_lastMove.NextRow == i_currentMove.CurrentRow)
                {
                    if (isPlayerDoEatMove(i_currentMove))
                    {
                        isThisValidMove = true;
                    }
                }
            }
            else
            {
                if (checkIfPlayerCanEat(i_currentPlayer))
                {
                    if (isPlayerDoEatMove(i_currentMove))
                        isThisValidMove = true;
                }


                else
                {
                    //if we cant eat, need to be valid move.
                    if (currentCell.isKing())
                    {
                        if (Math.Abs(i_currentMove.NextRow - i_currentMove.CurrentRow) == 1
                         && Math.Abs(i_currentMove.CurrentCoulmn - i_currentMove.NextCoulmn) == 1)
                        {
                            isThisValidMove = true;
                        }
                    }
                    else if (currentCell.isWhite())
                    {
                        if (i_currentMove.NextRow - i_currentMove.CurrentRow == 1
                         && Math.Abs(i_currentMove.CurrentCoulmn - i_currentMove.NextCoulmn) == 1)
                        {
                            isThisValidMove = true;
                        }
                    }
                    else if (currentCell.isBlack())
                    {
                        if (i_currentMove.CurrentRow - i_currentMove.NextRow == 1
                         && Math.Abs(i_currentMove.CurrentCoulmn - i_currentMove.NextCoulmn) == 1)
                        {
                            isThisValidMove = true;
                        }
                    }
                }
            }

            //check that we not overrider another piece.
            if (m_Checkers.GameBoard[i_currentMove.NextCoulmn, i_currentMove.NextRow].Value != Pieces.EmptyPiece)
                isThisValidMove = false;

            //now we check if the player is moving hes soilders !
            if (i_currentPlayer.IsWhite)
            {
                if (m_Checkers.GameBoard[i_currentMove.CurrentCoulmn, i_currentMove.CurrentRow].Value == Pieces.Black
                    || m_Checkers.GameBoard[i_currentMove.CurrentCoulmn, i_currentMove.CurrentRow].Value == Pieces.BlackKing)
                    isThisValidMove = false;

            }
            else
            {
                if (m_Checkers.GameBoard[i_currentMove.CurrentCoulmn, i_currentMove.CurrentRow].Value == Pieces.White
                  || m_Checkers.GameBoard[i_currentMove.CurrentCoulmn, i_currentMove.CurrentRow].Value == Pieces.WhiteKing)
                    isThisValidMove = false;
            }

            return isThisValidMove;
        }

        private bool isPlayerDoEatMove(Move i_currentMove)
        {
            bool eatRes = false;
            int currentRow = i_currentMove.CurrentRow;
            int currentCoulmn = i_currentMove.CurrentCoulmn;
            int nextRow = i_currentMove.NextRow;
            int nextCoulmn = i_currentMove.NextCoulmn;

            if (Math.Abs(i_currentMove.CurrentCoulmn - i_currentMove.NextCoulmn) == 2)
            {
                Cell currentCell = m_Checkers.GameBoard[currentCoulmn, currentRow];
                Cell targetCell = m_Checkers.GameBoard[(nextCoulmn + currentCoulmn) / 2, (nextRow + currentRow) / 2];

                if (checkIfOpponent(currentCell, targetCell))
                    eatRes = true;
            }

            return eatRes;
        }

        private bool checkIfPlayerCanEat(HumanPlayer i_currentPlayer)
        {
            int boardSize = m_Checkers.GameBoard.GetLength(0);
            bool canEatRes = false;
            for (int coulmn = 0; coulmn < boardSize; coulmn++)
            {
                for (int row = 0; row < boardSize; row++)
                {
                    if (i_currentPlayer.IsWhite)
                    {
                        if (m_Checkers.GameBoard[coulmn, row].isWhite())
                            if (isAbleToEat(coulmn, row, false))
                                canEatRes = true;
                    }
                    else
                    {
                        if (m_Checkers.GameBoard[coulmn, row].isBlack())
                            if (isAbleToEat(coulmn, row, false))
                                canEatRes = true;
                    }
                }
            }

            return canEatRes;
        }

        private void swapCurrentPlayer(ref HumanPlayer currentPlayer, ref HumanPlayer opponentPlayer)
        {
            if (currentPlayer == m_playerOne)
            {
                currentPlayer = m_playerTwo;
                opponentPlayer = m_playerOne;
            }
            else
            {
                currentPlayer = m_playerOne;
                opponentPlayer = m_playerTwo;
            }
        }

        private void initGame()
        {
            m_playerOne.Name = UI.GetPlayerNameFromInput();

            uint boardSize = UI.getSizeFromUserInput();
            opponent userOpponent = UI.GetUserRival();

            if (userOpponent == opponent.Human)
            {
                m_playerTwo.IsWhite = false;
                m_playerTwo.Name = UI.GetPlayerNameFromInput();
            }

            m_Checkers.clearBoard();
            m_Checkers.createEmptyGameBoard(boardSize);

            for (int column = 0; column < boardSize; column = column + 2)
            {
                for (int row = 0; row < boardSize / 2 - 1; row++)
                {
                    if (row % 2 == 0)
                    {
                        m_Checkers.GameBoard[column, row].Value = Pieces.White;
                        m_Checkers.GameBoard[column + 1, boardSize - row - 1].Value = Pieces.Black;
                    }
                    else
                    {
                        m_Checkers.GameBoard[column + 1, row].Value = Pieces.White;
                        m_Checkers.GameBoard[column, boardSize - row - 1].Value = Pieces.Black;
                    }
                }
            }
        }

        private bool isStringInputLegal(string i_moveInput)
        {
            int column = m_Checkers.GameBoard.GetLength(0);

            return (i_moveInput.Length == 5
                    && i_moveInput[0] >= 'A' && i_moveInput[0] <= (column + Board.COLUMN_CAPITAL_LETTER)
                    && i_moveInput[1] >= 'a' && i_moveInput[1] <= (column + Board.ROW_SMALL_LETTER)
                    && i_moveInput[2] == '>'
                    && i_moveInput[3] >= 'A' && i_moveInput[3] <= (column + Board.COLUMN_CAPITAL_LETTER)
                    && i_moveInput[4] >= 'a' && i_moveInput[4] <= (column + Board.ROW_SMALL_LETTER));
        }

        private void convertStringInputToIntegers(string i_MoveInput, out int i_currentCol, out int i_currentRow, out int i_nextCol, out int i_nextRow)
        {
            i_currentCol = i_MoveInput[0] - Board.COLUMN_CAPITAL_LETTER;
            i_currentRow = i_MoveInput[1] - Board.ROW_SMALL_LETTER;

            i_nextCol = i_MoveInput[3] - Board.COLUMN_CAPITAL_LETTER;
            i_nextRow = i_MoveInput[4] - Board.ROW_SMALL_LETTER;
        }

        private int getPlayerSolidersCount(HumanPlayer i_currentPlayer)
        {
            int solidersCount = 0;
            int boardSize = m_Checkers.GameBoard.GetLength(0);

            for (int currentCol = 0; currentCol < boardSize; currentCol++)
            {
                for (int currentRow = 0; currentRow < boardSize; currentRow++)
                {
                    if (i_currentPlayer.IsWhite)
                    {
                        if (m_Checkers.GameBoard[currentCol, currentRow].Value == Pieces.White
                            || m_Checkers.GameBoard[currentCol, currentRow].Value == Pieces.WhiteKing)
                            solidersCount++;
                    }
                    else
                    {
                        if (m_Checkers.GameBoard[currentCol, currentRow].Value == Pieces.Black
                            || m_Checkers.GameBoard[currentCol, currentRow].Value == Pieces.BlackKing)
                            solidersCount++;
                    }
                }
            }
            return solidersCount;
        }

    }
}
