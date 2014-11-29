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

    public class QKDrawScriptV6 : Script
    {
        public QKDrawScriptV6()
            : base("_QKDrawScriptV6", "Drawing")
        { }

        Color _colorLight = Color.Green;

        Color _ambient = Color.Blue;
        SlimDX.Direct3D9.Light _light;
        SlimDX.Direct3D9.Mesh _mesh;
        SlimDX.Direct3D9.Material activeMaterial, passiveMaterial, groundMaterial, _material;
        SlimDX.Direct3D9.StateBlock _SB = new SlimDX.Direct3D9.StateBlock(D3D.CurrDevice, StateBlockType.All);

        // OnStart
        ///////////////////////////////////////////////////////////////

        public override void OnStart()
        {
            if (!Manager.ObjectManager.IsInGame)
            {
                Stop();
                return;
            }
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

        // Initialize
        ///////////////////////////////////////////////////////////////

        private void Initialize()
        {
            //activeMaterial = new Material();
            //activeMaterial.Diffuse = Color.Orange;
            //activeMaterial.Ambient = _ambient;

            //passiveMaterial = new Material();
            //passiveMaterial.Diffuse = Color.Red;
            //passiveMaterial.Ambient = _ambient;

            groundMaterial = new Material();
            groundMaterial.Diffuse = Color.Green;
            groundMaterial.Ambient = _ambient;
        }

        // DrawCylinderTest
        ///////////////////////////////////////////////////////////////

        private void DrawCylinderTest(SlimDX.Vector3 loc, float width, float height, float depth, bool isFilled = true, bool wireframe = false)
        {
            if (_mesh == null)
                _mesh = SlimDX.Direct3D9.Mesh.CreateCylinder(D3D.CurrDevice, 0.1f, 0.5f, 0.5f, 15, 1);


            _SB.Capture();

            //_light = new Light();
            //_light.Type = LightType.Directional;
            //_light.Range = 100;
            //_light.Position = new Vector3(10, 25, 10);
            //_light.Diffuse = Color.Red;
            //_light.Attenuation0 = 1.0f;

            // Setup Lights
            //_light = new Light();
            //_light.Type = LightType.Spot;
            //_light.Diffuse = _colorLight;
            //_light.Attenuation0 = 0.2f;
            //_light.Range = 50f;
            //_light.Direction = new SlimDX.Vector3(0.0f, 0.0f, 0.0f);
            //_light.Direction.Normalize();

            //// Setup Material
            //_material.Diffuse = System.Drawing.Color.Blue;
            //_material.Ambient = _ambient;
            //D3D.CurrDevice.Material = _material;

            // Set the light and enable lighting.
            //D3D.CurrDevice.SetLight(0, _light);
            //D3D.CurrDevice.EnableLight(0, true);
            D3D.CurrDevice.SetRenderState(RenderState.Lighting, true);

            // Set an ambient light.
            D3D.CurrDevice.SetRenderState(RenderState.Ambient, _ambient.ToArgb());
            //D3D.CurrDevice.SetTexture(0, null);













            var worldMatrix = SlimDX.Matrix.Translation(loc);
            D3D.CurrDevice.SetTransform(TransformState.World, worldMatrix);



            _mesh.DrawSubset(0);

            D3D.CurrDevice.SetRenderState(RenderState.FillMode, SlimDX.Direct3D9.FillMode.Solid);

            _SB.Apply();


        }

        // End
        ///////////////////////////////////////////////////////////////
    }

    #endregion
}
