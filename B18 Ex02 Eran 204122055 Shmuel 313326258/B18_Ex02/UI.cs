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
        public const char RESIGN_GAME = 'Q';

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
            if (i_LastMove != null)
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
        }

        public static void DisplayCanEatMoreMessage()
        {
            Console.WriteLine("You can eat more");
        }

        public static void DisplayWatingToPcMove()
        {
            Console.WriteLine("Please Wait to PC move..");
            System.Threading.Thread.Sleep(100);
        }

        public static void DisplayGameOverMessage()
        {

            Console.WriteLine("Match is over !");
            
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
            bool userKeepPlaying = true;
            InitGame(m_Checkers.Board, m_Checkers.PlayerOne, m_Checkers.PlayerTwo);
            while (userKeepPlaying)
            {
                Player CurrentPlayer = m_Checkers.PlayerOne;
                Move lastMove = null;
                bool isSequenceEating = false;
                m_Checkers.ResetValues();
                m_Checkers.Board.InitGameBoard((uint)m_Checkers.Board.GameBoard.GetLength(0));

                while (!m_Checkers.GameOver)
                {
                    clearBoard();
                    drawBoard();
                    DisplayLastPlayerMove(m_Checkers.getOpponent(CurrentPlayer), lastMove);
                    DisplayCurrentPlayerMessage(CurrentPlayer);
                    m_Checkers.MakeNextMove(ref CurrentPlayer, ref lastMove, ref isSequenceEating);
                }

                m_Checkers.UpdateScore();
                clearBoard();
                drawBoard();

                DisplayGameOverMessage();
                DisplayEndMatchResult();
                DisplayCurrentGameScore(m_Checkers);
                DisplayAnotherGameMessage();
                userKeepPlaying = GetKeepPlayingInput();
            }

            DisplayQuitGameMessage();
        }

        private void DisplayQuitGameMessage()
        {
            Console.WriteLine("Thank you for playing Checkers !");
        }

        private bool GetKeepPlayingInput()
        {
            string PlayerDecision = Console.ReadLine();
            bool res = false;
            while (PlayerDecision != "Y" && PlayerDecision != "N")
            {
                Console.WriteLine("Please type Y for yes, N for no");
                PlayerDecision = Console.ReadLine();
            }
            if (PlayerDecision == "Y")
                res = true;


            return res;
        }

        private void DisplayAnotherGameMessage()
        {
            Console.WriteLine("Do you want to play another match ? Y/N");
        }

        private void DisplayCurrentGameScore(Game m_Checkers)
        {
            Console.WriteLine(m_Checkers.PlayerOne.Name + " current score is: " + m_Checkers.PlayerOne.Score);
            Console.WriteLine(m_Checkers.PlayerTwo.Name + " current score is: " + m_Checkers.PlayerTwo.Score);
        }

        private void DisplayEndMatchResult()
        {
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
            i_Checkers.InitGameBoard(boardSize);


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
