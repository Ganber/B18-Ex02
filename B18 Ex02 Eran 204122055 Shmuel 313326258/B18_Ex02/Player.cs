using System;

namespace B18_Ex02
{
    public class Player
    {
        private string m_name;
        private bool m_isWhite = true;
        private bool m_isHuman = true;
        
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }      
        
        public bool isHuman
        {
            get { return m_isHuman; }
            set { m_isHuman = value; }
        }
        
        public bool IsWhite
        {
            get { return m_isWhite; }
            set { m_isWhite = value; }
        }  
    }
}
