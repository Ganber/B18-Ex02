using System;
using System.Text;
using System.Text.RegularExpressions;

namespace B18_Ex02
{
    public class UI
    {
        private const string cantMoveHereMessage = "Error! Wrong move!";
        private const string incorrectInputMessage = "Error! Incorrect input.";
        private const string cantSelectMessage = "Error! Cannot select!";
        private const string enterPlayerNameMessage = "Hello! please enter your name (20 letters max)";
        private const string playAgainstComputerOrPlayerMessage = "enter 1 to play against computer or 2 to play against human player";
        private static uint m_BoardSize;

        private enum playerSymbol { X=0, O=1 };

        public static uint getSizeFromUserInput()
        {
            Console.WriteLine("Please enter board size: (6, 8, 10)");
            while(!uint.TryParse(Console.ReadLine(), out m_BoardSize) || 
                (m_BoardSize != Game.SMALL_BOARD_SIZE && m_BoardSize != Game.MEDIUM_BOARD_SIZE && m_BoardSize != Game.BIG_BOARD_SIZE))
            {
                Console.WriteLine(incorrectInputMessage);
            }

            return m_BoardSize;
        }

        public static void DisplayCurrentPlayerMessage(HumanPlayer currentPlayer)
        {
            playerSymbol symbol = (playerSymbol) Convert.ToInt32(currentPlayer.IsWhite);
            Console.WriteLine(currentPlayer.Name + "'s turn: (" + symbol + "):");
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

            while(playerName.Length > 20 || !Regex.IsMatch(playerName, @"^[a-zA-Z]+$"))
            {
                Console.WriteLine(incorrectInputMessage);
                playerName = Console.ReadLine();
            }

            return playerName;
        }

        public static Game.opponent GetUserRival()
        {
            Console.WriteLine(playAgainstComputerOrPlayerMessage);
            int userInput;
            int.TryParse(Console.ReadLine(), out userInput);

            while (userInput != (int)Game.opponent.Computer && userInput != (int)Game.opponent.Human)
            {
                DisplayIncorrectInputMessage();
                int.TryParse(Console.ReadLine(), out userInput);
            }

            return (Game.opponent) userInput;
        }

        public static void DisplayLastPlayerMove(HumanPlayer i_PreviousPlayer, string i_inputMove)
        {
            playerSymbol symbol;

            if (i_PreviousPlayer.IsWhite)
            {
                symbol = playerSymbol.X;
            }
            else
            {
                symbol = playerSymbol.O;
            }

            Console.WriteLine(i_PreviousPlayer.Name + "'s move was (" + symbol.ToString() + "): " + i_inputMove);
        }

        public static void DisplayCanEatMoreMessage()
        {
            Console.WriteLine("You can eat more");
        }

        public static void GameOverMessage()
        {
            Console.SetCursorPosition(50, 10);
            Console.Write("Game Over");
            Console.ReadLine();
        }
    }
}
