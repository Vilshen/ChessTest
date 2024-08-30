using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Godot;

namespace Chess.Pieces
{
    public interface IPiece_Directional : IPiece
    {
        public int orientation { get; set; }

        public void Flip();
        internal static int Flip(int original)
        {
            return original * -1;

        }

    }
}
