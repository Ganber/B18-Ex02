namespace B18_Ex02
{
    public class Cell
    {
        private const char EMPTY_CELL = ' ';
        private UI.Pieces m_cellValue = UI.Pieces.EmptyPiece;

        public UI.Pieces Value
        {
            get { return m_cellValue; }
            set { m_cellValue = value; }
        }

        public bool isEmpty()
        {
            return m_cellValue == UI.Pieces.EmptyPiece;
        }

        public bool isWhite()
        {
            return Value == UI.Pieces.White || Value == UI.Pieces.WhiteKing;
        }

        public bool isBlack()
        {
            return Value == UI.Pieces.Black || Value == UI.Pieces.BlackKing;
        }

        public bool isKing()
        {
            return Value == UI.Pieces.BlackKing || Value == UI.Pieces.WhiteKing;
        }

        public void DrawCell()
        {
            UI.DrawCell((char)m_cellValue);
        }
    }
}
