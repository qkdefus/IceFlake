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

using IceFlake.Client;

using Squid;

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
    #region DrawMeshScript

    public class QKDrawScriptV7 : Script
    {
        public QKDrawScriptV7()
            : base("_QKDrawScriptV7", "Drawing")
        { }

        Color _colorLight = Color.Green;

        Color _ambient = Color.Blue;
        SlimDX.Direct3D9.Light _light;
        SlimDX.Direct3D9.Mesh _mesh;
        SlimDX.Direct3D9.Material activeMaterial, passiveMaterial, groundMaterial, _material;
        SlimDX.Direct3D9.StateBlock _SB = new SlimDX.Direct3D9.StateBlock(D3D.CurrDevice, StateBlockType.All);


        SlimDX.Direct3D9.Material[] meshMaterials; // Materials for the mesh
        Texture[] meshTextures;            // Textures for the mesh

        //public static VertexBuffer Vertices;    // Vertex buffer object used to hold vertices.

        private VertexBuffer vertexBuffer = null;
        Texture texture = null;

        // OnStart
        ///////////////////////////////////////////////////////////////

        public override void OnStart()
        {
            if (!Manager.ObjectManager.IsInGame)
            {
                Stop();
                return;
            }

            LoadTexture();
        }

        // OnTick
        ///////////////////////////////////////////////////////////////

        public override void OnTick()
        {
            var _pos = Manager.LocalPlayer.Location;

            DrawCylinderTest(new SlimDX.Vector3(_pos.X, _pos.Y, _pos.Z), 0.5f, 0.5f, 0.5f, true, false);
        }

        // OnTerminate
        ///////////////////////////////////////////////////////////////

        public override void OnTerminate()
        {
            if (!Manager.ObjectManager.IsInGame)
            {
                Stop();
                return;
            }

            // reset
            D3D.CurrDevice.SetRenderState(RenderState.FillMode, SlimDX.Direct3D9.FillMode.Solid);
        }

        // LoadTexture
        ///////////////////////////////////////////////////////////////



        private void LoadTexture()
        {
            try
            {
                string _file = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Textures\\test.bmp";

                // Create the vertex buffer and fill with the triangle vertices. (Non-indexed)
                // Remember 3 vetices for a triangle, 2 tris per quad = 6.
                vertexBuffer = new VertexBuffer(D3D.CurrDevice, 6 * Vertex.SizeBytes, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                DataStream stream = vertexBuffer.Lock(0, 0, LockFlags.None);
                stream.WriteRange(BuildVertexData());
                vertexBuffer.Unlock();

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


        // DrawCylinderTest
        ///////////////////////////////////////////////////////////////






        private void DrawCylinderTest(SlimDX.Vector3 loc, float width, float height, float depth, bool isFilled = true, bool wireframe = false)
        {
            if (_mesh == null)
                _mesh = SlimDX.Direct3D9.Mesh.CreateBox(D3D.CurrDevice, 2f, 2f, 2f);

            loc.Z = loc.Z + 3;

            _SB.Capture();

            //_light = new Light();
            //_light.Type = LightType.Directional;
            //_light.Range = 100;
            //_light.Position = new Vector3(10, 25, 10);
            //_light.Diffuse = Color.Red;
            //_light.Attenuation0 = 1.0f;

            // Setup Lights
            _light = new Light();
            _light.Type = LightType.Spot;
            _light.Diffuse = _colorLight;
            _light.Attenuation0 = 0.2f;
            _light.Range = 50f;
            _light.Direction = new SlimDX.Vector3(0.0f, 0.0f, 0.0f);
            _light.Direction.Normalize();

            //// Setup Material
            //_material.Diffuse = System.Drawing.Color.Blue;
            //_material.Ambient = _ambient;

            //D3D.CurrDevice.Material = _material;

            // Set the light and enable lighting.
            D3D.CurrDevice.SetLight(0, _light);
            D3D.CurrDevice.EnableLight(0, true);
            D3D.CurrDevice.SetRenderState(RenderState.Lighting, true);








            Surface surface = texture.GetSurfaceLevel(0);
            Surface buffer = D3D.CurrDevice.GetRenderTarget(0);

            D3D.CurrDevice.SetRenderTarget(0, surface);


            D3D.CurrDevice.SetStreamSource(0, vertexBuffer, 0, Marshal.SizeOf(typeof(SlimDX.SampleFramework.TexturedVertex)));
            D3D.CurrDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 3);


















            // Set an ambient light.
            //D3D.CurrDevice.SetRenderState(RenderState.Ambient, Color.Orange.ToArgb());
            D3D.CurrDevice.SetRenderState(RenderState.Ambient, Color.Lime.ToArgb());

            D3D.CurrDevice.SetTexture(0, texture);



            // transform view matrix
            var worldMatrix = SlimDX.Matrix.Translation(loc);
            D3D.CurrDevice.SetTransform(TransformState.World, worldMatrix);

            // set renderstate fillmode
            D3D.CurrDevice.SetRenderState(RenderState.FillMode, SlimDX.Direct3D9.FillMode.Solid);

            // draw the mesh
            _mesh.DrawSubset(0);


            for (int i = 0; i < meshMaterials.Length; i++)
            {
                // Set the material and texture for this subset.
                D3D.CurrDevice.Material = meshMaterials[i];
                D3D.CurrDevice.SetTexture(0, meshTextures[i]);

                // Draw the mesh subset.
                _mesh.DrawSubset(i);
            }






            // fillmode solid

            _SB.Apply();


        }

        // End
        ///////////////////////////////////////////////////////////////
    }

    #endregion
}
