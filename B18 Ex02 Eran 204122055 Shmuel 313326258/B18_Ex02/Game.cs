namespace B18_Ex02
{
    using System;

    public class Game
    {
        public const int BIG_BOARD_SIZE = 10;
        public const int MEDIUM_BOARD_SIZE = 8;
        public const int SMALL_BOARD_SIZE = 6;

        public enum Pieces
        {
            White = 'O',
            WhiteKing = 'U',
            Black = 'X',
            BlackKing = 'K',
            EmptyPiece = ' '
        }

        public enum GamePlayer
        {
            Computer = 1,
            Human = 2
        }

        private Board m_Checkers = new Board();
        private bool m_GameOver = false;
        private Player m_playerOne = new Player();
        private Player m_playerTwo = new Player();

        public void run()
        {
            initGame();
            Player CurrentPlayer = m_playerOne;
            Player OpponentPlayer = m_playerTwo;

            m_Checkers.drawBoard();

            int OpponentPlayerSolidersCounter = 0;
            Move CurrentMove = null;
            Move lastMove = null;
            bool isSequenceEating = false;
            string nextMoveInputString = null;

            while (!m_GameOver)
            {
                if (CurrentPlayer.isHuman)
                {
                    if (nextMoveInputString != null)
                    {
                        UI.DisplayLastPlayerMove(OpponentPlayer, nextMoveInputString);
                    }

                    UI.DisplayCurrentPlayerMessage(CurrentPlayer);
                    nextMoveInputString = Console.ReadLine();

                    if (nextMoveInputString == "Q" && CurrentPlayer.Score < OpponentPlayer.Score)
                    {
                        m_GameOver = true;
                    }

                    lastMove = CurrentMove;
                    CurrentMove = new Move(nextMoveInputString);

                    while (!isStringInputLegal(nextMoveInputString) || !isValidMove(CurrentMove, lastMove, isSequenceEating, CurrentPlayer))
                    {
                        if (!isStringInputLegal(nextMoveInputString))
                        {
                            UI.DisplayIncorrectInputMessage();
                        }
                        else
                        {
                            UI.DisplayCantMoveHereMessage();
                        }

                        nextMoveInputString = Console.ReadLine();
                        CurrentMove = new Move(nextMoveInputString);
                    }
                }
                else
                {
                    UI.DisplayWatingToPcMove();
                    lastMove = CurrentMove;
                    CurrentMove = getAvailableMove(lastMove, isSequenceEating, m_playerTwo);
                }

                OpponentPlayerSolidersCounter = getPlayerSolidersCount(OpponentPlayer);
                makeMove(CurrentMove);

                isSequenceEating = true;

                if (OpponentPlayerSolidersCounter == getPlayerSolidersCount(OpponentPlayer))
                {
                    // not an Eat move.
                    isSequenceEating = false;
                    swapPlayers(ref CurrentPlayer, ref OpponentPlayer);
                }
                else
                {
                    if (isAbleToEat(CurrentMove.NextColumn, CurrentMove.NextRow, isSequenceEating) == null)
                    {
                        // cant eat more.
                        swapPlayers(ref CurrentPlayer, ref OpponentPlayer);
                        isSequenceEating = false;
                    }
                }

                m_GameOver = isGameOver(lastMove, isSequenceEating);

                m_Checkers.clearBoard();
                m_Checkers.drawBoard();
            }

            m_playerOne.Score = getPlayerSolidersCount(m_playerOne) - getPlayerSolidersCount(m_playerTwo);
            m_playerTwo.Score = getPlayerSolidersCount(m_playerTwo) - getPlayerSolidersCount(m_playerOne);

            if (m_playerOne.Score > m_playerTwo.Score)
            {
                UI.DisplayWinnerMessage(m_playerOne);
            }
            else if (m_playerTwo.Score > m_playerOne.Score)
            {
                UI.DisplayWinnerMessage(m_playerTwo);
            }
            else
            {
                UI.DisplayTieMessage();
            }

            UI.GameOverMessage();
        }

        private bool isGameOver(Move i_LastMove, bool i_SequenceEat)
        {
            bool isGameOver = false;

            if (getPlayerSolidersCount(m_playerOne) == 0 || getPlayerSolidersCount(m_playerTwo) == 0)
            {
                isGameOver = true;
            }

            if (getAvailableMove(i_LastMove, i_SequenceEat, m_playerOne) == null || getAvailableMove(i_LastMove, i_SequenceEat, m_playerTwo) == null)
            {
                isGameOver = true;
            }

            return isGameOver;
        }

        private Move getAvailableMove(Move i_LastMove, bool i_IsSequenceEat, Player i_CurrentPlayer)
        {
            Move resMove = null;
            Move tempMove;
            int boardSize = m_Checkers.GameBoard.GetLength(0);

            resMove = checkIfPlayerCanEat(i_CurrentPlayer, i_IsSequenceEat);

            // to ensure that if we can eat, we eat.
            if (resMove == null)
            {
                for (int column = 0; column < boardSize; column++)
                {
                    for (int row = 0; row < boardSize; row++)
                    {
                        if (i_CurrentPlayer.IsWhite)
                        {
                            if (m_Checkers.GameBoard[column, row].isWhite())
                            {
                                for (int nextRowMove = -1; nextRowMove < 2; nextRowMove += 2)
                                {
                                    for (int nextColMove = 1; nextColMove > -2; nextColMove -= 2)
                                    {
                                        tempMove = new Move(column, row, column + nextColMove, row + nextRowMove);
                                        if (isValidMove(tempMove, i_LastMove, i_IsSequenceEat, i_CurrentPlayer))
                                        {
                                            resMove = new Move(column, row, column + nextColMove, row + nextRowMove);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (m_Checkers.GameBoard[column, row].isBlack())
                            {
                                for (int nextRowMove = -1; nextRowMove < 2; nextRowMove += 2)
                                {
                                    for (int nextColMove = 1; nextColMove > -2; nextColMove -= 2)
                                    {
                                        tempMove = new Move(column, row, column + nextColMove, row + nextRowMove);
                                        if (isValidMove(tempMove, i_LastMove, i_IsSequenceEat, i_CurrentPlayer))
                                        {
                                            resMove = new Move(column, row, column + nextColMove, row + nextRowMove);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return resMove;
        }

        private void makeMove(Move i_CurrentMove)
        {
            int currentRow = i_CurrentMove.CurrentRow;
            int currentCoulmn = i_CurrentMove.CurrentCoulmn;
            int nextRow = i_CurrentMove.NextRow;
            int nextCoulmn = i_CurrentMove.NextColumn;

            if (m_Checkers.GameBoard[currentCoulmn, currentRow].Value == Pieces.White)
            {
                if (nextRow == m_Checkers.GameBoard.GetLength(0) - 1)
                {
                    m_Checkers.GameBoard[currentCoulmn, currentRow].Value = Pieces.WhiteKing;
                }
            }

            if (m_Checkers.GameBoard[currentCoulmn, currentRow].Value == Pieces.Black)
            {
                if (nextRow == 0)
                {
                    m_Checkers.GameBoard[currentCoulmn, currentRow].Value = Pieces.BlackKing;
                }
            }

            m_Checkers.GameBoard[nextCoulmn, nextRow].Value = m_Checkers.GameBoard[currentCoulmn, currentRow].Value;
            m_Checkers.GameBoard[currentCoulmn, currentRow].Value = Pieces.EmptyPiece;

            if (Math.Abs(i_CurrentMove.CurrentCoulmn - i_CurrentMove.NextColumn) == 2)
            { 
                // this mean we eat, and we need now to delete the Player.
                m_Checkers.GameBoard[(nextCoulmn + currentCoulmn) / 2, (nextRow + currentRow) / 2].Value = Pieces.EmptyPiece;
            }
        }

        private Move isAbleToEat(int i_CurrentColumn, int i_CurrentRow, bool i_IsIntEatingSequence)
        {
            Cell currentCell = m_Checkers.GameBoard[i_CurrentColumn, i_CurrentRow];
            int boardSize = m_Checkers.GameBoard.GetLength(0);

            // assume we cant eat.
            Move eatMove = null; 

            if (currentCell.isWhite() || currentCell.isKing() || i_IsIntEatingSequence)
            {
                if (i_CurrentRow + 2 < boardSize && i_CurrentColumn + 2 < boardSize)
                {
                    if (checkIfOpponent(currentCell, m_Checkers.GameBoard[i_CurrentColumn + 1, i_CurrentRow + 1]) &&
                        m_Checkers.GameBoard[i_CurrentColumn + 2, i_CurrentRow + 2].isEmpty())
                    {
                        eatMove = new Move(i_CurrentColumn, i_CurrentRow, i_CurrentColumn + 2, i_CurrentRow + 2);
                    }
                }

                if (i_CurrentRow + 2 < boardSize && i_CurrentColumn >= 2)
                {
                    if (checkIfOpponent(currentCell, m_Checkers.GameBoard[i_CurrentColumn - 1, i_CurrentRow + 1]) &&
                        m_Checkers.GameBoard[i_CurrentColumn - 2, i_CurrentRow + 2].isEmpty())
                    {
                        eatMove = new Move(i_CurrentColumn, i_CurrentRow, i_CurrentColumn - 2, i_CurrentRow + 2);
                    }
                }
            }

            if (currentCell.isBlack() || currentCell.isKing() || i_IsIntEatingSequence)
            {
                if (i_CurrentRow >= 2 && i_CurrentColumn >= 2)
                {
                    if (checkIfOpponent(currentCell, m_Checkers.GameBoard[i_CurrentColumn - 1, i_CurrentRow - 1]) &&
                        m_Checkers.GameBoard[i_CurrentColumn - 2, i_CurrentRow - 2].isEmpty())
                    {
                        eatMove = new Move(i_CurrentColumn, i_CurrentRow, i_CurrentColumn - 2, i_CurrentRow - 2);
                    }
                }

                if (i_CurrentRow >= 2 && i_CurrentColumn + 2 < boardSize)
                {
                    if (checkIfOpponent(currentCell, m_Checkers.GameBoard[i_CurrentColumn + 1, i_CurrentRow - 1]) &&
                        m_Checkers.GameBoard[i_CurrentColumn + 2, i_CurrentRow - 2].isEmpty())
                    {
                        eatMove = new Move(i_CurrentColumn, i_CurrentRow, i_CurrentColumn + 2, i_CurrentRow - 2);
                    }
                }
            }

            return eatMove;
        }

        private bool checkIfOpponent(Cell i_cellOne, Cell i_cellTwo)
        {
            bool res = false;

            if (i_cellOne.isBlack() && i_cellTwo.isWhite())
            {
                res = true;
            }

            if (i_cellTwo.isBlack() && i_cellOne.isWhite())
            {
                res = true;
            }

            return res;
        }

        private bool isMoveInputLegal(Move i_move)
        {
            int boardSize = m_Checkers.GameBoard.GetLength(0);
            bool legalMove = true;

            if (i_move.CurrentCoulmn < 0 || i_move.CurrentRow < 0 || i_move.NextColumn < 0 || i_move.NextRow < 0)
            {
                legalMove = false;
            }

            if (i_move.CurrentCoulmn >= boardSize || i_move.CurrentRow >= boardSize || i_move.NextColumn >= boardSize || i_move.NextRow >= boardSize)
            {
                legalMove = false;
            }

            return legalMove;
        }

        private bool isValidMove(Move i_CurrentMove, Move i_LastMove, bool i_SequenceEat, Player i_CurrentPlayer)
        {
            bool isThisValidMove = false;

            if (isMoveInputLegal(i_CurrentMove))
            {
                Cell currentCell = m_Checkers.GameBoard[i_CurrentMove.CurrentCoulmn, i_CurrentMove.CurrentRow];
                if (i_SequenceEat)
                {
                    if (i_LastMove.NextColumn == i_CurrentMove.CurrentCoulmn && i_LastMove.NextRow == i_CurrentMove.CurrentRow)
                    {
                        if (isPlayerDoEatMove(i_CurrentMove))
                        {
                            isThisValidMove = true;
                        }
                    }
                }
                else
                {
                    if (checkIfPlayerCanEat(i_CurrentPlayer, i_SequenceEat) != null)
                    {
                        if (isPlayerDoEatMove(i_CurrentMove))
                        {
                            isThisValidMove = true;
                        }
                    }
                    else
                    {
                        // if we cant eat, need to be valid move.
                        if (currentCell.isKing())
                        {
                            if (Math.Abs(i_CurrentMove.NextRow - i_CurrentMove.CurrentRow) == 1
                             && Math.Abs(i_CurrentMove.CurrentCoulmn - i_CurrentMove.NextColumn) == 1)
                            {
                                isThisValidMove = true;
                            }
                        }
                        else if (currentCell.isWhite())
                        {
                            if (i_CurrentMove.NextRow - i_CurrentMove.CurrentRow == 1
                             && Math.Abs(i_CurrentMove.CurrentCoulmn - i_CurrentMove.NextColumn) == 1)
                            {
                                isThisValidMove = true;
                            }
                        }
                        else if (currentCell.isBlack())
                        {
                            if (i_CurrentMove.CurrentRow - i_CurrentMove.NextRow == 1
                             && Math.Abs(i_CurrentMove.CurrentCoulmn - i_CurrentMove.NextColumn) == 1)
                            {
                                isThisValidMove = true;
                            }
                        }
                    }
                }

                // check that we not overrider another piece.
                if (m_Checkers.GameBoard[i_CurrentMove.NextColumn, i_CurrentMove.NextRow].Value != Pieces.EmptyPiece)
                {
                    isThisValidMove = false;
                }

                // now we check if the player is moving hes soilders !
                if (i_CurrentPlayer.IsWhite)
                {
                    if (m_Checkers.GameBoard[i_CurrentMove.CurrentCoulmn, i_CurrentMove.CurrentRow].Value == Pieces.Black
                        || m_Checkers.GameBoard[i_CurrentMove.CurrentCoulmn, i_CurrentMove.CurrentRow].Value == Pieces.BlackKing)
                    {
                        isThisValidMove = false;
                    }
                }
                else
                {
                    if (m_Checkers.GameBoard[i_CurrentMove.CurrentCoulmn, i_CurrentMove.CurrentRow].Value == Pieces.White
                      || m_Checkers.GameBoard[i_CurrentMove.CurrentCoulmn, i_CurrentMove.CurrentRow].Value == Pieces.WhiteKing)
                    {
                        isThisValidMove = false;
                    }
                }
            }

            return isThisValidMove;
        }

        private bool isPlayerDoEatMove(Move i_CurrentMove)
        {
            bool eatRes = false;

            int currentRow = i_CurrentMove.CurrentRow;
            int currentCoulmn = i_CurrentMove.CurrentCoulmn;
            int nextRow = i_CurrentMove.NextRow;
            int nextCoulmn = i_CurrentMove.NextColumn;

            if (Math.Abs(i_CurrentMove.CurrentCoulmn - i_CurrentMove.NextColumn) == 2)
            {
                Cell currentCell = m_Checkers.GameBoard[currentCoulmn, currentRow];
                Cell targetCell = m_Checkers.GameBoard[(nextCoulmn + currentCoulmn) / 2, (nextRow + currentRow) / 2];

                if (checkIfOpponent(currentCell, targetCell))
                {
                    eatRes = true;
                }
            }

            return eatRes;
        }

        private Move checkIfPlayerCanEat(Player i_CurrentPlayer, bool i_IsSequenceEating)
        {
            int boardSize = m_Checkers.GameBoard.GetLength(0);
            Move canEatRes = null;

            for (int column = 0; column < boardSize; column++)
            {
                for (int row = 0; row < boardSize; row++)
                {
                    if (i_CurrentPlayer.IsWhite)
                    {
                        if (m_Checkers.GameBoard[column, row].isWhite())
                        {
                            if (isAbleToEat(column, row, i_IsSequenceEating) != null)
                            {
                                canEatRes = isAbleToEat(column, row, i_IsSequenceEating);
                            }
                        }
                    }
                    else
                    {
                        if (m_Checkers.GameBoard[column, row].isBlack())
                        {
                            if (isAbleToEat(column, row, i_IsSequenceEating) != null)
                            {
                                canEatRes = isAbleToEat(column, row, i_IsSequenceEating);
                            }
                        }
                    }
                }
            }

            return canEatRes;
        }

        private void swapPlayers(ref Player i_CurrentPlayer, ref Player i_OpponentPlayer)
        {
            if (i_CurrentPlayer == m_playerOne)
            {
                i_CurrentPlayer = m_playerTwo;
                i_OpponentPlayer = m_playerOne;
            }
            else
            {
                i_CurrentPlayer = m_playerOne;
                i_OpponentPlayer = m_playerTwo;
            }
        }

        private void initGame()
        {
            m_playerOne.Name = UI.GetPlayerNameFromInput();
            m_playerOne.IsWhite = false;

            uint boardSize = UI.getSizeFromUserInput();
            GamePlayer userOpponent = UI.GetUserRival();

            if (userOpponent == GamePlayer.Human)
            {
                m_playerTwo.Name = UI.GetPlayerNameFromInput();
            }
            else
            {
                m_playerTwo.Name = GamePlayer.Computer.ToString();
                m_playerTwo.isHuman = false;
            }

            m_Checkers.clearBoard();
            m_Checkers.createEmptyGameBoard(boardSize);

            for (int column = 0; column < boardSize; column = column + 2)
            {
                for (int row = 0; row < (boardSize / 2) - 1; row++)
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

        private bool isStringInputLegal(string i_MoveInput)
        {
            int column = m_Checkers.GameBoard.GetLength(0);

            return (i_MoveInput.Length == 5
                    && i_MoveInput[0] >= 'A' && i_MoveInput[0] <= (column + Board.COLUMN_CAPITAL_LETTER)
                    && i_MoveInput[1] >= 'a' && i_MoveInput[1] <= (column + Board.ROW_SMALL_LETTER)
                    && i_MoveInput[2] == '>'
                    && i_MoveInput[3] >= 'A' && i_MoveInput[3] <= (column + Board.COLUMN_CAPITAL_LETTER)
                    && i_MoveInput[4] >= 'a' && i_MoveInput[4] <= (column + Board.ROW_SMALL_LETTER));
        }

        private int getPlayerSolidersCount(Player i_CurrentPlayer)
        {
            int solidersCount = 0;
            int boardSize = m_Checkers.GameBoard.GetLength(0);

            for (int currentCol = 0; currentCol < boardSize; currentCol++)
            {
                for (int currentRow = 0; currentRow < boardSize; currentRow++)
                {
                    if (i_CurrentPlayer.IsWhite)
                    {
                        if (m_Checkers.GameBoard[currentCol, currentRow].Value == Pieces.White)
                        {
                            solidersCount++;
                        }

                        if (m_Checkers.GameBoard[currentCol, currentRow].Value == Pieces.WhiteKing)
                        {
                            solidersCount += 4;
                        }
                    }
                    else
                    {
                        if (m_Checkers.GameBoard[currentCol, currentRow].Value == Pieces.Black)
                        {
                            solidersCount++;
                        }

                        if (m_Checkers.GameBoard[currentCol, currentRow].Value == Pieces.BlackKing)
                        {
                            solidersCount += 4;
                        }
                    }
                }
            }

            return solidersCount;
        }
    }
}
