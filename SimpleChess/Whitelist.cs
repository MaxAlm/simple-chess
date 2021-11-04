using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChess
{
    class Whitelist
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Whitelist(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
