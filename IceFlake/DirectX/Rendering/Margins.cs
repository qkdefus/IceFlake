using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace IceFlake.DirectX
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Margins
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
        public bool Contains(Point p)
        {
            if (p.X > this.Right)
            {
                return false;
            }
            if (p.Y > this.Bottom)
            {
                return false;
            }
            if (p.X < this.Left)
            {
                return false;
            }
            if (p.Y < this.Top)
            {
                return false;
            }
            return true;
        }
    }
}

