using SlimDX;
using SlimDX.Direct3D9;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using GreyMagic.Internals;

namespace IceFlake.DirectX
{
    public static class Direct3D
    {
        private static Direct3DAPI.Direct3D9EndScene_Delegate _endSceneDelegate;
        private static Detour _endSceneHook;

        private static Direct3DAPI.Direct3D9DeviceReset_Delegate _resetDelegate;
        private static Detour _resetHook;

        //private static Direct3DAPI.Direct3D9Present_Delegate _presentDelegate;
        //private static Detour _resetHook;

        //private static Direct3DAPI.Direct3D9PresentEx_Delegate _presentExDelegate;
        //private static Detour _resetHook;

        private static ManualResetEventSlim FrameQueueFinalized;
        public static int FrameCount { get; private set; }
        public static event EventHandler OnFirstFrame;
        public static event EventHandler OnLastFrame = (sender, e) => FrameQueueFinalized.Set();
        public static Device CurrDevice { get; private set; }
        //public static Device TempDevice { get; private set; }

        private static readonly object _frameLock = new object();

        // EndSceneHook
        ///////////////////////////////////////////////////////////////

        private static int EndSceneHook(IntPtr device)
        {
            lock (_frameLock)
            {
                try
                {
                    if (FrameCount == -1)
                    {
                        Log.WriteLine("[System] OnLastFrame");
                        if (OnLastFrame != null)
                            OnLastFrame(null, new EventArgs());
                        CurrDevice = null;
                    }
                    else
                    {
                        if (CurrDevice == null)
                            CurrDevice = Device.FromPointer(device);

                        if (FrameCount == 0)
                            if (OnFirstFrame != null)
                                OnFirstFrame(null, new EventArgs());

                        // Prepare Rendering
                        PrepareRenderState();

                        foreach (IPulsable pulsable in _pulsables)
                            pulsable.Direct3D_EndScene();
                    }
                }
                catch (Exception ex) { Log.LogException(ex); }

                if (FrameCount != -1)
                    FrameCount += 1;
            }

            return (int)_endSceneHook.CallOriginal(device);
        }

        // ResetHook
        ///////////////////////////////////////////////////////////////

        private static int ResetHook(IntPtr device, ref PresentParameters pp)
        {
            CurrDevice = null;
            return (int) _resetHook.CallOriginal(device, pp);
        }

        // Initialize
        ///////////////////////////////////////////////////////////////

        public static void Initialize()
        {
            FrameQueueFinalized = new ManualResetEventSlim(false);
            var endScenePointer = IntPtr.Zero;
            var resetPointer = IntPtr.Zero;

            using (var _direct3D = new SlimDX.Direct3D9.Direct3D())
            {
                using (var _device = new Device(_direct3D, 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1 }))
                {
                    endScenePointer = Manager.Memory.GetObjectVtableFunction(_device.ComPointer, (uint)Direct3DAPI.VMTIndex.EndScene);
                    resetPointer = Manager.Memory.GetObjectVtableFunction(_device.ComPointer, (uint)Direct3DAPI.VMTIndex.Reset);
                }
            }

            //var _direct3D = new SlimDX.Direct3D9.Direct3D();
            //PresentParameters pp = new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1 };
            //Device _device = new Device(_direct3D, 0, DeviceType.NullReference, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, pp);
            //endScenePointer = Manager.Memory.GetObjectVtableFunction(_device.ComPointer, (uint)Direct3DAPI.VMTIndex.EndScene);
            //resetPointer = Manager.Memory.GetObjectVtableFunction(_device.ComPointer, (uint)Direct3DAPI.VMTIndex.Reset);

            // 42 - EndScene (we will retrieve the back buffer here)
            _endSceneDelegate = Manager.Memory.RegisterDelegate<Direct3DAPI.Direct3D9EndScene_Delegate>(endScenePointer);
            _endSceneHook = Manager.Memory.Detours.CreateAndApply(_endSceneDelegate,
                new Direct3DAPI.Direct3D9EndScene_Delegate(EndSceneHook), "D9EndScene");

            // 16 - Reset (called on resolution change or windowed/fullscreen change)
            _resetDelegate = Manager.Memory.RegisterDelegate<Direct3DAPI.Direct3D9DeviceReset_Delegate>(resetPointer);
            _resetHook = Manager.Memory.Detours.CreateAndApply(_resetDelegate,
                new Direct3DAPI.Direct3D9DeviceReset_Delegate(ResetHook), "D9Reset");

            //_direct3D.Dispose();
            //_device.Dispose();

            Log.WriteLine("Direct3D9x:");
            Log.WriteLine("\tEndScene: {0}", Manager.ConvertToHexString((uint)endScenePointer));
        }

        // Shutdown
        ///////////////////////////////////////////////////////////////

        public static void Shutdown()
        {
            try
            {
                Log.WriteLine("[System] Shutdown : Start");

                _pulsables.Clear();

                FrameCount = -1;
                FrameQueueFinalized.Wait();

                Manager.Memory.Detours.RemoveAll();
                Manager.Memory.Patches.RemoveAll();;

                Log.WriteLine("[System] Shutdown : Complete");

            }
            catch (Exception ex) { Log.LogException(ex); }
        }

        // Prepare Rendering
        ///////////////////////////////////////////////////////////////

        private static void PrepareRenderState()
        {
            if (CurrDevice == null || Manager.Camera == null)
                return;

            //if (Device.Disposed) // prevent or just roll with it ?
            //    return;

            var viewport = CurrDevice.Viewport;
            viewport.MinZ = 0.0f;
            viewport.MaxZ = 0.94f;
            CurrDevice.Viewport = viewport;

            CurrDevice.SetTransform(TransformState.View, Manager.Camera.View);
            CurrDevice.SetTransform(TransformState.Projection, Manager.Camera.Projection);

            CurrDevice.VertexShader = null;
            CurrDevice.PixelShader = null;
            CurrDevice.SetRenderState(RenderState.ZEnable, true);
            CurrDevice.SetRenderState(RenderState.ZWriteEnable, true);
            CurrDevice.SetRenderState(RenderState.ZFunc, Compare.LessEqual);
            CurrDevice.SetRenderState(RenderState.AlphaBlendEnable, true);
            CurrDevice.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

            CurrDevice.SetRenderState(RenderState.Lighting, false);

            CurrDevice.SetTexture(0, null);
            CurrDevice.SetRenderState(RenderState.CullMode, Cull.None);

        }

        // Pulsables
        ///////////////////////////////////////////////////////////////

        private static readonly LinkedList<IPulsable> _pulsables = new LinkedList<IPulsable>();

        public static void RegisterCallback(IPulsable pulsable)
        {
            _pulsables.AddLast(pulsable);
        }

        public static void RegisterCallbacks(params IPulsable[] pulsables)
        {
            foreach (IPulsable pulsable in pulsables)
                RegisterCallback(pulsable);
        }

        public static void RemoveCallback(IPulsable pulsable)
        {
            if (_pulsables.Contains(pulsable))
                _pulsables.Remove(pulsable);
        }


        // End
        ///////////////////////////////////////////////////////////////

        // Vertex structure.
        [StructLayout(LayoutKind.Sequential)]
        public struct Vertex
        {
            public SlimDX.Vector3 Position;
            public SlimDX.Vector3 Normal;

            public static int SizeBytes
            {
                get { return Marshal.SizeOf(typeof(Vertex)); }
            }

            public static VertexFormat Format
            {
                get { return VertexFormat.Position | VertexFormat.Normal; }
            }
        }

        /// <summary>
        /// Builds an array of vertices that can be written to a vertex buffer.
        /// </summary>
        /// <returns>An array of vertices.</returns>
        public static Vertex[] BuildVertexData()
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





        // End
        ///////////////////////////////////////////////////////////////
    }
}