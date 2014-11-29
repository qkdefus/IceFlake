using IceFlake.Client;
using IceFlake.Client.Objects;
using IceFlake.Client.Scripts;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using D3D = IceFlake.DirectX.Direct3D;
//using REN = IceFlake.DirectX;
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
    #region DrawLineScriptV2

    public class QKDrawLineScriptV2 : Script
    {
        public QKDrawLineScriptV2() 
            : base("_QKDrawLineScriptV2", "Drawing") 
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

            //DrawLine3D(Manager.LocalPlayer.Location.ToVector3(), Manager.LocalPlayer.Target.Location.ToVector3(), 10, _lineColor);

            
        }

        private System.Drawing.Color _lineColor = DirectX.Graphics.HexColor("33ff66", 200);
        private SlimDX.Direct3D9.StateBlock _SB = new SlimDX.Direct3D9.StateBlock(D3D.Device, StateBlockType.All);

        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>

        public void DrawLine2D(float x1, float y1, float x2, float y2, float w, Color Color)
        {
            Line line = new Line(D3D.Device);
            Vector2[] vertexList = new Vector2[] { new Vector2(x1, y1), new Vector2(x2, y2) };
            line.GLLines = true;
            line.Antialias = false;
            line.Width = w;
            line.Begin();
            SetTarget(Vector3.Zero);
            line.Draw(vertexList, new Color4(Color));
            line.End();
        }

        // bugs when turning camera (looking behind)
        public void DrawLine3D(Vector3 fromLoc, Vector3 toLoc, float w, Color Color)
        {
            //_SB.Capture();
            Line line = new Line(D3D.Device);
            Vector3[] vertexList = new Vector3[] { fromLoc, toLoc};
            line.GLLines = true;
            line.Antialias = false;
            line.Width = w;
            line.Begin();
            SetTarget(Vector3.Zero);
            Matrix worldViewProjection = Manager.Camera.View * Manager.Camera.Projection;
            line.DrawTransformed(vertexList, worldViewProjection, new Color4(Color));
            line.End();
            line.Dispose();
            //_SB.Apply();
        }

        private void SetTarget(Vector3 target, float yaw = 0, float pitch = 0, float roll = 0)
        {
            var worldMatrix = Matrix.Translation(target) * Matrix.RotationYawPitchRoll(yaw, pitch, roll);
            D3D.Device.SetTransform(TransformState.World, worldMatrix);
        }



        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>
        /// 




 



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
