using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSDYNAParser
{
    class Section<T> : Connection<T>
    {
        private IList<Card> cards;

        public Section()
        {
            
        }

        public IList<Card> Cards
        {
            get
            {
                return cards;
            }
            set
            {
                cards = value;
            }
        }
    }
}
