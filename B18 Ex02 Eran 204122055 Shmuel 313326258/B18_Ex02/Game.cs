using System;

namespace B18_Ex02
{
    public class Game
    {
        public const int BIG_BOARD_SIZE = 10;
        public const int MEDIUM_BOARD_SIZE = 8;
        public const int SMALL_BOARD_SIZE = 6;

        public enum pieces { White = 'O', WhiteKing = 'U', Black = 'X', BlackKing = 'K' };
        public enum eatDirections { UpRight, UpLeft, DownRight, DownLeft, CantEat };

        Board m_Checkers = new Board();
        bool m_GameOver = false;
        private HumanPlayer m_playerOne = new HumanPlayer();
        private HumanPlayer m_playerTwo = new HumanPlayer();

        public void run()
        {
            HumanPlayer currentPlayerTurn = m_playerOne;

            initGame();
            m_Checkers.drawBoard();

            while (!m_GameOver)
            {

                UI.DisplayCurrentPlayerMessage(m_playerOne); //TODO: needs to be current player, add the symbol at the end of message (X or O)
                string nextMove = Console.ReadLine();
                Move(nextMove, currentPlayerTurn);

                if (currentPlayerTurn.Equals(m_playerOne))
                {
                    currentPlayerTurn = m_playerTwo;
                }
                else
                {
                    currentPlayerTurn = m_playerOne;
                }
                m_Checkers.clearBoard();
                m_Checkers.drawBoard();
            }
            UI.GameOverMessage(); //TODO: exit game
        }

        private void initGame()
        {
            m_playerOne.Name = UI.GetPlayerNameFromInput();
            m_playerTwo.IsWhite = false;

            uint boardSize = UI.getSizeFromUserInput();
            m_Checkers.createEmptyGameBoard(boardSize);

            //TODO: play against comp or human player

            for (int column = 0; column < boardSize; column = column + 2)
            {
                for (int row = 0; row < boardSize / 2 - 1; row++)
                {
                    if (row % 2 == 0)
                    {
                        m_Checkers.GameBoard[column, row].Value = (char)pieces.White;
                        m_Checkers.GameBoard[column, row].HasPiece = true;
                        m_Checkers.GameBoard[column + 1, boardSize - row - 1].Value = (char)pieces.Black;
                        m_Checkers.GameBoard[column + 1, boardSize - row - 1].HasPiece = true;
                    }
                    else
                    {
                        m_Checkers.GameBoard[column + 1, row].Value = (char)pieces.White;
                        m_Checkers.GameBoard[column + 1, row].HasPiece = true;
                        m_Checkers.GameBoard[column, boardSize - row - 1].Value = (char)pieces.Black;
                        m_Checkers.GameBoard[column, boardSize - row - 1].HasPiece = true;
                    }
                }
            }
        }

        private void Move(string i_MoveInput, HumanPlayer i_currentPlayer)
        {
            bool legalMove;
            legalMove = isMoveInputLegal(i_MoveInput, i_currentPlayer);

            while ((!legalMove))
            {
                UI.DisplayIncorrectInputMessage();
                i_MoveInput = Console.ReadLine();
                legalMove = isMoveInputLegal(i_MoveInput, i_currentPlayer);

            }

            int currentCol, currentRow, nextCol, nextRow;
            convertStringInputToIntegers(i_MoveInput, out currentCol, out currentRow, out nextCol, out nextRow);

            m_Checkers.GameBoard[nextCol, nextRow].Value = m_Checkers.GameBoard[currentCol, currentRow].Value;
            m_Checkers.GameBoard[nextCol, nextRow].HasPiece = true;
            m_Checkers.GameBoard[currentCol, currentRow].Value = ' ';
            m_Checkers.GameBoard[currentCol, currentRow].HasPiece = false;
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


        private bool isMoveInputLegal(string i_MoveInput, HumanPlayer i_CurrentPlayer)
        {
            int boardSize = m_Checkers.GameBoard.GetLength(0);

            if (isStringInputLegal(i_MoveInput))
            {
                int currentCol, currentRow, nextCol, nextRow;

                convertStringInputToIntegers(i_MoveInput, out currentCol, out currentRow, out nextCol, out nextRow);

                if (!isThereEatMove(i_CurrentPlayer)) //if we cant eat, just play leagal move.
                {
                    if (m_Checkers.GameBoard[currentCol, currentRow].HasPiece && !m_Checkers.GameBoard[nextCol, nextRow].HasPiece)
                    {
                        if (i_CurrentPlayer.IsWhite)
                        {
                            return (nextRow - currentRow == 1 && Math.Abs(currentCol - nextCol) == 1);
                        }
                        else
                        {
                            return (currentRow - nextRow == 1 && Math.Abs(currentCol - nextCol) == 1);
                        }
                    }
                }

                else //that's mean we MUST eat, need to check if we eating !
                {
                    if (i_CurrentPlayer.IsWhite)
                    {
                        if (boardSize > currentCol + 2 && boardSize > currentRow + 2)
                        {
                            if ((m_Checkers.GameBoard[currentCol + 1, currentRow + 1].Value == (char)Game.pieces.Black
                                           && m_Checkers.GameBoard[currentCol + 2, currentRow + 2].Value == ' '))
                            {
                                if (nextCol == currentCol + 2 && nextRow == currentRow + 2)
                                {
                                    m_Checkers.GameBoard[currentCol + 1, currentRow + 1].Value = ' ';
                                    return true;
                                }
                            }
                            else if (currentCol >= 2 && boardSize > currentRow + 2)
                            {
                                if ((m_Checkers.GameBoard[currentCol - 1, currentRow + 1].Value == (char)Game.pieces.Black
                                               && m_Checkers.GameBoard[currentCol - 2, currentRow + 2].Value == ' '))
                                {
                                    if (nextCol == currentCol - 2 && nextRow == currentRow + 2)
                                    {
                                        m_Checkers.GameBoard[currentCol - 1, currentRow + 1].Value = ' ';
                                        return true;
                                    }
                                }


                            }

                        }
             
                    }

                    else //that's mean Black player.
                    {
                        if (boardSize > currentCol + 2 &&  currentRow >= 2)
                        {
                            if ((m_Checkers.GameBoard[currentCol + 1, currentRow - 1].Value == (char)Game.pieces.White
                                           && m_Checkers.GameBoard[currentCol + 2, currentRow - 2].Value == ' '))
                            {
                                if (nextCol == currentCol + 2 && nextRow == currentRow - 2)
                                {
                                    m_Checkers.GameBoard[currentCol + 1, currentRow - 1].Value = ' ';
                                    return true;
                                }
                            }
                            else if (currentCol >= 2 &&  currentRow >= 2)
                            {
                                if ((m_Checkers.GameBoard[currentCol - 1, currentRow - 1].Value == (char)Game.pieces.White
                                               && m_Checkers.GameBoard[currentCol - 2, currentRow - 2].Value == ' '))
                                {
                                    if (nextCol == currentCol - 2 && nextRow == currentRow - 2)
                                    {
                                        m_Checkers.GameBoard[currentCol - 1, currentRow - 1].Value = ' ';
                                        return true;
                                    }
                                }


                            }

                        }
                    }
                }

            }
            return false;
        }

        private bool isThereEatMove(HumanPlayer i_CurrentPlayer)
        {
            bool isThereEatMove = false;
            int boardSize = m_Checkers.GameBoard.GetLength(0);

            for (int currentCol = 0; currentCol < boardSize; currentCol++)
            {
                for (int currentRow = 0; currentRow < boardSize; currentRow++)
                {

                    if (i_CurrentPlayer.IsWhite) //this is White Turn !
                    {                            //TODO: need condition for King ! 
                        if (m_Checkers.GameBoard[currentCol, currentRow].Value == (char)pieces.White)
                        {
                            if (boardSize > currentCol + 2)
                            {
                                if ((m_Checkers.GameBoard[currentCol + 1, currentRow + 1].Value == (char)Game.pieces.Black
                                               && m_Checkers.GameBoard[currentCol + 2, currentRow + 2].Value == ' '))
                                {
                                    isThereEatMove = true;
                                }
                            }
                            else if (currentCol >= 2 && boardSize > currentRow + 2)
                            {
                                if (m_Checkers.GameBoard.GetLength(0) > currentCol + 2 && currentCol - 2 > 0)
                                    if ((m_Checkers.GameBoard[currentCol - 1, currentRow + 1].Value == (char)Game.pieces.Black
                                      && m_Checkers.GameBoard[currentCol - 2, currentRow + 2].Value == ' '))
                                    {
                                        isThereEatMove = true;
                                    }
                            }
                        }
                    }
                    else //this is black turn !
                    {
                        //TODO: need condition for King ! 
                        if (m_Checkers.GameBoard[currentCol, currentRow].Value == (char)pieces.Black)
                        {
                            if (boardSize > currentCol + 2 && currentRow >= 2)
                            {
                                if (m_Checkers.GameBoard[currentCol + 1, currentRow - 1].Value == (char)Game.pieces.White
                                && m_Checkers.GameBoard[currentCol + 2, currentRow - 2].Value == ' ')
                                {
                                    isThereEatMove = true;
                                }
                            }

                            if (currentRow >= 2 && currentCol >= 2)
                                if (m_Checkers.GameBoard[currentCol - 1, currentRow - 1].Value == (char)Game.pieces.White
                                   && m_Checkers.GameBoard[currentCol - 2, currentRow - 2].Value == ' ')
                                {
                                    isThereEatMove = true;
                                }
                        }
                    }
                }
            }

            return isThereEatMove;
        }

        private void convertStringInputToIntegers(string i_MoveInput, out int i_currentCol, out int i_currentRow, out int i_nextCol, out int i_nextRow)
        {
            i_currentCol = i_MoveInput[0] - Board.COLUMN_CAPITAL_LETTER;
            i_currentRow = i_MoveInput[1] - Board.ROW_SMALL_LETTER;

            i_nextCol = i_MoveInput[3] - Board.COLUMN_CAPITAL_LETTER;
            i_nextRow = i_MoveInput[4] - Board.ROW_SMALL_LETTER;
        }
    }
}
