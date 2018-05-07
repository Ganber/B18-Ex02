using System;

namespace B18_Ex02
{
    public class Game
    {
        public const int BIG_BOARD_SIZE = 10;
        public const int MEDIUM_BOARD_SIZE = 8;
        public const int SMALL_BOARD_SIZE = 6;

        public enum GamePlayer
        {
            Computer = 1,
            Human = 2
        }

        private Board m_Checkers = new Board();
        private Player m_playerOne = new Player();
        private Player m_playerTwo = new Player();
        private bool m_GameOver = false;

        public Board Board
        {
            get { return m_Checkers; }
        }

        public Player PlayerOne
        {
            get { return m_playerOne; }
        }

        public Player PlayerTwo
        {
            get { return m_playerTwo; }
        }

        public bool GameOver
        {
            get { return m_GameOver; }
            set { m_GameOver = value; }
        }

        public bool IsGameOver(Player i_OpponentPlayer)
        {
            bool isGameOver = false;

            if (m_Checkers.getPlayerSolidersCount(PlayerOne) == 0 || m_Checkers.getPlayerSolidersCount(PlayerTwo) == 0)
            {
                isGameOver = true;
            }

            if (GetAvailableMove(null, false, i_OpponentPlayer) == null)
            {
                isGameOver = true;
            }

            return isGameOver;
        }

        public Move GetAvailableMove(Move i_LastMove, bool i_IsSequenceEat, Player i_CurrentPlayer)
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
                                        if (IsValidMove(tempMove, i_LastMove, i_IsSequenceEat, i_CurrentPlayer))
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
                                        if (IsValidMove(tempMove, i_LastMove, i_IsSequenceEat, i_CurrentPlayer))
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

        public void MakeMove(Move i_CurrentMove)
        {
            int currentRow = i_CurrentMove.CurrentRow;
            int currentCoulmn = i_CurrentMove.CurrentCoulmn;
            int nextRow = i_CurrentMove.NextRow;
            int nextCoulmn = i_CurrentMove.NextColumn;

            if (m_Checkers.GameBoard[currentCoulmn, currentRow].Value == UI.Pieces.White)
            {
                if (nextRow == m_Checkers.GameBoard.GetLength(0) - 1)
                {
                    m_Checkers.GameBoard[currentCoulmn, currentRow].Value = UI.Pieces.WhiteKing;
                }
            }

            if (m_Checkers.GameBoard[currentCoulmn, currentRow].Value == UI.Pieces.Black)
            {
                if (nextRow == 0)
                {
                    m_Checkers.GameBoard[currentCoulmn, currentRow].Value = UI.Pieces.BlackKing;
                }
            }

            m_Checkers.GameBoard[nextCoulmn, nextRow].Value = m_Checkers.GameBoard[currentCoulmn, currentRow].Value;
            m_Checkers.GameBoard[currentCoulmn, currentRow].Value = UI.Pieces.EmptyPiece;

            if (System.Math.Abs(i_CurrentMove.CurrentCoulmn - i_CurrentMove.NextColumn) == 2)
            { 
                // this mean we eat, and we need now to delete the Player.
                m_Checkers.GameBoard[(nextCoulmn + currentCoulmn) / 2, (nextRow + currentRow) / 2].Value = UI.Pieces.EmptyPiece;
            }
        }

        public void ResetValues()
        {
            GameOver = false;
        }

        public Move IsAbleToEat(int i_CurrentColumn, int i_CurrentRow, bool i_IsIntEatingSequence)
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

        public void UpdateScore()
        {
            if (Board.getPlayerSolidersCount(PlayerOne) - Board.getPlayerSolidersCount(PlayerTwo) > 0)
            {
                PlayerOne.Score += Board.getPlayerSolidersCount(PlayerOne) - Board.getPlayerSolidersCount(PlayerTwo);
            }
            else
            {
                PlayerTwo.Score += Board.getPlayerSolidersCount(PlayerTwo) - Board.getPlayerSolidersCount(PlayerOne);
            }
        }

        public void UpdateLastMove(Move currentMove, ref Move lastMove)
        {
            lastMove = currentMove;
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

        public Player getOpponent(Player i_CurrentPlayer)
        {
            if (i_CurrentPlayer == PlayerOne)
                return PlayerTwo;
            else
                return PlayerOne;
        }

        public void MakeNextMove(ref Player i_CurrentPlayer, ref Move i_LastMove,ref bool i_isSequenceEating)
        {
            Move CurrentMove=null;
            int OpponentPlayerSolidersCounter = 0;
            string nextMoveStringInput;
            
            if (i_CurrentPlayer.isHuman)
            {
                nextMoveStringInput = UI.GetUserInput();
                if (nextMoveStringInput.Equals(UI.RESIGN_GAME) 
                    && Board.getPlayerSolidersCount(i_CurrentPlayer) < Board.getPlayerSolidersCount(getOpponent(i_CurrentPlayer)))
                {
                    GameOver = true;
                }

                CurrentMove = new Move(nextMoveStringInput);

                while (!IsStringInputLegal(nextMoveStringInput) || !IsValidMove(CurrentMove, i_LastMove, i_isSequenceEating, i_CurrentPlayer))
                {
                    if (!IsStringInputLegal(nextMoveStringInput))
                    {
                        UI.DisplayIncorrectInputMessage();
                    }
                    else
                    {
                        UI.DisplayCantMoveHereMessage();
                    }

                    nextMoveStringInput = UI.GetUserInput();
                    CurrentMove = new Move(nextMoveStringInput);
                }
            }

            else
            {
               UI.DisplayWatingToPcMove();
               CurrentMove = GetAvailableMove(i_LastMove, i_isSequenceEating, PlayerTwo);
            }

            OpponentPlayerSolidersCounter = Board.getPlayerSolidersCount(getOpponent(i_CurrentPlayer));
            MakeMove(CurrentMove);
            UpdateLastMove(CurrentMove, ref i_LastMove);

            i_isSequenceEating = true;

            if (OpponentPlayerSolidersCounter == Board.getPlayerSolidersCount(getOpponent(i_CurrentPlayer)))
            {
                // not an Eat move.
                i_isSequenceEating = false;
                if (i_CurrentPlayer == PlayerOne)
                {
                    i_CurrentPlayer = PlayerTwo;
                }
                else
                {
                    i_CurrentPlayer = PlayerOne;
                }
            }
            else
            {
                if (IsAbleToEat(CurrentMove.NextColumn, CurrentMove.NextRow, i_isSequenceEating) == null)
                {
                    // cant eat more.
                    if (i_CurrentPlayer == PlayerOne)
                    {
                        i_CurrentPlayer = PlayerTwo;
                    }
                    else
                    {
                        i_CurrentPlayer = PlayerOne;
                    }

                    i_isSequenceEating = false;
                }
            }

            GameOver = IsGameOver(getOpponent(i_CurrentPlayer));

        }

        public bool IsValidMove(Move i_CurrentMove, Move i_LastMove, bool i_SequenceEat, Player i_CurrentPlayer)
        {
            bool isThisValidMove = false;

            // check that the move is legal.
            if (i_CurrentMove.isMoveInputLegal(m_Checkers.GameBoard.GetLength(0))) 
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
                            if (System.Math.Abs(i_CurrentMove.NextRow - i_CurrentMove.CurrentRow) == 1
                             && System.Math.Abs(i_CurrentMove.CurrentCoulmn - i_CurrentMove.NextColumn) == 1)
                            {
                                isThisValidMove = true;
                            }
                        }
                        else if (currentCell.isWhite())
                        {
                            if (i_CurrentMove.NextRow - i_CurrentMove.CurrentRow == 1
                             && System.Math.Abs(i_CurrentMove.CurrentCoulmn - i_CurrentMove.NextColumn) == 1)
                            {
                                isThisValidMove = true;
                            }
                        }
                        else if (currentCell.isBlack())
                        {
                            if (i_CurrentMove.CurrentRow - i_CurrentMove.NextRow == 1
                             && System.Math.Abs(i_CurrentMove.CurrentCoulmn - i_CurrentMove.NextColumn) == 1)
                            {
                                isThisValidMove = true;
                            }
                        }
                    }
                }

                // check that we not overrider another piece.
                if (m_Checkers.GameBoard[i_CurrentMove.NextColumn, i_CurrentMove.NextRow].Value != UI.Pieces.EmptyPiece)
                {
                    isThisValidMove = false;
                }

                // now we check if the player is moving hes soilders !
                if (i_CurrentPlayer.IsWhite)
                {
                    if (m_Checkers.GameBoard[i_CurrentMove.CurrentCoulmn, i_CurrentMove.CurrentRow].Value == UI.Pieces.Black
                        || m_Checkers.GameBoard[i_CurrentMove.CurrentCoulmn, i_CurrentMove.CurrentRow].Value == UI.Pieces.BlackKing)
                    {
                        isThisValidMove = false;
                    }
                }
                else
                {
                    if (m_Checkers.GameBoard[i_CurrentMove.CurrentCoulmn, i_CurrentMove.CurrentRow].Value == UI.Pieces.White
                      || m_Checkers.GameBoard[i_CurrentMove.CurrentCoulmn, i_CurrentMove.CurrentRow].Value == UI.Pieces.WhiteKing)
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

            if (System.Math.Abs(i_CurrentMove.CurrentCoulmn - i_CurrentMove.NextColumn) == 2)
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
                            if (IsAbleToEat(column, row, i_IsSequenceEating) != null)
                            {
                                canEatRes = IsAbleToEat(column, row, i_IsSequenceEating);
                            }
                        }
                    }
                    else
                    {
                        if (m_Checkers.GameBoard[column, row].isBlack())
                        {
                            if (IsAbleToEat(column, row, i_IsSequenceEating) != null)
                            {
                                canEatRes = IsAbleToEat(column, row, i_IsSequenceEating);
                            }
                        }
                    }
                }
            }

            return canEatRes;
        }

        public void SwapPlayers(ref Player i_CurrentPlayer, ref Player i_OpponentPlayer)
        {
            if (i_CurrentPlayer == PlayerOne)
            {
                i_CurrentPlayer = PlayerTwo;
                i_OpponentPlayer = PlayerOne;
            }
            else
            {
                i_CurrentPlayer = PlayerOne;
                i_OpponentPlayer = PlayerTwo;
            }
        }

        public bool IsStringInputLegal(string i_MoveInput)
        {
            int column = m_Checkers.GameBoard.GetLength(0);

            return i_MoveInput.Length == 5
                   && i_MoveInput[0] >= 'A' && i_MoveInput[0] <= column + Board.COLUMN_CAPITAL_LETTER
                   && i_MoveInput[1] >= 'a' && i_MoveInput[1] <= column + Board.ROW_SMALL_LETTER
                   && i_MoveInput[2] == '>'
                   && i_MoveInput[3] >= 'A' && i_MoveInput[3] <= column + Board.COLUMN_CAPITAL_LETTER
                   && i_MoveInput[4] >= 'a' && i_MoveInput[4] <= column + Board.ROW_SMALL_LETTER;
        }
    }
}
