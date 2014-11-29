using IceFlake.Client;
using IceFlake.Client.Objects;
using IceFlake.Client.Scripts;

using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using D3D = IceFlake.DirectX.Direct3D;
using Mesh = SlimDX.Direct3D9.Mesh;

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
    #region DrawScript

    public class QKDrawScriptV5 : Script
    {
        public QKDrawScriptV5() 
            : base("_QKDrawScriptV5", "Drawing") 
        { }

        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>


        public override void OnStart()
        {
            LoadTexture();
        }

        public override void OnTerminate()
        {

        }

        public override void OnTick()
        {
            RenderCubeWithTexture();
        }

        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>

        public static VertexBuffer Vertices;    // Vertex buffer object used to hold vertices.


        public void RenderCubeWithTexture()
        {
            /*/
            _SB.Capture();

            // Create the vertex buffer and fill with the triangle vertices.
            Vertices = new VertexBuffer(D3D.Device, 3 * D3D.Vertex.SizeBytes, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            SlimDX.DataStream stream = Vertices.Lock(0, 0, LockFlags.None);
            stream.WriteRange(D3D.BuildVertexData());
            Vertices.Unlock();

            var worldMatrix = SlimDX.Matrix.Translation(loc);
            D3D.Device.SetTransform(TransformState.World, worldMatrix);

            D3D.Device.VertexFormat = D3D.Vertex.Format;

            // Render the vertex buffer.
            D3D.Device.SetStreamSource(0, Vertices, 0, D3D.Vertex.SizeBytes);
            /*/


            SlimDX.Vector3 _loc = Manager.LocalPlayer.Location.ToVector3();





            // Create the vertex buffer and fill with the triangle vertices.
            //Vertices = new VertexBuffer(D3D.CurrDevice, 3 * D3D.Vertex.SizeBytes, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            //SlimDX.DataStream stream = Vertices.Lock(0, 0, LockFlags.None);
            //stream.WriteRange(D3D.BuildVertexData());
            //Vertices.Unlock();

            //vertexBuffer = new VertexBuffer(typeof(Vertex),
            //                     cubeVertices.Length, D3D.CurrDevice,
            //                     Usage.Dynamic | Usage.WriteOnly,
            //                     Vertex.FVF_Flags,
            //                     Pool.Default);

            vertexBuffer = new VertexBuffer(D3D.CurrDevice, 3 * D3D.Vertex.SizeBytes, Usage.WriteOnly, VertexFormat.None, Pool.Managed);

            DataStream gStream = vertexBuffer.Lock(0, 0, LockFlags.None);

            // Now, copy the vertex data into the vertex buffer
            gStream.WriteRange(cubeVertices);

            var worldMatrix = SlimDX.Matrix.Translation(_loc);
            D3D.CurrDevice.SetTransform(TransformState.World, worldMatrix);
 

            D3D.CurrDevice.VertexFormat = Vertex.FVF_Flags;
            D3D.CurrDevice.SetStreamSource(0, Vertices, 0, 0);

            D3D.CurrDevice.SetTexture(0, texture);

            D3D.CurrDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            D3D.CurrDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 4, 2);
            D3D.CurrDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 8, 2);
            D3D.CurrDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 12, 2);
            D3D.CurrDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 16, 2);
            D3D.CurrDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 20, 2);





        }


        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>

        private VertexBuffer vertexBuffer = null;
        Texture texture = null;

        private void LoadTexture()
        {
            try
            {
                string _file = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Textures\\test.bmp";

                texture = Texture.FromFile(D3D.CurrDevice, _file);
                Log.WriteLine("texture path found : " + _file);
            }
            catch(Exception ex)
            {
                Log.LogException(ex);
            }

            D3D.CurrDevice.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
            D3D.CurrDevice.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);

        }


        struct Vertex
        {
            float x, y, z;
            float tu, tv;

            public Vertex(float _x, float _y, float _z, float _tu, float _tv)
            {
                x = _x; y = _y; z = _z;
                tu = _tu; tv = _tv;
            }

            public static readonly VertexFormat FVF_Flags = VertexFormat.Position | VertexFormat.Texture1;
        };


        /// <summary>
        /// Builds an array of vertices that can be written to a vertex buffer.
        /// </summary>
        /// <returns>An array of vertices.</returns>
        public static Vertex[] BuildCubeVertexData()
        {
            Vertex[] vertexData = new Vertex[3];

            vertexData[0].Position = new SlimDX.Vector3(-1.0f, -1.0f, 0.0f);
            vertexData[0].Normal = new SlimDX.Vector3(0.0f, 0.0f, -1.0f);

            vertexData[1].Position = new SlimDX.Vector3(1.0f, -1.0f, 0.0f);
            vertexData[1].Normal = new SlimDX.Vector3(0.0f, 0.0f, -1.0f);

            vertexData[2].Position = new SlimDX.Vector3(0.0f, 1.0f, 0.0f);
            vertexData[2].Normal = new SlimDX.Vector3(0.0f, 0.0f, -1.0f);

            return vertexData;
        }

        Vertex[] cubeVertices =
		{
			new Vertex(-1.0f, 1.0f,-1.0f,  0.0f,0.0f ),
			new Vertex( 1.0f, 1.0f,-1.0f,  1.0f,0.0f ),
			new Vertex(-1.0f,-1.0f,-1.0f,  0.0f,1.0f ),
			new Vertex( 1.0f,-1.0f,-1.0f,  1.0f,1.0f ),

			new Vertex(-1.0f, 1.0f, 1.0f,  1.0f,0.0f ),
			new Vertex(-1.0f,-1.0f, 1.0f,  1.0f,1.0f ),
			new Vertex( 1.0f, 1.0f, 1.0f,  0.0f,0.0f ),
			new Vertex( 1.0f,-1.0f, 1.0f,  0.0f,1.0f ),

			new Vertex(-1.0f, 1.0f, 1.0f,  0.0f,0.0f ),
			new Vertex( 1.0f, 1.0f, 1.0f,  1.0f,0.0f ),
			new Vertex(-1.0f, 1.0f,-1.0f,  0.0f,1.0f ),
			new Vertex( 1.0f, 1.0f,-1.0f,  1.0f,1.0f ),

			new Vertex(-1.0f,-1.0f, 1.0f,  0.0f,0.0f ),
			new Vertex(-1.0f,-1.0f,-1.0f,  1.0f,0.0f ),
			new Vertex( 1.0f,-1.0f, 1.0f,  0.0f,1.0f ),
			new Vertex( 1.0f,-1.0f,-1.0f,  1.0f,1.0f ),

			new Vertex( 1.0f, 1.0f,-1.0f,  0.0f,0.0f ),
			new Vertex( 1.0f, 1.0f, 1.0f,  1.0f,0.0f ),
			new Vertex( 1.0f,-1.0f,-1.0f,  0.0f,1.0f ),
			new Vertex( 1.0f,-1.0f, 1.0f,  1.0f,1.0f ),

			new Vertex(-1.0f, 1.0f,-1.0f,  1.0f,0.0f ),
			new Vertex(-1.0f,-1.0f,-1.0f,  1.0f,1.0f ),
			new Vertex(-1.0f, 1.0f, 1.0f,  0.0f,0.0f ),
			new Vertex(-1.0f,-1.0f, 1.0f,  0.0f,1.0f )
		};
    }

    #endregion
}
