﻿using System;

namespace B18_Ex02
{
    public class Board
    {
        public const int COLUMN_CAPITAL_LETTER = 65;
        public const int ROW_SMALL_LETTER = 97;
        private const int MARGIN_SIZE = 5;

        private static Cell[,] m_gameBoard;

        public Cell[,] GameBoard
        {
            get { return m_gameBoard; }
        }

        public Cell[,] createEmptyGameBoard(uint size)
        {
            m_gameBoard = new Cell[size, size];

            for (int i = 0; i < m_gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < m_gameBoard.GetLength(0); j++)
                {
                    m_gameBoard[i, j] = new Cell();
                }
            }

            return m_gameBoard;
        }

        private static void DrawColumnHeader()
        {
            Console.WriteLine();
            Console.Write("    ");

            for (int column = 0; column < m_gameBoard.GetLength(0); column++)
            {
                Console.Write(" " + Convert.ToChar(column + COLUMN_CAPITAL_LETTER) + "  ");
            }
            Console.WriteLine();

            Console.Write("   ");
            for (int column = 0; column < m_gameBoard.GetLength(0) * (MARGIN_SIZE - 1) + 1; column++)
            {
                Console.Write("=");
            }
        }

        private static void DrawRowHeader(int row)
        {
            Console.Write(" " + Convert.ToChar(row + ROW_SMALL_LETTER) + " ");
        }

        public void drawBoard()
        {
            DrawColumnHeader();

            for (int row = 0; row < m_gameBoard.GetLength(0); row++)
            {
                Console.WriteLine();

                DrawRowHeader(row);

                for (int column = 0; column < m_gameBoard.GetLength(0); column++)
                {
                    Console.Write("| ");
                    m_gameBoard[column, row].DrawCell(); 
                    Console.Write(" ");
                }
                Console.Write("|");
                Console.WriteLine();

                Console.Write("   ");
                for (int column = 0; column < m_gameBoard.GetLength(0) * (MARGIN_SIZE - 1) + 1; column++)
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
    }
}
