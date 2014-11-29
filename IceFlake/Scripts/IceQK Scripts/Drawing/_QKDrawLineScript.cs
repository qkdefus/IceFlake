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
    #region DrawLineScript

    public class QKDrawLineScript : Script
    {
        public QKDrawLineScript() 
            : base("_QKDrawLineScript", "Drawing") 
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


            //var vertFrom = new DirectX.Vector2D(Manager.LocalPlayer.Location.X, Manager.LocalPlayer.Location.Y);
            //var vertTo = new DirectX.Vector2D(Manager.LocalPlayer.Target.Location.X, Manager.LocalPlayer.Target.Location.Y);


            var vertFrom = new DirectX.Vector3D(Manager.LocalPlayer.Location.X, Manager.LocalPlayer.Location.Y, Manager.LocalPlayer.Location.Z);
            var vertTo = new DirectX.Vector3D(Manager.LocalPlayer.Target.Location.X, Manager.LocalPlayer.Target.Location.Y, Manager.LocalPlayer.Target.Location.Z);

            RenderLine(vertFrom, vertTo, 0.6f, new Color4(_lineColor.ToArgb()));
        }

        private System.Drawing.Color _lineColor = DirectX.Graphics.HexColor("33ff66", 200);


        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>

        //private Ellipse ellipse;
        //private float lineThickness;
        //private SolidColorBrush fillBrush;
        //private SolidColorBrush lineBrush;

        //private RectangleF rectangle;

        //private void DrawLine(RenderTarget renderTarget)
        //{

        //    fillBrush = GetSolidColorBrush(FillColor, Opacity);
        //    lineBrush = GetSolidColorBrush(LineColor, Opacity);
        //    if (Shape == ShapeType.Ellipse)
        //    {
        //        ellipse.Center = new PointF(transformComponent.X, transformComponent.Y);
        //        ellipse.RadiusX = transformComponent.Width / 2.0f;
        //        ellipse.RadiusY = transformComponent.Height / 2.0f;
        //        renderTarget.FillEllipse(fillBrush, ellipse);
        //        renderTarget.DrawEllipse(lineBrush, ellipse, LineThickness);
        //    }

            
        //}


        //public SolidColorBrush GetSolidColorBrush(Color4 color, float opacity)
        //{
        //    return GetSolidColorBrush(color, opacity);
        //}




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

        // FlatVertex
        public struct FlatVertex
        {
            // Vertex format
            public static readonly int Stride = 6 * 4;

            // Members
            public float x;
            public float y;
            public float z;
            public int c;
            public float u;
            public float v;
        }

        private void SetTarget(Vector3 target, float yaw = 0, float pitch = 0, float roll = 0)
        {
            var worldMatrix = Matrix.Translation(target) * Matrix.RotationYawPitchRoll(yaw, pitch, roll);
            D3D.CurrDevice.SetTransform(TransformState.World, worldMatrix);
        }

        private SlimDX.Direct3D9.StateBlock _SB = new SlimDX.Direct3D9.StateBlock(D3D.CurrDevice, StateBlockType.All);




        // This renders a line with given color
        public void RenderLine(DirectX.Vector3D start, DirectX.Vector3D end, float thickness, Color4 c)
        {
            _SB.Capture();
            FlatVertex[] verts = new FlatVertex[4];
            // Calculate World Matrix
            DirectX.Vector3D delta = end - start;
            DirectX.Vector3D dn = delta.GetNormal() * thickness;
            // Make vertices
            verts[0].x = start.x - dn.x + dn.y;
            verts[0].y = start.y - dn.y - dn.x;
            verts[0].z = start.z + 0.2f;
            verts[0].c = c.ToArgb();
            verts[1].x = start.x - dn.x - dn.y;
            verts[1].y = start.y - dn.y + dn.x;
            verts[1].z = start.z + 0.2f;
            verts[1].c = c.ToArgb();
            verts[2].x = end.x + dn.x + dn.y;
            verts[2].y = end.y + dn.y - dn.x;
            verts[2].z = end.z;
            verts[2].c = c.ToArgb();
            verts[3].x = end.x + dn.x - dn.y;
            verts[3].y = end.y + dn.y + dn.x;
            verts[3].z = end.z;
            verts[3].c = c.ToArgb();
            // Render
            D3D.CurrDevice.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, verts);

            _SB.Apply();
        }


















        //// This renders a line with given color
        //public void RenderLine(DirectX.Vector2D start, DirectX.Vector2D end, float thickness, Color4 c, bool transformcoords = false)
        //{
        //    _SB.Capture();

        //    FlatVertex[] verts = new FlatVertex[4];

        //    DirectX.Vector2D delta = end - start;
        //    DirectX.Vector2D dn = delta.GetNormal() * thickness;
        //    // Make vertices
        //    verts[0].x = start.x - dn.x + dn.y;
        //    verts[0].y = start.y - dn.y - dn.x;
        //    verts[0].z = 0.0f;
        //    verts[0].c = c.ToArgb();
        //    verts[1].x = start.x - dn.x - dn.y;
        //    verts[1].y = start.y - dn.y + dn.x;
        //    verts[1].z = 0.0f;
        //    verts[1].c = c.ToArgb();
        //    verts[2].x = end.x + dn.x + dn.y;
        //    verts[2].y = end.y + dn.y - dn.x;
        //    verts[2].z = 0.0f;
        //    verts[2].c = c.ToArgb();
        //    verts[3].x = end.x + dn.x - dn.y;
        //    verts[3].y = end.y + dn.y + dn.x;
        //    verts[3].z = 0.0f;
        //    verts[3].c = c.ToArgb();
        //    // Set renderstates for rendering
        //    //D3D.Device.SetRenderState(RenderState.CullMode, Cull.None);
            
        //    D3D.Device.SetRenderState(RenderState.ZEnable, false);

        //    D3D.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, verts);

        //    _SB.Apply();
        //}

        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>




















        //// This renders a line with given color
        //public void RenderLine(DirectX.Vector2D start, DirectX.Vector2D end, float thickness, Color4 c, bool transformcoords = false)
        //{
        //    FlatVertex[] verts = new FlatVertex[4];
        //    // Calculate positions
        //    //if (transformcoords)
        //    //{
        //    //    start = start.GetTransformed(translatex, translatey, scale, -scale);
        //    //    end = end.GetTransformed(translatex, translatey, scale, -scale);
        //    //}
        //    // Calculate offsets
        //    DirectX.Vector2D delta = end - start;
        //    DirectX.Vector2D dn = delta.GetNormal() * thickness;
        //    // Make vertices
        //    verts[0].x = start.x - dn.x + dn.y;
        //    verts[0].y = start.y - dn.y - dn.x;
        //    verts[0].z = 0.0f;
        //    verts[0].c = c.ToArgb();
        //    verts[1].x = start.x - dn.x - dn.y;
        //    verts[1].y = start.y - dn.y + dn.x;
        //    verts[1].z = 0.0f;
        //    verts[1].c = c.ToArgb();
        //    verts[2].x = end.x + dn.x + dn.y;
        //    verts[2].y = end.y + dn.y - dn.x;
        //    verts[2].z = 0.0f;
        //    verts[2].c = c.ToArgb();
        //    verts[3].x = end.x + dn.x - dn.y;
        //    verts[3].y = end.y + dn.y + dn.x;
        //    verts[3].z = 0.0f;
        //    verts[3].c = c.ToArgb();
        //    // Set renderstates for rendering
        //    D3D.Device.SetRenderState(RenderState.CullMode, Cull.None);
        //    D3D.Device.SetRenderState(RenderState.ZEnable, false);
        //    D3D.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
        //    //D3D.Device.SetRenderState(RenderState.AlphaTestEnable, false);
        //    //D3D.Device.SetRenderState(RenderState.TextureFactor, -1);
        //    //D3D.Device.SetRenderState(RenderState.FogEnable, false);
        //    //SetWorldTransformation(false);
        //    D3D.Device.SetTexture(0, null);
        //    D3D.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, verts);








        //    //D3D.Device.SetTexture(0, General.Map.Data.WhiteTexture.Texture);
        //    //D3D.Shaders.Display2D.Texture1 = General.Map.Data.WhiteTexture.Texture;
        //    //D3D.Shaders.Display2D.SetSettings(1f, 1f, 0f, 1f, General.Settings.ClassicBilinear);
        //    // Draw
        //    //D3D.Shaders.Display2D.Begin();
        //    //D3D.Shaders.Display2D.BeginPass(0);
        //    //D3D.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, verts);
        //    //D3D.Shaders.Display2D.EndPass();
        //    //D3D.Shaders.Display2D.End();
        //}

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
