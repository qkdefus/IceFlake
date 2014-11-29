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
    #region QKDrawScriptV3

    public class QKDrawScriptV3 : Script
    {
        public QKDrawScriptV3() 
            : base("_QKDrawScriptV3", "Drawing") 
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

            Run();
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




        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>
        /// 

        System.Windows.Forms.Panel slimPanel = new System.Windows.Forms.Panel();

        public void Run()
        {
            //InitializeComponent();

            CreateDevice();
            BuildWindows();
        }

        void BuildWindows()
        {
            //var listBox = new System.Windows.Forms.ListBox();
            //listBox.Items = Enumerable.Range(0, 100);
            //var elementHost = new ElementHost();
            //elementHost.Child = listBox;

            //elementHost.Dock = DockStyle.Fill;
            //Controls.Add(elementHost);
            //slimPanel.Dock = DockStyle.Left;
            //Controls.Add(slimPanel);
        }

        Direct3D Direct3D = new Direct3D();
        void CreateDevice()
        {
            PresentParameters presentParams = new PresentParameters();
            presentParams.BackBufferHeight = slimPanel.ClientRectangle.Height;
            presentParams.BackBufferWidth = slimPanel.ClientRectangle.Width;
            presentParams.DeviceWindowHandle = slimPanel.Handle;

            var device = new Device(Direct3D, 0, DeviceType.Hardware, slimPanel.Handle, CreateFlags.HardwareVertexProcessing, presentParams);

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
    }

    #endregion
}
