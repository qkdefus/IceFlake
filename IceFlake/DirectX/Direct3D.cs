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
        private static Detour _endSceneHook;
        private static Detour _resetHook;
        private static Direct3DAPI.Direct3D9EndScene _endSceneDelegate;
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

        private static int ResetHook(IntPtr device, Direct3DAPI.PresentParameters pp)
        {
            Device = null;
            return (int) _resetHook.CallOriginal(device, pp);
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
            _endSceneHook = Manager.Memory.Detours.CreateAndApply(_endSceneDelegate, new Direct3DAPI.Direct3D9EndScene(EndSceneHook), "D9EndScene");

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