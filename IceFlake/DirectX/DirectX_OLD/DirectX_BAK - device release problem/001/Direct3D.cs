using SlimDX;
using SlimDX.Direct3D9;

using System;
using System.Collections.Generic;
using System.Threading;
using GreyMagic.Internals;

namespace IceFlake.DirectX
{
    public static class Direct3D
    {
        private const int VMT_ENDSCENE = 42;
        private const int VMT_RESET = 16;

        private static Direct3DAPI.Direct3D9EndScene _endSceneDelegate;
        private static Detour _endSceneHook;
        //private static Direct3DAPI.Direct3D9Reset _resetDelegate;
        //private static Detour _resetHook;

        private static Direct3DAPI.Direct3D9Device_ResetDelegate _resetDelegate;
        private static Detour _resetHook;



        private static ManualResetEventSlim FrameQueueFinalized;
        public static int FrameCount { get; private set; }
        public static event EventHandler OnFirstFrame;
        public static event EventHandler OnLastFrame = (sender, e) => FrameQueueFinalized.Set();
        public static Device Device { get; private set; }

        private static int EndSceneHook(IntPtr device)
        {
            try
            {
                if (FrameCount == -1)
                {
                    Log.WriteLine("[D] OnLastFrame");
                    if (OnLastFrame != null)
                        OnLastFrame(null, new EventArgs());
                    Device = null;
                }
                else
                {
                    if (Device == null)
                        Device = Device.FromPointer(device);

                    if (FrameCount == 0)
                        if (OnFirstFrame != null)
                            OnFirstFrame(null, new EventArgs());

                    PrepareRenderState();

                    foreach (IPulsable pulsable in _pulsables)
                        pulsable.Direct3D_EndScene();
                }
            }
            catch (Exception e) 
            { 
                Log.LogException(e); 
            }

            if (FrameCount != -1)
                FrameCount += 1;

            return (int)_endSceneHook.CallOriginal(device);
        }

        //private static int ResetHook(IntPtr device, Direct3DAPI.PresentParameters pp)
        //{
        //    Device = null;
        //    return (int) _resetHook.CallOriginal(device, pp);
        //}

        /// <summary>
        /// Reset the _renderTarget so that we are sure it will have the correct presentation parameters (required to support working across changes to windowed/fullscreen or resolution changes)
        /// </summary>
        /// <param name="devicePtr"></param>
        /// <param name="presentParameters"></param>
        /// <returns></returns>
        //private static int ResetHook(IntPtr device, Direct3DAPI.PresentParameters pp)
        private static int ResetHook(IntPtr device, Direct3DAPI.PresentParameters presentParameters)
        {
            Log.WriteLine(LogType.Critical, "ResetHook Event :");

            //Device = null;
            //Device = Device.FromPointer(device);

            Device _device = Device.FromPointer(device);

            try
            {
                if (_device != null)
                {
                    _device.Dispose();
                    //_device = null;

                    //_device.Reset(); 
                }


                Log.WriteLine(LogType.Good, SlimDX.Result.Last.Code.ToString());
                return SlimDX.Result.Last.Code;
            }
            catch (SlimDX.SlimDXException sde)
            {
                Log.WriteLine(LogType.Error, sde.ResultCode.Code.ToString());
                Log.WriteLine(LogType.Critical, sde.ToString());
                return sde.ResultCode.Code;
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogType.Error, SlimDX.Result.Last.Code.ToString());
                Log.WriteLine(LogType.Critical, ex.ToString());
                return SlimDX.Result.Last.Code;
            }
        }

        public static void Initialize()
        {
            FrameQueueFinalized = new ManualResetEventSlim(false);

            var endScenePointer = IntPtr.Zero;
            var resetPointer = IntPtr.Zero;
            using (var d3d = new SlimDX.Direct3D9.Direct3D())
            {
                using (var tmpDevice = new Device(d3d, 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1 }))
                {
                    endScenePointer = Manager.Memory.GetObjectVtableFunction(tmpDevice.ComPointer, VMT_ENDSCENE);
                    resetPointer = Manager.Memory.GetObjectVtableFunction(tmpDevice.ComPointer, VMT_RESET);
                }
            }

            _endSceneDelegate = Manager.Memory.RegisterDelegate<Direct3DAPI.Direct3D9EndScene>(endScenePointer);
            _endSceneHook = Manager.Memory.Detours.CreateAndApply(_endSceneDelegate, 
                new Direct3DAPI.Direct3D9EndScene(EndSceneHook), "D9EndScene");


            // 16 - Reset (called on resolution change or windowed/fullscreen change - we will reset some things as well)
            _resetDelegate = Manager.Memory.RegisterDelegate<Direct3DAPI.Direct3D9Device_ResetDelegate>(resetPointer);
            _resetHook = Manager.Memory.Detours.CreateAndApply(_resetDelegate,
                // On Windows 7 64-bit w/ 32-bit app and d3d9 dll version 6.1.7600.16385, the address is equiv to:
                //(IntPtr)(GetModuleHandle("d3d9").ToInt32() + 0x58dda),
                // A 64-bit app would use 0x3b3a0
                // Note: GetD3D9DeviceFunctionAddress will output these addresses to a log file
                new Direct3DAPI.Direct3D9Device_ResetDelegate(ResetHook), "D9Reset");

            Log.WriteLine("Direct3D9x:");
            Log.WriteLine("\tEndScene: 0x{0:X}", endScenePointer);
        }

        public static void Shutdown()
        {
            Log.WriteLine("[D] D3DShutdown");
            _pulsables.Clear();

            FrameCount = -1;
            FrameQueueFinalized.Wait();

            Manager.Memory.Detours.RemoveAll();
            Manager.Memory.Patches.RemoveAll();
        }

        private static void PrepareRenderState()
        {
            if (Device == null || Manager.Camera == null)
                return;

            var viewport = Device.Viewport;
            viewport.MinZ = 0.0f;
            viewport.MaxZ = 0.94f;
            Device.Viewport = viewport;

            Device.SetTransform(TransformState.View, Manager.Camera.View);
            Device.SetTransform(TransformState.Projection, Manager.Camera.Projection);

            Device.VertexShader = null;
            Device.PixelShader = null;
            Device.SetRenderState(RenderState.ZEnable, true);
            Device.SetRenderState(RenderState.ZWriteEnable, true);
            Device.SetRenderState(RenderState.ZFunc, Compare.LessEqual);
            Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            Device.SetRenderState(RenderState.Lighting, false);
            Device.SetTexture(0, null);
            Device.SetRenderState(RenderState.CullMode, Cull.None);
        }

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
    }
}