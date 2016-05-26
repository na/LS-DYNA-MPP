using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSDYNAParser
{
    class ConnectionList<T> : Collection<Connection<T>>
    {
        public ConnectionList() : base() { }

        public ConnectionList(int initialSize)
        {
            for (int i = 0; i < initialSize; i++)
                base.Items.Add(default(Connection<T>));
        }

        public Connection<T> FindByValue(T value)
        {
            foreach (Connection<T> node in Items)
                if (node.Value.Equals(value))
                    return node;
            
            return null;
        }
    }
}
