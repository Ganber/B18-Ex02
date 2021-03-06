﻿namespace B18_Ex02
{
    public class Move
    {
        private int m_CurrentRow;
        private int m_CurrentCoulmn;
        private int m_NextRow;
        private int m_NextCoulmn;

        public Move(string i_MoveInput)
        {
            if (i_MoveInput.Length == 5)
            {
                m_CurrentCoulmn = i_MoveInput[0] - Board.COLUMN_CAPITAL_LETTER;
                m_CurrentRow = i_MoveInput[1] - Board.ROW_SMALL_LETTER;

                m_NextCoulmn = i_MoveInput[3] - Board.COLUMN_CAPITAL_LETTER;
                m_NextRow = i_MoveInput[4] - Board.ROW_SMALL_LETTER;
            }
        }

        public Move(int i_CurrentCoulmn, int i_CurrentRow, int i_NextCoulmn, int i_NextRow)
        {
            m_CurrentCoulmn = i_CurrentCoulmn;
            m_CurrentRow = i_CurrentRow;
            m_NextCoulmn = i_NextCoulmn;
            m_NextRow = i_NextRow;
        }

        public int CurrentRow
        {
            get { return m_CurrentRow; }
            set { m_CurrentRow = value; }
        }

        public int CurrentCoulmn
        {
            get { return m_CurrentCoulmn; }
            set { m_CurrentCoulmn = value; }
        }

        public int NextRow
        {
            get { return m_NextRow; }
            set { m_NextRow = value; }
        }

        public int NextColumn
        {
            get { return m_NextCoulmn; }
            set { m_NextCoulmn = value; }
        }

        public bool isMoveInputLegal(int i_boardSize)
        {
            bool legalMove = true;

            if (this.CurrentCoulmn < 0 || this.CurrentRow < 0 || this.NextColumn < 0 || this.NextRow < 0)
            {
                legalMove = false;
            }

            if (this.CurrentCoulmn >= i_boardSize || this.CurrentRow >= i_boardSize || this.NextColumn >= i_boardSize || this.NextRow >= i_boardSize)
            {
                legalMove = false;
            }

            return legalMove;
        }

        public string convertToString()
        {
            string resStr = string.Empty;

            resStr += (char)(this.CurrentCoulmn + Board.COLUMN_CAPITAL_LETTER);
            resStr += (char)(this.CurrentRow + Board.ROW_SMALL_LETTER);
            resStr += ">";
            resStr += (char)(this.NextColumn + Board.COLUMN_CAPITAL_LETTER);
            resStr += (char)(this.NextRow + Board.ROW_SMALL_LETTER);

            return resStr;
        }
    }
}
