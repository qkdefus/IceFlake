using IceFlake.Client;
using IceFlake.Client.Objects;
using IceFlake.Client.Scripts;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using D3D = IceFlake.DirectX.Direct3D;

using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.Direct2D;
using SlimDX.Windows;

using Squid;
using SquidSlimDX;

//using SlimDX.Direct2D;
//using SlimDX.Windows;
//using SlimDX.Direct3D10;
//using SlimDX.Direct3D11;
//using SlimDX.D3DCompiler;
//using SlimDX.DXGI;


//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Content;
//using Xna = Microsoft.Xna.Framework;

//using SharpDX;
//using SharpDX.D3DCompiler;
//using SharpDX.Direct3D;
//using SharpDX.Direct3D11;
//using SharpDX.DXGI;
//using SharpDX.Windows;
//using Buffer = SharpDX.Direct3D11.Buffer;
//using Device = SharpDX.Direct3D11.Device;
//using MapFlags = SharpDX.Direct3D11.MapFlags;


namespace IceFlake.Scripts
{
    #region DrawMenuBasicScript

    public class QKDrawMenuBasicScript : Script
    {
        public QKDrawMenuBasicScript() 
            : base("_QKDrawMenuBasicScript", "Drawing") 
        { }

        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>

        public override void OnStart()
        {
            //InitializeD3D();
            //if (!Manager.ObjectManager.IsInGame)
            //{
            //    Stop();
            //    return;
            //}
        }

        public override void OnTerminate()
        {
            //if (!Manager.ObjectManager.IsInGame)
            //{
            //    Stop();
            //    return;
            //}
        }

        public override void OnTick()
        {

            string Title = "Cosmic";
            int SizeX = 150;
            int SizeY = 200;
            int RealPointX = 50;
            int RealPointY = 150;
            System.Drawing.Color TitleTextColor = DirectX.Graphics.HexColor("0", 100);
            System.Drawing.Color TitleColor = DirectX.Graphics.HexColor("33ff66", 100);
            System.Drawing.Color BackgroundColor = DirectX.Graphics.HexColor("4e4e4e", 100);
            System.Drawing.Color BorderColor = DirectX.Graphics.HexColor("33ff66", 100);
            DrawControl2D(Title, SizeX, SizeY, RealPointX, RealPointY, TitleTextColor, TitleColor, BackgroundColor, BorderColor);
            // WireFrame World
            //D3D.Device.SetRenderState(RenderState.FillMode, SlimDX.Direct3D9.FillMode.Wireframe);
        }

        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>

        // Draw Font
        internal SlimDX.Direct3D9.Font _font = new SlimDX.Direct3D9.Font(D3D.CurrDevice, new System.Drawing.Font("Arial", 10f, FontStyle.Bold));
        internal void DrawString2D(string Text, int X, int Y, System.Drawing.Color Color, string FontName, bool FontStyleBold = false)
        {
            if (FontStyleBold)
                _font = new SlimDX.Direct3D9.Font(D3D.CurrDevice, new System.Drawing.Font(FontName, 12f, FontStyle.Bold));
            else
                _font = new SlimDX.Direct3D9.Font(D3D.CurrDevice, new System.Drawing.Font(FontName, 12f));

            DrawString2D(Text, X, Y, Color);
        }
        internal void DrawString2D(string Text, int X, int Y, System.Drawing.Color Color)
        {
            _font.DrawString(null, Text, X, Y, (SlimDX.Color4)Color);
        }

        // DrawControl2D
        internal void DrawControl2D(string Title, int SizeX, int SizeY, int PosX, int PosY, System.Drawing.Color TitleTextColor, System.Drawing.Color TitleColor, System.Drawing.Color BackgroundColor, System.Drawing.Color BorderColor)
        {
            int num = 18;
            int num2 = PosY + 1;
            if (SizeY >= 18)
            {
                // Draw Header
                SlimDX.Direct3D9.Line line = new SlimDX.Direct3D9.Line(D3D.CurrDevice);
                List<SlimDX.Vector2> list = new List<SlimDX.Vector2>();
                for (int i = 0; i < num; i++)
                {
                    list.Add(new SlimDX.Vector2((float)PosX, (float)num2));
                    list.Add(new SlimDX.Vector2((float)(PosX + SizeX), (float)num2));
                    num2++;
                }
                line.Draw(list.ToArray(), TitleColor);


                // Draw Title
                DrawString2D(Title, PosX + 4, PosY + 1, TitleTextColor);

                // Draw Background
                List<SlimDX.Vector2> list2 = new List<SlimDX.Vector2>();
                for (int j = num; j < SizeY; j++)
                {
                    list2.Add(new SlimDX.Vector2((float)PosX, (float)num2));
                    list2.Add(new SlimDX.Vector2((float)(PosX + SizeX), (float)num2));
                    num2++;
                }
                line.Draw(list2.ToArray(), BackgroundColor);

                // Draw top border
                SlimDX.Vector2[] vertexList = new SlimDX.Vector2[] { new SlimDX.Vector2((float)PosX, (float)PosY), new SlimDX.Vector2((float)(PosX + SizeX), (float)PosY) };
                line.Draw(vertexList, BorderColor);

                // Draw bottom border
                SlimDX.Vector2[] vectorArray2 = new SlimDX.Vector2[] { new SlimDX.Vector2((float)PosX, (float)(PosY + SizeY)), new SlimDX.Vector2((float)((PosX + SizeX) + 1), (float)(PosY + SizeY)) };
                line.Draw(vectorArray2, BorderColor);

                // Draw right border
                SlimDX.Vector2[] vectorArray3 = new SlimDX.Vector2[] { new SlimDX.Vector2((float)(PosX + SizeX), (float)PosY), new SlimDX.Vector2((float)(PosX + SizeX), (float)(PosY + SizeY)) };
                line.Draw(vectorArray3, BorderColor);

                // Draw left border
                SlimDX.Vector2[] vectorArray4 = new SlimDX.Vector2[] { new SlimDX.Vector2((float)PosX, (float)PosY), new SlimDX.Vector2((float)PosX, (float)(PosY + SizeY)) };
                line.Draw(vectorArray4, BorderColor);

                // Dispose
                line.Dispose();
            }
        }

        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>



        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>



        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>
    }

    #endregion
}
