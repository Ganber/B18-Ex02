namespace B18_Ex02
{
    using System;
    using System.Text.RegularExpressions;

    public class UI
    {
        private const string cantMoveHereMessage = "Error! Wrong move!";
        private const string incorrectInputMessage = "Error! Incorrect input.";
        private const string cantSelectMessage = "Error! Cannot select!";
        private const string enterPlayerNameMessage = "Hello! please enter your name (20 letters max)";
        private const string playAgainstComputerOrPlayerMessage = "enter 1 to play against computer or 2 to play against human player";
        private const char RESIGN_GAME = 'Q';

        private static uint m_BoardSize;
        private Game m_Checkers = new Game();

        public enum Pieces
        {
            White = 'O',
            WhiteKing = 'U',
            Black = 'X',
            BlackKing = 'K',
            EmptyPiece = ' '
        }

        public static string GetUserInput()
        {
            return Console.ReadLine();
        }

        public static uint getSizeFromUserInput()
        {
            Console.WriteLine("Please enter board size: (6, 8, 10)");
            while (!uint.TryParse(Console.ReadLine(), out m_BoardSize) ||
                (m_BoardSize != Game.SMALL_BOARD_SIZE && m_BoardSize != Game.MEDIUM_BOARD_SIZE && m_BoardSize != Game.BIG_BOARD_SIZE))
            {
                Console.WriteLine(incorrectInputMessage);
            }

            return m_BoardSize;
        }

        public static void DisplayCurrentPlayerMessage(Player currentPlayer)
        {
            Pieces symbol;

            if (currentPlayer.IsWhite)
            {
                symbol = Pieces.White;
            }
            else
            {
                symbol = Pieces.Black;
            }

            Console.WriteLine(currentPlayer.Name + "'s turn: (" + (char)symbol + "):");
        }

        public static void DisplayCantMoveHereMessage()
        {
            Console.WriteLine(cantMoveHereMessage);
        }

        public static void DisplayIncorrectInputMessage()
        {
            Console.WriteLine(incorrectInputMessage);
        }

        public static void DisplayCantSelectMessage()
        {
            Console.WriteLine(cantSelectMessage);
        }

        public static void EnterPlayerNameMessage()
        {
            Console.WriteLine(enterPlayerNameMessage);
        }

        public static string GetPlayerNameFromInput()
        {
            EnterPlayerNameMessage();
            string playerName = Console.ReadLine();

            while (playerName.Length > 20 || !Regex.IsMatch(playerName, @"^[a-zA-Z]+$"))
            {
                Console.WriteLine(incorrectInputMessage);
                playerName = Console.ReadLine();
            }

            return playerName;
        }

        public static Game.GamePlayer GetUserRival()
        {
            Console.WriteLine(playAgainstComputerOrPlayerMessage);
            int userInput;
            int.TryParse(Console.ReadLine(), out userInput);

            while (userInput != (int)Game.GamePlayer.Computer && userInput != (int)Game.GamePlayer.Human)
            {
                DisplayIncorrectInputMessage();
                int.TryParse(Console.ReadLine(), out userInput);
            }

            return (Game.GamePlayer)userInput;
        }

        public static void DisplayLastPlayerMove(Player i_PreviousPlayer, Move i_LastMove)
        {
            Pieces symbol;

            if (i_PreviousPlayer.IsWhite)
            {
                symbol = Pieces.White;
            }
            else
            {
                symbol = Pieces.Black;
            }

            Console.WriteLine(i_PreviousPlayer.Name + "'s move was (" + (char)symbol + "): " + i_LastMove.convertToString());
        }

        public static void DisplayCanEatMoreMessage()
        {
            Console.WriteLine("You can eat more");
        }

        public static void DisplayWatingToPcMove()
        {
            Console.WriteLine("Please Wait to PC move..");
            System.Threading.Thread.Sleep(500);
        }

        public static void GameOverMessage()
        {
            Console.SetCursorPosition(50, 10);
            Console.Write("Game Over");
            Console.ReadLine();
        }

        public static void DrawCell(char i_cellToDraw)
        {
            Console.Write(i_cellToDraw);
        }

        private static void DisplayScoreOfPlayers(Player i_PlayerOne, Player i_PlayerTwo)
        {
            Console.WriteLine(i_PlayerOne.Name + "'s Score: " + i_PlayerOne.Score);
            Console.WriteLine(i_PlayerTwo.Name + "'s Score: " + i_PlayerTwo.Score);
        }

        private static void DisplayWinnerMessage(Player i_CurrentPlayer)
        {
            Console.WriteLine("The winner is: " + i_CurrentPlayer.Name);
        }

        private static void DisplayTieMessage()
        {
            Console.WriteLine("Game end with a tie !");
        }

        public void run()
        {
            InitGame(m_Checkers.Board, m_Checkers.PlayerOne, m_Checkers.PlayerTwo);
            Player CurrentPlayer = m_Checkers.PlayerOne;
            Player OpponentPlayer = m_Checkers.PlayerTwo;

            drawBoard();

            int OpponentPlayerSolidersCounter = 0;
            Move CurrentMove = null;
            Move lastMove = null;
            bool isSequenceEating = false;
            string nextMoveInputString = null;

            while (!m_Checkers.GameOver)
            {
                lastMove = CurrentMove;

                m_Checkers.MakeNextMove(nextMoveInputString, CurrentPlayer, CurrentMove);

                if (CurrentPlayer.isHuman)
                {
                    if (nextMoveInputString != null)
                    {
                        DisplayLastPlayerMove(OpponentPlayer, lastMove);
                    }

                    DisplayCurrentPlayerMessage(CurrentPlayer);
                    nextMoveInputString = GetUserInput();

                    if (nextMoveInputString.Equals(RESIGN_GAME) && m_Checkers.Board.getPlayerSolidersCount(CurrentPlayer) < m_Checkers.Board.getPlayerSolidersCount(OpponentPlayer))
                    {
                        m_Checkers.GameOver = true;
                        break;
                    }

                    CurrentMove = new Move(nextMoveInputString);

                    while (!m_Checkers.IsStringInputLegal(nextMoveInputString) || !m_Checkers.IsValidMove(CurrentMove, lastMove, isSequenceEating, CurrentPlayer))
                    {
                        if (!m_Checkers.IsStringInputLegal(nextMoveInputString))
                        {
                            DisplayIncorrectInputMessage();
                        }
                        else
                        {
                            DisplayCantMoveHereMessage();
                        }

                        nextMoveInputString = GetUserInput();
                        CurrentMove = new Move(nextMoveInputString);
                    }
                }
                else
                {
                    DisplayWatingToPcMove();
                    CurrentMove = m_Checkers.GetAvailableMove(lastMove, isSequenceEating, CurrentPlayer); // TODO: change CurrentPlayer -> m_playerTwo.
                }

                OpponentPlayerSolidersCounter = m_Checkers.Board.getPlayerSolidersCount(OpponentPlayer);
                m_Checkers.MakeMove(CurrentMove);

                isSequenceEating = true;

                if (OpponentPlayerSolidersCounter == m_Checkers.Board.getPlayerSolidersCount(OpponentPlayer))
                {
                    // not an Eat move.
                    isSequenceEating = false;
                    m_Checkers.SwapPlayers(ref CurrentPlayer, ref OpponentPlayer);
                }
                else
                {
                    if (m_Checkers.IsAbleToEat(CurrentMove.NextColumn, CurrentMove.NextRow, isSequenceEating) == null)
                    {
                        // cant eat more.
                        m_Checkers.SwapPlayers(ref CurrentPlayer, ref OpponentPlayer);
                        isSequenceEating = false;
                    }
                }

                m_Checkers.GameOver = m_Checkers.IsGameOver(OpponentPlayer);

                clearBoard();
                drawBoard();
            }

            if (m_Checkers.Board.getPlayerSolidersCount(m_Checkers.PlayerOne) - m_Checkers.Board.getPlayerSolidersCount(m_Checkers.PlayerTwo) > 0)
            {
                m_Checkers.PlayerOne.Score += m_Checkers.Board.getPlayerSolidersCount(m_Checkers.PlayerOne) - m_Checkers.Board.getPlayerSolidersCount(m_Checkers.PlayerTwo);
            }
            else
            {
                m_Checkers.PlayerTwo.Score += m_Checkers.Board.getPlayerSolidersCount(m_Checkers.PlayerTwo) - m_Checkers.Board.getPlayerSolidersCount(m_Checkers.PlayerOne);
            }

            if (m_Checkers.PlayerOne.Score > m_Checkers.PlayerTwo.Score)
            {
                DisplayWinnerMessage(m_Checkers.PlayerOne);
            }
            else if (m_Checkers.PlayerTwo.Score > m_Checkers.PlayerOne.Score)
            {
                DisplayWinnerMessage(m_Checkers.PlayerTwo);
            }
            else
            {
                DisplayTieMessage();
            }

            GameOverMessage();
        }

        public void InitGame(Board i_Checkers, Player i_PlayerOne, Player i_PlayerTwo)
        {
            i_PlayerOne.Name = GetPlayerNameFromInput();
            i_PlayerOne.IsWhite = false;

            uint boardSize = getSizeFromUserInput();
            Game.GamePlayer userOpponent = GetUserRival();

            if (userOpponent == Game.GamePlayer.Human)
            {
                i_PlayerTwo.Name = GetPlayerNameFromInput();
            }
            else
            {
                i_PlayerTwo.Name = Game.GamePlayer.Computer.ToString();
                i_PlayerTwo.isHuman = false;
            }

            clearBoard();
            i_Checkers.createEmptyGameBoard(boardSize);

            for (int column = 0; column < boardSize; column = column + 2)
            {
                for (int row = 0; row < (boardSize / 2) - 1; row++)
                {
                    if (row % 2 == 0)
                    {
                        i_Checkers.GameBoard[column, row].Value = Pieces.White;
                        i_Checkers.GameBoard[column + 1, boardSize - row - 1].Value = Pieces.Black;
                    }
                    else
                    {
                        i_Checkers.GameBoard[column + 1, row].Value = Pieces.White;
                        i_Checkers.GameBoard[column, boardSize - row - 1].Value = Pieces.Black;
                    }
                }
            }
        }

        public void drawBoard()
        {
            int boardSize = m_Checkers.Board.GameBoard.GetLength(0);
            DrawColumnHeader();

            for (int row = 0; row < boardSize; row++)
            {
                Console.WriteLine();

                DrawRowHeader(row);

                for (int column = 0; column < boardSize; column++)
                {
                    Console.Write("| ");
                    m_Checkers.Board.GameBoard[column, row].DrawCell();
                    Console.Write(" ");
                }

                Console.Write("|");
                Console.WriteLine();

                Console.Write("   ");
                for (int column = 0; column < (boardSize * (Board.MARGIN_SIZE - 1)) + 1; column++)
                {
                    Console.Write("=");
                }
            }

            Console.WriteLine();
        }

        public void clearBoard()
        {
            Ex02.ConsoleUtils.Screen.Clear();
        }

        private void DrawColumnHeader()
        {
            Console.WriteLine();
            Console.Write("    ");

            for (int column = 0; column < m_Checkers.Board.GameBoard.GetLength(0); column++)
            {
                Console.Write(" " + Convert.ToChar(column + Board.COLUMN_CAPITAL_LETTER) + "  ");
            }

            Console.WriteLine();

            Console.Write("   ");
            for (int column = 0; column < (m_Checkers.Board.GameBoard.GetLength(0) * (Board.MARGIN_SIZE - 1)) + 1; column++)
            {
                Console.Write("=");
            }
        }

        private void DrawRowHeader(int row)
        {
            Console.Write(" " + Convert.ToChar(row + Board.ROW_SMALL_LETTER) + " ");
        }
    }
}
