using System;
using System.Linq;
using System.Runtime.InteropServices; // needed for DLL import

using IceFlake.Client;
using IceFlake.Client.Objects;
using IceFlake.Client.Patchables;
using IceFlake.Client.Scripts;


namespace IceFlake.Scripts
{
    #region _QKTestWardenScript

    public class _QKTestWardenScript : Script
    {
        public _QKTestWardenScript()
            : base("QK", "_QKTestWardenScript")
        { }

        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>

        public override void OnStart()
        {
            Scan();
            return;
        }

        public override void OnTick()
        {
            //Log.WriteLine("---");
            //Scan();

        }

        public override void OnTerminate()
        {

        }

        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr WardenDelegate(IntPtr ptr, uint adress, uint len);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaExecuteBufferDelegate(string lua, string fileName, uint pState);
        private static LuaExecuteBufferDelegate LuaExecute;
        
        //private static uint baseAddress = (uint)Process.GetCurrentProcess().MainModule.BaseAddress;
        //private static uint moduleSize = (uint)Process.GetCurrentProcess().MainModule.ModuleMemorySize;

        private static uint baseAddress = (uint)Manager.Memory.Process.MainModule.BaseAddress;
        private static uint moduleSize = (uint)Manager.Memory.Process.MainModule.ModuleMemorySize;

        
        public void Scan()
        {
            LuaExecute = Manager.Memory.RegisterDelegate<LuaExecuteBufferDelegate>((IntPtr)baseAddress + 0x0242E9); //0x50229);
            SearchWarden(new byte[] { 0x74, 0x02, 0xF3, 0xA5, 0xB1, 0x03, 0x23, 0xCA });
        }

        private static void makeDetour(uint ptr)
        {
            Manager.Memory.Detours.RemoveAll();
            Manager.Memory.Detours.CreateAndApply(Manager.Memory.RegisterDelegate<WardenDelegate>((IntPtr)ptr + 0xE), new WardenDelegate(WardenCave), "WardenHook");
        }

        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>

        [DllImport("kernel32")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, uint dwAddress, int nSize, uint dwAllocationType, uint dwProtect);
        [DllImport("kernel32")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, uint dwAddress, int nSize, uint dwFreeType);
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);
        [DllImport("kernel32.dll")]
        public static extern int VirtualQuery(ref uint lpAddress, ref MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public uint BaseAddress;
            public uint AllocationBase;
            public uint AllocationProtect;
            public uint RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        #region pattern scan
        private unsafe void SearchWarden(byte[] Signature)
        {
            uint currentAddr = baseAddress;
            uint Max = 0;
            int index = 0;
            uint old;
            MEMORY_BASIC_INFORMATION mbi = new MEMORY_BASIC_INFORMATION();

            do
            {
                VirtualQuery(ref currentAddr, ref mbi, sizeof(MEMORY_BASIC_INFORMATION));

                if ((mbi.RegionSize <= 0x9000) && (mbi.State == 4096) && (mbi.Type == 131072))
                {
                    if (VirtualProtect((IntPtr)currentAddr, mbi.RegionSize, 0x40, out old))
                    {
                        if (currentAddr < Max)
                            return;
                        else
                            Max = currentAddr;

                        for (int x = (int)currentAddr; x < (currentAddr + mbi.RegionSize); x++)
                        {
                            if (*(byte*)x == Signature[index])
                                index++;
                            else
                                index = 0;

                            if (index >= Signature.Length)
                            {
                                makeDetour((uint)(x - Signature.Length + 1));
                                return;
                            }
                        }
                    }
                }
                currentAddr += mbi.RegionSize;
            } while (true);
            Log.WriteLine("---");

        }

        #endregion

        private static IntPtr WardenCave(IntPtr ptr, uint adress, uint len)
        {
            if (adress < baseAddress + moduleSize)
            {
                Log.WriteLine("pri0 TEST");

                Log.WriteLine("print('found: |cffff00000x" + (adress - baseAddress).ToString("X") + "|r, length: |cff00ff00" + len.ToString() + "b|r')", "mylua.lua", 0);
                LuaExecute("print('found: |cffff00000x" + (adress - baseAddress).ToString("X") + "|r, length: |cff00ff00" + len.ToString() + "b|r')", "mylua.lua", 0);
            }
            return (IntPtr)Manager.Memory.Detours["WardenHook"].CallOriginal(ptr, adress, len);
        }

        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>
    }

    #endregion
}