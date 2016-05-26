using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSDYNAParser
{
    public class Connection<T>
    {
        private T data;
        private ConnectionList<T> neighbors = null;

        public Connection() { }
        public Connection(T data) : this(data, null) { }
        public Connection(T data, ConnectionList<T> neighbors)
        {
            this.data = data;
            this.neighbors = neighbors;
        }

        public T Value
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        protected ConnectionList<T> Neighbors
        {
            get
            {
                return neighbors;
            }
            set
            {
                neighbors = value;
            }
        }
    }
}
