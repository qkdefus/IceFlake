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
            [In] ref Direct3DAPI.PresentParameters presentationParameters,
            [Out] out IntPtr returnedDeviceInterface);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate int Direct3D9EndScene_Delegate(IntPtr instance);



        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate int Direct3D9PresentEx_Delegate(IntPtr instance);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate int Direct3D9Present_Delegate(IntPtr instance);

        //[UnmanagedFunctionPointer(CallingConvention.StdCall)]
        //public delegate int Direct3D9DeviceReset_Delegate(IntPtr device);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int Direct3D9DeviceReset_Delegate(IntPtr device, ref SlimDX.Direct3D9.PresentParameters presentParameters);

        //[UnmanagedFunctionPointer(CallingConvention.StdCall)]
        //public delegate int Direct3D9Reset(IntPtr device, PresentParameters presentationParameters);

        /// <summary>
        /// The IDirect3DDevice9.Reset function definition
        /// </summary>
        /// <param name="device"></param>
        /// <param name="presentParameters"></param>
        /// <returns></returns>
        //[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        ////public delegate int Direct3D9Device_ResetDelegate(IntPtr device, ref PresentParameters presentParameters);
        //public delegate int Direct3D9Device_ResetDelegate(IntPtr device, PresentParameters presentParameters);

        //[UnmanagedFunctionPointer(CallingConvention.StdCall)]
        ////public delegate int Direct3D9Device_ResetDelegate(IntPtr device, PresentParameters presentParams);
        //public delegate int Direct3D9DeviceReset_Delegate(IntPtr device);







        //[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        //unsafe delegate int Direct3D9Device_PresentDelegate(IntPtr devicePtr, SharpDX.Rectangle* pSourceRect, SharpDX.Rectangle* pDestRect, IntPtr hDestWindowOverride, IntPtr pDirtyRegion);
        //[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        //unsafe delegate int Direct3D9DeviceEx_PresentExDelegate(IntPtr devicePtr, SharpDX.Rectangle* pSourceRect, SharpDX.Rectangle* pDestRect, IntPtr hDestWindowOverride, IntPtr pDirtyRegion, Present dwFlags);













        #region VMTIndex

        internal enum VMTIndex : uint
        {
            QueryInterface = 0,
            AddRef = 1,
            Release = 2,
            TestCooperativeLevel = 3,
            GetAvailableTextureMem = 4,
            EvictManagedResources = 5,
            GetDirect3D = 6,
            GetDeviceCaps = 7,
            GetDisplayMode = 8,
            GetCreationParameters = 9,
            SetCursorProperties = 10,
            SetCursorPosition = 11,
            ShowCursor = 12,
            CreateAdditionalSwapChain = 13,
            GetSwapChain = 14,
            GetNumberOfSwapChains = 15,
            Reset = 16,
            Present = 17,
            GetBackBuffer = 18,
            GetRasterStatus = 19,
            SetDialogBoxMode = 20,
            SetGammaRamp = 21,
            GetGammaRamp = 22,
            CreateTexture = 23,
            CreateVolumeTexture = 24,
            CreateCubeTexture = 25,
            CreateVertexBuffer = 26,
            CreateIndexBuffer = 27,
            CreateRenderTarget = 28,
            CreateDepthStencilSurface = 29,
            UpdateSurface = 30,
            UpdateTexture = 31,
            GetRenderTargetData = 32,
            GetFrontBufferData = 33,
            StretchRect = 34,
            ColorFill = 35,
            CreateOffscreenPlainSurface = 36,
            SetRenderTarget = 37,
            GetRenderTarget = 38,
            SetDepthStencilSurface = 39,
            GetDepthStencilSurface = 40,
            BeginScene = 41,
            EndScene = 42,
            Clear = 43,
            SetTransform = 44,
            GetTransform = 45,
            MultiplyTransform = 46,
            SetViewport = 47,
            GetViewport = 48,
            SetMaterial = 49,
            GetMaterial = 50,
            SetLight = 51,
            GetLight = 52,
            LightEnable = 53,
            GetLightEnable = 54,
            SetClipPlane = 55,
            GetClipPlane = 56,
            SetRenderState = 57,
            GetRenderState = 58,
            CreateStateBlock = 59,
            BeginStateBlock = 60,
            EndStateBlock = 61,
            SetClipStatus = 62,
            GetClipStatus = 63,
            GetTexture = 64,
            SetTexture = 65,
            GetTextureStageState = 66,
            SetTextureStageState = 67,
            GetSamplerState = 68,
            SetSamplerState = 69,
            ValidateDevice = 70,
            SetPaletteEntries = 71,
            GetPaletteEntries = 72,
            SetCurrentTexturePalette = 73,
            GetCurrentTexturePalette = 74,
            SetScissorRect = 75,
            GetScissorRect = 76,
            SetSoftwareVertexProcessing = 77,
            GetSoftwareVertexProcessing = 78,
            SetNPatchMode = 79,
            GetNPatchMode = 80,
            DrawPrimitive = 81,
            DrawIndexedPrimitive = 82,
            DrawPrimitiveUP = 83,
            DrawIndexedPrimitiveUP = 84,
            ProcessVertices = 85,
            CreateVertexDeclaration = 86,
            SetVertexDeclaration = 87,
            GetVertexDeclaration = 88,
            SetFVF = 89,
            GetFVF = 90,
            CreateVertexShader = 91,
            SetVertexShader = 92,
            GetVertexShader = 93,
            SetVertexShaderConstantF = 94,
            GetVertexShaderConstantF = 95,
            SetVertexShaderConstantI = 96,
            GetVertexShaderConstantI = 97,
            SetVertexShaderConstantB = 98,
            GetVertexShaderConstantB = 99,
            SetStreamSource = 100,
            GetStreamSource = 101,
            SetStreamSourceFreq = 102,
            GetStreamSourceFreq = 103,
            SetIndices = 104,
            GetIndices = 105,
            CreatePixelShader = 106,
            SetPixelShader = 107,
            GetPixelShader = 108,
            SetPixelShaderConstantF = 109,
            GetPixelShaderConstantF = 110,
            SetPixelShaderConstantI = 111,
            GetPixelShaderConstantI = 112,
            SetPixelShaderConstantB = 113,
            GetPixelShaderConstantB = 114,
            DrawRectPatch = 115,
            DrawTriPatch = 116,
            DeletePatch = 117,
            CreateQuery = 118,
        }

        #endregion

        //public const uint SDKVersion2 = 32;
        //public const int BeginSceneOffset = 39;
        //public const int EndSceneOffset = 42;
        //public const int ResetOffset = 16;
        //public const int ResetExOffset = 132;




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