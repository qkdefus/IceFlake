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
        /// ///////////////////////////////                         NOT WORKING
        /// ///////////////////////////////
        /// </summary>


        public override void OnStart()
        {


            List<string> colors = new List<string>();

            foreach (string colorName in Enum.GetNames(typeof(KnownColor)))
            {
                //cast the colorName into a KnownColor
                KnownColor knownColor = (KnownColor)Enum.Parse(typeof(KnownColor), colorName);
                //check if the knownColor variable is a System color
                if (knownColor > KnownColor.Transparent)
                {
                    //add it to our list
                    colors.Add(colorName);

                    Log.WriteLine(colorName.ToString());

                }
            }




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

            SlimDX.Vector3 _loc = Manager.LocalPlayer.Location.ToVector3();


            var worldMatrix = SlimDX.Matrix.Translation(_loc);
            D3D.CurrDevice.SetTransform(TransformState.World, worldMatrix);



            // Turn off culling, so we see the front and back of the triangle
            D3D.CurrDevice.SetRenderState(RenderState.CullMode, Cull.None);

            // Turn off lighting
            D3D.CurrDevice.SetRenderState(RenderState.Lighting, false);









            // Now drawing 2 triangles, for a quad.
            D3D.CurrDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);

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

                // Create the vertex buffer and fill with the triangle vertices. (Non-indexed)
                // Remember 3 vetices for a triangle, 2 tris per quad = 6.
                Vertices = new VertexBuffer(D3D.CurrDevice, 6 * Vertex.SizeBytes, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                DataStream stream = Vertices.Lock(0, 0, LockFlags.None);
                stream.WriteRange(BuildVertexData());
                Vertices.Unlock();

                // Create the texture.
                // Be sure to either use the full path, or place the texture in the app's root dir.
                // If DX can't find the file you will get an InvalidDataException.
                //texture = Texture.FromFile(D3D.CurrDevice, "earth.bmp");
                texture = Texture.FromFile(D3D.CurrDevice, _file);
                Log.WriteLine("texture path found : " + _file);

            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }

            // Setup our texture. Using Textures introduces the texture stage states,
            // which govern how Textures get blended together (in the case of multiple
            // Textures) and lighting information.
            D3D.CurrDevice.SetTexture(0, texture);

            // The sampler states govern how smooth the texture is displayed.
            D3D.CurrDevice.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
            D3D.CurrDevice.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
            D3D.CurrDevice.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Linear);

        }


        /// <summary>
        /// Builds an array of vertices that can be written to a vertex buffer.
        /// This time we will be creating 6 vertices for a textured quad.
        /// </summary>
        /// <returns>An array of vertices.</returns>
        private static Vertex[] BuildVertexData()
        {
            Vertex[] vertexData = new Vertex[6];

            vertexData[0].Position = new Vector3(-1.0f, 1.0f, 0.0f);
            vertexData[0].Tu = 0.0f;
            vertexData[0].Tv = 0.0f;

            vertexData[1].Position = new Vector3(-1.0f, -1.0f, 0.0f);
            vertexData[1].Tu = 0.0f;
            vertexData[1].Tv = 1.0f;

            vertexData[2].Position = new Vector3(1.0f, 1.0f, 0.0f);
            vertexData[2].Tu = 1.0f;
            vertexData[2].Tv = 0.0f;

            vertexData[3].Position = new Vector3(-1.0f, -1.0f, 0.0f);
            vertexData[3].Tu = 0.0f;
            vertexData[3].Tv = 1.0f;

            vertexData[4].Position = new Vector3(1.0f, -1.0f, 0.0f);
            vertexData[4].Tu = 1.0f;
            vertexData[4].Tv = 1.0f;

            vertexData[5].Position = new Vector3(1.0f, 1.0f, 0.0f);
            vertexData[5].Tu = 1.0f;
            vertexData[5].Tv = 0.0f;

            return vertexData;
        }

        // Vertex structure.
        [StructLayout(LayoutKind.Sequential)]
        struct Vertex
        {
            public Vector3 Position;
            public float Tu;
            public float Tv;

            public static int SizeBytes
            {
                get { return Marshal.SizeOf(typeof(Vertex)); }
            }

            public static VertexFormat Format
            {
                get { return VertexFormat.Position | VertexFormat.Texture1; }
            }
        }
    }

    #endregion
}
