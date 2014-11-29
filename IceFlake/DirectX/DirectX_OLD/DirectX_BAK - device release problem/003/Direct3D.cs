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
        public static Device Device { get; private set; }
        private static readonly object _frameLock = new object();

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
                        Device = null;
                    }
                    else
                    {
                        if (Device == null)
                            Device = Device.FromPointer(device);

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

        ///// <summary>
        ///// Reset the _renderTarget so that we are sure it will have the correct presentation parameters (required to support working across changes to windowed/fullscreen or resolution changes)
        ///// </summary>
        public static int EndSceneHookReset(IntPtr devicePtr)
        {
            using (Device _device = Device.FromPointer(devicePtr))
            {
                //PresentParameters pp = new PresentParameters() { Windowed = true, BackBufferWidth = 1, BackBufferHeight = 1 };

                //PresentParameters pp = new PresentParameters() 
                //{ 
                //    Windowed = true, 
                //    BackBufferCount = 1,
                //    BackBufferWidth = 1, 
                //    BackBufferHeight = 1,
                //    BackBufferFormat = Format.X8R8G8B8,
                //    SwapEffect = SlimDX.Direct3D9.SwapEffect.Discard,
                //};
                Log.WriteLine(LogType.Critical, "XX");
                Log.WriteLine(LogType.Critical, "XXX : " + ((int)_endSceneHook.CallOriginal(_device)).ToString());

/*/
d3dpp.BackBufferWidth=800; //pixel width
d3dpp.BackBufferHeight=600; //pixel height
d3dpp.BackBufferFormat=D3DFMT_X8R8G8B8; // or any other that video card supports
d3dpp.SwapEffect=D3DSWAPEFFECT_DISCARD; //if you want to render based on monitor refresh rate then use D3DSWAPEFFECT_COPY_USYNC
d3dpp.Windowed=FALSE;
d3dpp.EnableAutoDepthStencil=TRUE;
d3dpp.AutoDepthStencilFormat=D3DFMT_D24S8; //or D3DFMT_D16
d3dpp.FullScreen_RefreshRateInHz=D3DPRESENT_RATE_DEFAULT;
d3dpp.FullScreen_PresentationInterval= D3DPRESENT_INTERVAL_IMMEDIATE; // if you want to render based on monitor refresh rate then use D3DPRESENT_INTERVAL_ONE
d3dpp.Flags=D3DPRESENTFLAG_LOCKABLE_BACKBUFFER;
/*/


                /*/
                // Create Presentparams
                PresentParameters pp = new PresentParameters();
                pp.BackBufferFormat = Format.Unknown; // or any other that video card supports
                pp.SwapEffect = SwapEffect.Discard; //if you want to render based on monitor refresh rate then use D3DSWAPEFFECT_COPY_USYNC
                pp.PresentationInterval = PresentInterval.Immediate; // if you want to render based on monitor refresh rate then use D3DPRESENT_INTERVAL_ONE
                pp.PresentFlags = PresentFlags.LockableBackBuffer;
                pp.Windowed = true; // when in fullscreen set this to false
                pp.EnableAutoDepthStencil = true;
                pp.AutoDepthStencilFormat = Format.D24S8; // D3DFMT_D24S8; //or D3DFMT_D16
                pp.PresentationInterval = PresentInterval.Immediate;
                pp.BackBufferWidth = 1; //pixel width
                pp.BackBufferHeight = 1; //pixel height
                pp.BackBufferCount = 1;
                //pp.FullScreenRefreshRateInHertz = ???
                /*/

                lock (_frameLock)
                {
                    try
                    {
                        if (_device != null)
                        {
                            //_device.Direct3D.Dispose();
                            //_device.Dispose();
                            //if (!_device.Disposed)
                            //    System.Windows.Forms.MessageBox.Show("what the fuck?¿");

                            //_device.Reset(pp);
                            //_device.Reset();

                            //_device = null;



                           // if(Manager.Memory.Detours.)

                            //Surface _RenderTarget = _device.GetRenderTarget(0);

                            //_RenderTarget.Dispose();

                            //_RenderTarget = null;

                            //_device.GetRenderTarget(0).Dispose();
                            //_device.GetRenderTarget(0) = null;



                        }
                    }
                    catch (Direct3D9Exception dex) { Log.WriteLine(LogType.Critical, dex.ToString()); }
                    catch (SlimDXException sde) { Log.WriteLine(LogType.Critical, sde.ToString()); }
                    catch (Exception ex) { Log.WriteLine(LogType.Critical, ex.ToString()); }
                    //catch (Direct3D9Exception dex) { Log.LogException(dex); }
                    //catch (SlimDXException sde) { Log.LogException(sde); }
                    //catch (Exception ex) { Log.LogException(ex); }




                    //return (int)_resetHook.CallOriginal(_device, pp);
                    return (int)_endSceneHook.CallOriginal(_device);
                }
            }
        }



        //private static int ResetHook(IntPtr device, Direct3DAPI.PresentParameters pp)
        //{
        //    Device = null;
        //    return (int) _resetHook.CallOriginal(device, pp);
        //}

        public static void Initialize()
        {
            FrameQueueFinalized = new ManualResetEventSlim(false);
            var endScenePointer = IntPtr.Zero;
            var resetPointer = IntPtr.Zero;
            var _direct3D = new SlimDX.Direct3D9.Direct3D();

            //PresentParameters pp = new PresentParameters();
            //pp.SwapEffect = SwapEffect.Discard;
            //pp.Windowed = true;
            //pp.BackBufferCount = 1;
            //pp.BackBufferWidth = 1;
            //pp.BackBufferHeight = 1;
            //PresentParameters pp = new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1 };
            //Device _device = new Device(_direct3D, 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, pp);

            // Create Presentparams
            //PresentParameters pp = new PresentParameters();
            //pp.BackBufferFormat       = Format.Unknown;
            //pp.SwapEffect             = SwapEffect.Discard;
            //pp.Windowed               = true;
            //pp.EnableAutoDepthStencil = true;
            //pp.AutoDepthStencilFormat = Format.D16;
            //pp.PresentationInterval   = PresentInterval.Immediate;
            //pp.BackBufferCount        = 1;
            //pp.BackBufferWidth        = 1; // most likely only theese needed
            //pp.BackBufferHeight       = 1; //

            /*/
            // Create Presentparams
            PresentParameters pp = new PresentParameters();
            pp.BackBufferFormat = Format.Unknown; // or any other that video card supports
            pp.SwapEffect = SwapEffect.Discard; //if you want to render based on monitor refresh rate then use D3DSWAPEFFECT_COPY_USYNC
            pp.PresentationInterval = PresentInterval.Immediate; // if you want to render based on monitor refresh rate then use D3DPRESENT_INTERVAL_ONE
            pp.PresentFlags = PresentFlags.LockableBackBuffer;
            pp.Windowed = true; // when in fullscreen set this to false
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = Format.D16; // D3DFMT_D24S8; //or D3DFMT_D16
            pp.PresentationInterval = PresentInterval.Immediate;
            pp.BackBufferWidth = 1; //pixel width
            pp.BackBufferHeight = 1; //pixel height
            pp.BackBufferCount = 1;
            //pp.FullScreenRefreshRateInHertz = ???

            Device _device = new Device(_direct3D, 0, DeviceType.NullReference, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, pp);


            /*/




            Device _device = new Device(_direct3D, 0, DeviceType.NullReference, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, new PresentParameters() { Windowed = true, BackBufferWidth = 1, BackBufferHeight = 1 });
            endScenePointer = Manager.Memory.GetObjectVtableFunction(_device.ComPointer, (uint)Direct3DAPI.VMTIndex.EndScene);
            resetPointer = Manager.Memory.GetObjectVtableFunction(_device.ComPointer, (uint)Direct3DAPI.VMTIndex.Reset);

            // 42 - EndScene (we will retrieve the back buffer here)
            _endSceneDelegate = Manager.Memory.RegisterDelegate<Direct3DAPI.Direct3D9EndScene_Delegate>(endScenePointer);
            _endSceneHook = Manager.Memory.Detours.CreateAndApply(_endSceneDelegate,
                new Direct3DAPI.Direct3D9EndScene_Delegate(EndSceneHook), "D9EndScene");




            //unsafe
            //{
            //    // If Direct3D9Ex is available - hook the PresentEx
            //    if (_supportsDirect3D9Ex)
            //    {
            //        _presentExDelegate = Manager.Memory.RegisterDelegate<Direct3DAPI.Direct3D9PresentEx_Delegate>(presentExPointer);
            //        _presentExHook = Manager.Memory.Detours.CreateAndApply(_presentExDelegate,
            //            new Direct3DAPI.Direct3D9PresentEx_Delegate(PresentExHook), "D9PresentEx");
            //    }
            //    // Always hook Present also (device will only call Present or PresentEx not both)
            //    _presentDelegate = Manager.Memory.RegisterDelegate<Direct3DAPI.Direct3D9Present_Delegate>(presentPointer);
            //    _presentHook = Manager.Memory.Detours.CreateAndApply(_presentDelegate,
            //        new Direct3DAPI.Direct3D9Present_Delegate(PresentHook), "D9Present");
            //}

            // 16 - Reset (called on resolution change or windowed/fullscreen change)
            _resetDelegate = Manager.Memory.RegisterDelegate<Direct3DAPI.Direct3D9DeviceReset_Delegate>(resetPointer);
            _resetHook = Manager.Memory.Detours.CreateAndApply(_resetDelegate,
                new Direct3DAPI.Direct3D9DeviceReset_Delegate(EndSceneHookReset), "D9Reset");




            // not needed??
            //var deviceRelease = Manager.Memory.RegisterDelegate<Direct3DAPI.D3DRelease>(Manager.Memory.GetObjectVtableFunction(_device.ComPointer, 2));
            //var release = Manager.Memory.RegisterDelegate<Direct3DAPI.D3DRelease>(Manager.Memory.GetObjectVtableFunction(_direct3D.ComPointer, 2));
            //deviceRelease(_device.ComPointer);
            //release(_direct3D.ComPointer);
            //_direct3D.Dispose();
            //_device.Dispose();

            /*/
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
            /*/






            Log.WriteLine("Direct3D9x:");
            Log.WriteLine("\tEndScene: 0x{0:X}", endScenePointer);
        }

        public static void Shutdown()
        {
            try
            {
                Log.WriteLine("[System] D3DShutdown");

                _pulsables.Clear();

                FrameCount = -1;
                FrameQueueFinalized.Wait();
                Log.WriteLine("[System] FrameQueueFinalized");

                Manager.Memory.Detours.RemoveAll();
                Manager.Memory.Patches.RemoveAll();

                Log.WriteLine("[System] Memory RemoveAll");
            }
            catch (Exception ex) { Log.LogException(ex); }
        }

        private static void PrepareRenderState()
        {
            if (Device == null || Manager.Camera == null)
                return;

            //if (Device.Disposed) // prevent or just roll with it ?
            //    return;

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