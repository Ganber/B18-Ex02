namespace B18_Ex02
{
    public class Board
    {
        public const int COLUMN_CAPITAL_LETTER = 65;
        public const int ROW_SMALL_LETTER = 97;
        public const int MARGIN_SIZE = 5;

        private static Cell[,] m_gameBoard;

        public Cell[,] GameBoard
        {
            get { return m_gameBoard; }
        }

        public Cell[,] InitGameBoard(uint i_size)
        {
            m_gameBoard = new Cell[i_size, i_size];

            for (int i = 0; i < m_gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < m_gameBoard.GetLength(0); j++)
                {
                    m_gameBoard[i, j] = new Cell();
                }
            }

            for (int column = 0; column < i_size; column = column + 2)
            {
                for (int row = 0; row < (i_size / 2) - 1; row++)
                {
                    if (row % 2 == 0)
                    {
                        GameBoard[column, row].Value = UI.Pieces.White;
                        GameBoard[column + 1, i_size - row - 1].Value = UI.Pieces.Black;
                    }
                    else
                    {
                        GameBoard[column + 1, row].Value = UI.Pieces.White;
                        GameBoard[column, i_size - row - 1].Value = UI.Pieces.Black;
                    }
                }
            }

            return m_gameBoard;
        }

        public int getPlayerSolidersCount(Player i_CurrentPlayer)
        {
            int solidersCount = 0;
            int boardSize = m_gameBoard.GetLength(0);

            for (int currentCol = 0; currentCol < boardSize; currentCol++)
            {
                for (int currentRow = 0; currentRow < boardSize; currentRow++)
                {
                    if (i_CurrentPlayer.IsWhite)
                    {
                        if (m_gameBoard[currentCol, currentRow].Value == UI.Pieces.White)
                        {
                            solidersCount++;
                        }

                        if (m_gameBoard[currentCol, currentRow].Value == UI.Pieces.WhiteKing)
                        {
                            solidersCount += 4;
                        }
                    }
                    else
                    {
                        if (m_gameBoard[currentCol, currentRow].Value == UI.Pieces.Black)
                        {
                            solidersCount++;
                        }

                        if (m_gameBoard[currentCol, currentRow].Value == UI.Pieces.BlackKing)
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
