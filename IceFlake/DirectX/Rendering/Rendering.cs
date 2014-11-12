using SlimDX;
using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace IceFlake.DirectX
{
    internal class Rendering
    {
        //private float aspectRatio;
        //private WoWCamera camera;
        //public bool chests;
        //private Direct3D d3d;
        //private Device device;
        //private SlimDX.Direct3D9.Font font;
        //private long frames;
        public const int GWL_EXSTYLE = -20;
        private Line line;
        public const int LWA_ALPHA = 2;
        public const int LWA_COLORKEY = 1;
        //public bool objects;
        //public bool players;
        //private PresentParameters presentParameters;
        private Size screenSize;
        //private int startTime;
        //public bool units;
        //private IntPtr wowWindow;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_TRANSPARENT = 0x20;

        public void DrawBoxLines(SlimDX.Vector3 pos, Color Color, float size, Viewport viewport, Matrix worldViewProj, ref Margins bounds)
        {
            int num2;
            SlimDX.Vector3[] points = new SlimDX.Vector3[8];
            float[] numArray = new float[] { pos.X + size, pos.X + size, pos.X - size, pos.X - size, pos.X + size, pos.X + size, pos.X - size, pos.X - size };
            float[] numArray2 = new float[] { pos.Y - size, pos.Y + size, pos.Y + size, pos.Y - size, pos.Y - size, pos.Y + size, pos.Y + size, pos.Y - size };
            float[] numArray3 = new float[] { pos.Z, pos.Z, pos.Z, pos.Z, pos.Z + (size * 2f), pos.Z + (size * 2f), pos.Z + (size * 2f), pos.Z + (size * 2f) };
            for (int i = 0; i < 8; i = num2 + 1)
            {
                bool flag;
                points[i] = new SlimDX.Vector3(numArray[i], numArray2[i], numArray3[i]).WorldToScreen(viewport, worldViewProj, out flag);
                if (!flag)
                {
                    return;
                }
                num2 = i;
            }
            this.FigureScreenBounds(points, ref bounds);
            SlimDX.Vector3[] vectorArray1 = new SlimDX.Vector3[] { points[3], points[0], points[1], points[2] };
            this.DrawPolyLine(1f, Color, vectorArray1);
            SlimDX.Vector3[] vectorArray2 = new SlimDX.Vector3[] { points[7], points[4], points[5], points[6] };
            this.DrawPolyLine(1f, Color, vectorArray2);
            for (int j = 0; j < 4; j = num2 + 1)
            {
                this.DrawLine(points[j].X, points[j].Y, points[j + 4].X, points[j + 4].Y, 1f, Color);
                num2 = j;
            }
        }

        public void DrawLine(float x1, float y1, float x2, float y2, float w, Color Color)
        {
            Vector2[] vertexList = new Vector2[] { new Vector2(x1, y1), new Vector2(x2, y2) };
            this.line.GLLines = true;
            this.line.Antialias = false;
            this.line.Width = w;
            this.line.Begin();
            this.line.Draw(vertexList, new Color4(Color));
            this.line.End();
        }

        public void DrawPolyLine(float w, Color Color, params SlimDX.Vector3[] points)
        {
            int num2;
            Vector2[] vertexList = new Vector2[points.Length + 1];
            for (int i = 0; i < points.Length; i = num2)
            {
                vertexList[i] = new Vector2(points[i].X, points[i].Y);
                num2 = i + 1;
            }
            vertexList[points.Length] = new Vector2(points[0].X, points[0].Y);
            this.line.GLLines = true;
            this.line.Antialias = false;
            this.line.Width = w;
            this.line.Begin();
            this.line.Draw(vertexList, new Color4(Color));
            this.line.End();
        }

        private bool FigureScreenBounds(SlimDX.Vector3[] points, ref Margins rect)
        {
            int num2;
            rect.Left = this.screenSize.Width;
            rect.Right = 0;
            rect.Top = this.screenSize.Height;
            rect.Bottom = 0;
            for (int i = 0; i < points.Length; i = num2 + 1)
            {
                if (points[i].X > rect.Right)
                {
                    rect.Right = (int)points[i].X;
                }
                if (points[i].Y > rect.Bottom)
                {
                    rect.Bottom = (int)points[i].Y;
                }
                if (points[i].X < rect.Left)
                {
                    rect.Left = (int)points[i].X;
                }
                if (points[i].Y < rect.Top)
                {
                    rect.Top = (int)points[i].Y;
                }
                num2 = i;
            }
            if ((((rect.Left < -200) || (rect.Right > (this.screenSize.Width + 200))) || (rect.Top < -200)) || (rect.Bottom > (this.screenSize.Height + 200)))
            {
                rect.Top = -1;
                rect.Bottom = -1;
                rect.Left = -1;
                rect.Right = -1;
                return false;
            }
            return true;
        }
    }
}

