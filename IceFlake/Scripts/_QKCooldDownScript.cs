using System;
using System.Linq;
using System.Runtime.InteropServices; // needed for DLL import

using IceFlake.Client;
using IceFlake.Client.Objects;
using IceFlake.Client.Patchables;
using IceFlake.Client.Scripts;

namespace IceFlake.Scripts
{
    #region _QKCoolDownScript

    public class _QKCoolDownScript : Script
    {
        public _QKCoolDownScript()
            : base("QK", "_QKCoolDownScript")
        { }

        public override void OnStart()
        {

            return;
        }

        public override void OnTick()
        {
            Log.WriteLine("res " + GlobalCooldown);
            Log.WriteLine("---");
        }

        public override void OnTerminate()
        {

        }

        /// <summary>
        /// Returns true if the global cooldown is currently running.
        /// </summary>
        /// 

        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private readonly IntPtr SpellCooldownPtr = new IntPtr(0x00D3F5AC);
        private readonly IntPtr _GCD = new IntPtr(0xD3F5AC);

        public bool GlobalCooldown
        {
            get
            {
                long frequency;
                long perfCount;
                QueryPerformanceFrequency(out frequency);
                QueryPerformanceCounter(out perfCount);

                //Current time in ms
                long currentTime = (perfCount * 1000) / frequency;

                //Get first list object
                //var currentListObject = Memory.BlackMagic.ReadUInt((uint)Memory.BaseAddress + Pointers.Globals.SpellCooldownPtr + 0x8);
                var currentListObject = Manager.Memory.Read<uint>(new IntPtr((uint)SpellCooldownPtr + 0x8));

                while ((currentListObject != 0) && ((currentListObject & 1) == 0))
                {
                    //Start time of the spell cooldown in ms
                    //var startTime = Memory.BlackMagic.ReadUInt(currentListObject + 0x10);
                    var startTime = Manager.Memory.Read<uint>(new IntPtr((uint)currentListObject + 0x10));

                    //Absolute gcd of the spell in ms
                    //var globalCooldown = Memory.BlackMagic.ReadUInt(currentListObject + 0x2C);
                    var globalCooldown = Manager.Memory.Read<uint>(new IntPtr((uint)currentListObject + 0x2C));

                    ////Spell on gcd?
                    //if ((startTime + globalCooldown) > currentTime)
                    //    return true;

                    //Spell on gcd?
                    if ((startTime + globalCooldown) > currentTime)
                    {
                        // cooldown reset hack // not really working on fixed servers..
                        Manager.Memory.Write<uint>(new IntPtr((uint)currentListObject + 0x10), startTime - 1500);
                        return true;
                    }
                    
                    //Get next list object
                    //currentListObject = Memory.BlackMagic.ReadUInt(currentListObject + 4);
                    currentListObject = Manager.Memory.Read<uint>(new IntPtr((uint)currentListObject + 0x4));
                }

                return false;
            }
        }
    }

    #endregion
}