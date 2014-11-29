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

        private static Direct3DAPI.Direct3D9EndScene_PresentDelegate _endSceneDelegate;
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
        private static readonly object _frameLock = new object();





        //public static bool IsInitialized
        //{
        //    get { return Device != null; }
        //}

        //private static IntPtr _usedDevicePointer = IntPtr.Zero;

        //private static void Init(IntPtr devicePointer)
        //{
        //    if (_usedDevicePointer != devicePointer)
        //    {
        //        Device = Device.FromPointer(devicePointer);
        //        _usedDevicePointer = devicePointer;
        //        Log.WriteLine(LogType.Critical, "Rendering: Device initialized on " + devicePointer);
        //    }
        //}

        private static int EndSceneHook(IntPtr device)
        {
            lock (_frameLock)
            {
                //if (!IsInitialized)
                //{
                //    Init(device);
                //}

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
            }

            return (int)_endSceneHook.CallOriginal(device);
        }


        public static Device tmpDevice;

        public static int EndSceneHookReset(IntPtr device, PresentParameters presentParams)
        {
            lock (_frameLock)
            {
                //if (!IsInitialized)
                //{
                //    Init(Device);
                //}

                try
                {

                    //_SB.Capture();
                    Device = Device.FromPointer(device);
                    //Device _dev = dev

                    Device.Dispose();
                    Device = null;

                    //_SB.Apply();

                    //_SB.Dispose();




                    //IceFlake.DirectX.Direct3D.Device.Dispose();
                }
                catch (Direct3D9Exception dex)
                {
                    Log.WriteLine(LogType.Critical, dex.ToString());
                }
                catch (SlimDXException sde)
                {
                    Log.WriteLine(LogType.Critical, sde.ToString());
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogType.Error, ex.ToString());
                }

                //return (int)_endSceneHook.CallOriginal(device);

                return (int)_endSceneHook.CallOriginal(device);
            }
        }

        //private static int ResetHook(IntPtr device, Direct3DAPI.PresentParameters pp)
        //{
        //    Device = null;
        //    return (int) _resetHook.CallOriginal(device, pp);
        //}

        /// <summary>
        /// Reset the _renderTarget so that we are sure it will have the correct presentation parameters (required to support working across changes to windowed/fullscreen or resolution changes)
        /// </summary>
        private static int ResetHook(IntPtr device, Direct3DAPI.PresentParameters presentParameters)
        {
            //Log.WriteLine(LogType.Critical, "ResetHook Event :");

            //Device = null;
            Device _device = Device.FromPointer(device);

            try
            {
                if (_device != null)
                {
                    //_device = null;
                    //_device.Reset(); 
                    _device.Dispose();
                }
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

        public static PresentParameters presParams = new PresentParameters(){ BackBufferWidth = 1, BackBufferHeight = 1 };

        public static void Initialize()
        {
            //FrameQueueFinalized = new ManualResetEventSlim(false);

            var endScenePointer = IntPtr.Zero;
            var resetPointer = IntPtr.Zero;


            //var _tmpD3D = new SlimDX.Direct3D9.Direct3D();
            //PresentParameters parameters = new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1 };
            //Device _tmpDevice = new Device(_tmpD3D, 0, DeviceType.NullReference, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, new PresentParameters[] { parameters });

            //endScenePointer = Manager.Memory.GetObjectVtableFunction(_tmpDevice.ComPointer, VMT_ENDSCENE);
            //resetPointer = Manager.Memory.GetObjectVtableFunction(_tmpDevice.ComPointer, VMT_RESET);



            ////using (var d3d = new SlimDX.Direct3D9.Direct3D())
            ////{
            ////    //using (var tmpDevice = new Device(d3d, 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1 }))
            ////    //using (var tmpDevice = new Device(d3d, 0, DeviceType.NullReference, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1 }))
            ////    using (var tmpDevice = new Device(d3d, 0, DeviceType.NullReference, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, presParams))
            ////    {
            ////        endScenePointer = Manager.Memory.GetObjectVtableFunction(tmpDevice.ComPointer, VMT_ENDSCENE);
            ////        resetPointer = Manager.Memory.GetObjectVtableFunction(tmpDevice.ComPointer, VMT_RESET);
            ////    }
            ////}

            //// 42 - EndScene (we will retrieve the back buffer here)
            //_endSceneDelegate = Manager.Memory.RegisterDelegate<Direct3DAPI.Direct3D9EndScene_PresentDelegate>(endScenePointer);
            //_endSceneHook = Manager.Memory.Detours.CreateAndApply(_endSceneDelegate,
            //    new Direct3DAPI.Direct3D9EndScene_PresentDelegate(EndSceneHook), "D9EndScene");

            ////unsafe
            ////{
            ////    // If Direct3D9Ex is available - hook the PresentEx
            ////    if (_supportsDirect3D9Ex)
            ////    {
            ////        _presentExDelegate = Manager.Memory.RegisterDelegate<Direct3DAPI.Direct3D9EndScene_PresentDelegate>(presentExPointer);
            ////        _presentExHook = Manager.Memory.Detours.CreateAndApply(_presentExDelegate,
            ////            new Direct3DAPI.Direct3D9EndScene_PresentDelegate(PresentExHook), "D9PresentEx");
            ////    }
            ////    // Always hook Present also (device will only call Present or PresentEx not both)
            ////    _presentDelegate = Manager.Memory.RegisterDelegate<Direct3DAPI.Direct3D9EndScene_PresentDelegate>(presentPointer);
            ////    _presentHook = Manager.Memory.Detours.CreateAndApply(_presentDelegate,
            ////        new Direct3DAPI.Direct3D9EndScene_PresentDelegate(PresentHook), "D9Present");
            ////}

            //// 16 - Reset (called on resolution change or windowed/fullscreen change)
            //_resetDelegate = Manager.Memory.RegisterDelegate<Direct3DAPI.Direct3D9Device_ResetDelegate>(resetPointer);
            //_resetHook = Manager.Memory.Detours.CreateAndApply(_resetDelegate,
            //    //new Direct3DAPI.Direct3D9Device_ResetDelegate(ResetHook), "D9Reset");
            //    new Direct3DAPI.Direct3D9Device_ResetDelegate(EndSceneHookReset, new PresentParameters()), "D9Reset");










            using (var window = new System.Windows.Forms.Form())
            {
                IntPtr direct3D = Direct3DAPI.Direct3DCreate9(Direct3DAPI.SDKVersion);
                if (direct3D == IntPtr.Zero)
                {
                    System.Windows.Forms.MessageBox.Show("Direct3DCreate9 failed (SDK Version: " + Direct3DAPI.SDKVersion + ")");
                    throw new Exception("Direct3DCreate9 failed (SDK Version: " + Direct3DAPI.SDKVersion + ")");
                }
                var pp = new Direct3DAPI.PresentParameters { Windowed = true, SwapEffect = 1, BackBufferFormat = 0 };

                var createDevice = Manager.Memory.RegisterDelegate<Direct3DAPI.Direct3D9CreateDevice>(Manager.Memory.GetObjectVtableFunction(direct3D, 16));
                IntPtr device;

                //tmpDevice = new Device(d3d, 0, DeviceType.NullReference, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1 }))
                //if (createDevice(direct3D, 0, 1, IntPtr.Zero, 0x20, ref pp, out device) < 0)
                if (createDevice(direct3D, 0, 1, IntPtr.Zero, 0x20, ref pp, out device) < 0)
                {
                    System.Windows.Forms.MessageBox.Show("Failed to create device");
                    throw new Exception("Failed to create device");
                }
                endScenePointer = Manager.Memory.GetObjectVtableFunction(device, Direct3DAPI.EndSceneOffset);
                resetPointer = Manager.Memory.GetObjectVtableFunction(device, Direct3DAPI.ResetOffset);

                _endSceneDelegate = Manager.Memory.RegisterDelegate<Direct3DAPI.Direct3D9EndScene_PresentDelegate>(endScenePointer);
                var deviceRelease = Manager.Memory.RegisterDelegate<Direct3DAPI.D3DRelease>(Manager.Memory.GetObjectVtableFunction(device, 2));
                var release = Manager.Memory.RegisterDelegate<Direct3DAPI.D3DRelease>(Manager.Memory.GetObjectVtableFunction(direct3D, 2));

                deviceRelease(device);
                release(direct3D);
            }

            FrameQueueFinalized = new ManualResetEventSlim(false);
            _endSceneHook = Manager.Memory.Detours.CreateAndApply(_endSceneDelegate, new Direct3DAPI.Direct3D9EndScene_PresentDelegate(EndSceneHook), "EndScene");
            Log.WriteLine("EndScene detoured");







            Log.WriteLine("Direct3D9x:");
            Log.WriteLine("\tEndScene: 0x{0:X}", endScenePointer);
            //Log.WriteLine("\tEndScene: {0:X}", Manager.ConvertToHexString((uint)endScenePointer));
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