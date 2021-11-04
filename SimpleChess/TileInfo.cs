using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChess
{
    class TileInfo
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int TYPE { get; set; }
        public int COLOR { get; set; }
        public bool FIRST { get; set; }
        public bool SELECTED { get; set; }

        public TileInfo(int X, int Y, int TYPE, int COLOR, bool FIRST, bool SELECTED)
        {
            this.X = X;
            this.Y = Y;
            this.TYPE = TYPE;
            this.COLOR = COLOR;
            this.FIRST = FIRST;
            this.SELECTED = SELECTED;
        }
    }
}
