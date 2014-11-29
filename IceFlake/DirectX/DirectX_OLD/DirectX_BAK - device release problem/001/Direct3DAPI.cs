using System;
using System.Runtime.InteropServices;

namespace IceFlake.DirectX
{
    public static class Direct3DAPI
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void D3DRelease(IntPtr instance);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int Direct3D9CreateDevice(IntPtr instance, uint adapter, uint deviceType,
            IntPtr focusWindow,
            uint behaviorFlags,
            [In] ref PresentParameters presentationParameters,
            [Out] out IntPtr returnedDeviceInterface);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate int Direct3D9EndScene(IntPtr instance);

        //[UnmanagedFunctionPointer(CallingConvention.StdCall)]
        //public delegate int Direct3D9Reset(IntPtr device, PresentParameters presentationParameters);

        /// <summary>
        /// The IDirect3DDevice9.Reset function definition
        /// </summary>
        /// <param name="device"></param>
        /// <param name="presentParameters"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        //public delegate int Direct3D9Device_ResetDelegate(IntPtr device, ref PresentParameters presentParameters);
        public delegate int Direct3D9Device_ResetDelegate(IntPtr device, PresentParameters presentParameters);

        //[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        //unsafe delegate int Direct3D9Device_PresentDelegate(IntPtr devicePtr, SharpDX.Rectangle* pSourceRect, SharpDX.Rectangle* pDestRect, IntPtr hDestWindowOverride, IntPtr pDirtyRegion);
        //[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        //unsafe delegate int Direct3D9DeviceEx_PresentExDelegate(IntPtr devicePtr, SharpDX.Rectangle* pSourceRect, SharpDX.Rectangle* pDestRect, IntPtr hDestWindowOverride, IntPtr pDirtyRegion, Present dwFlags);

















        public const uint SDKVersion = 32;

        public const int EndSceneOffset = 42;
        public const int ResetOffset = 16;

        [DllImport("d3d9.dll")]
        public static extern IntPtr Direct3DCreate9(uint sdkVersion);

        [StructLayout(LayoutKind.Sequential)]
        public struct PresentParameters
        {
            public readonly uint BackBufferWidth;
            public readonly uint BackBufferHeight;
            public uint BackBufferFormat;
            public readonly uint BackBufferCount;
            public readonly uint MultiSampleType;
            public readonly uint MultiSampleQuality;
            public uint SwapEffect;
            public readonly IntPtr hDeviceWindow;
            [MarshalAs(UnmanagedType.Bool)] public bool Windowed;
            [MarshalAs(UnmanagedType.Bool)] public readonly bool EnableAutoDepthStencil;
            public readonly uint AutoDepthStencilFormat;
            public readonly uint Flags;
            public readonly uint FullScreen_RefreshRateInHz;
            public readonly uint PresentationInterval;
        }
    }
}