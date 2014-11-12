using SlimDX;
using SlimDX.Direct3D9;
using System;
using System.Runtime.InteropServices;

namespace IceFlake.DirectX
{
    internal class CustomVertex
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PositionColored
        {
            public static readonly VertexFormat Format;
            public static readonly int Stride;
            public Vector3 Position;
            public int Color;
            static PositionColored()
            {
                Format = VertexFormat.Diffuse | VertexFormat.Position;
                Stride = Vector3.SizeInBytes + 4;
            }

            public PositionColored(Vector3 pos, int col)
            {
                this.Position = pos;
                this.Color = col;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TransformedColored
        {
            public static readonly VertexFormat Format;
            public static readonly int Stride;
            public Vector4 Position;
            public int Color;
            static TransformedColored()
            {
                Format = VertexFormat.Diffuse | VertexFormat.PositionRhw;
                Stride = Vector4.SizeInBytes + 4;
            }

            public TransformedColored(Vector4 pos, int col)
            {
                this.Position = pos;
                this.Color = col;
            }
        }
    }
}

