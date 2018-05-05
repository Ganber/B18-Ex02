using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B18_Ex02
{
    public class Cell
    {
        private const char EMPTY_CELL = ' ';
        private Game.Pieces m_cellValue = Game.Pieces.EmptyPiece;


        public Game.Pieces Value
        {
            get { return m_cellValue; }
            set { m_cellValue = value; }
        }

      public bool isEmpty()
        {
            return m_cellValue == Game.Pieces.EmptyPiece;
        }
        public bool isWhite()
        {
            return (Value == Game.Pieces.White || Value == Game.Pieces.WhiteKing);
        }

        public bool isBlack()
        {
            return (Value == Game.Pieces.Black || Value == Game.Pieces.BlackKing);
        }

        public bool isKing()
        {
            return (Value == Game.Pieces.BlackKing || Value == Game.Pieces.WhiteKing);
        }


        public void DrawCell()
        {
            Console.Write((char)m_cellValue);
        }
    }
}
