using System;
using IceFlake.Client;
using IceFlake.Client.Objects;
using IceFlake.Client.Patchables;
using IceFlake.Client.Scripts;
using System.Linq;

namespace IceFlake.Scripts
{
    #region SpeedHackScript

    public class _QKPlayerTitleScript : Script
    {
        public _QKPlayerTitleScript()
            : base("QK", "_QKPlayerTitleScript")
        { }

        /// <summary>
        /// write to player title 0-90
        /// </summary>

        private readonly IntPtr _PlayerStatic = new IntPtr(0xCD87A8);
        private readonly IntPtr _PlayerTitle = new IntPtr(0x504);
        private readonly IntPtr _pFlag1 = new IntPtr(0x34);
        private readonly IntPtr _pFlag2 = new IntPtr(0x24);
        private readonly IntPtr _pFlag3 = new IntPtr(0x8);

        public override void OnStart()
        {
            uint _pPointer1 = Manager.Memory.Read<uint>(new IntPtr((uint)_PlayerStatic));
            uint _pPointer2 = Manager.Memory.Read<uint>(new IntPtr((uint)_pPointer1 + (uint)_pFlag1));
            uint _pBase1 = Manager.Memory.Read<uint>(new IntPtr((uint)_pPointer2 + (uint)_pFlag2));
            uint _pBase2 = Manager.Memory.Read<uint>(new IntPtr((uint)_pBase1 + (uint)_pFlag3));
            uint _pTitle = Manager.Memory.Read<uint>(new IntPtr((uint)_pBase2 + (uint)_PlayerTitle));
            Log.WriteLine("---" + _pTitle);

            return;
        }

        public override void OnTick()
        {

        }

        public override void OnTerminate()
        {

        }
    }

    #endregion
}