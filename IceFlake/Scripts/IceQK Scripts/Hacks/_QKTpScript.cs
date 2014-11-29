using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using IceFlake.Client;
using IceFlake.Client.Objects;
using IceFlake.Client.Patchables;
using IceFlake.Client.Scripts;
using System.Linq;

namespace IceFlake.Scripts
{
    #region QKTpScript // qk

    public class QKTpScript: Script
    {
        public QKTpScript()
            : base("_QKTpScript", "Hack")
        { }

        public override void OnStart()
        {
            if (!Manager.ObjectManager.IsInGame)
                return;






            /*/ WoWPluss->
            WoWPlayer *pPlayer = ObjectManager::GetActivePlayer();
		    if( HasRestrictedFlag( pPlayer->GetMovementFlags() ) )
			throw std::exception( "Player has restricted movementflag for teleport!" );

            DWORD_PTR dwMovementData = pPlayer->GetMovementData();
            Delegates::CMovement__SetTransportAndSend_909( dwMovementData, GetAvailableTransport()->GetGUID(), 0xFF, 1 );
            Delegates::CMovementData_C__ForceSetTransportInt( dwMovementData, 0, 0xFF, 1, 0 );
            pPlayer->SetLocation( loc );
            SendMoveHeartbeat( dwMovementData, Delegates::OsGetAsyncTimeMs() );
            /*/

            WoWGameObject obj = Manager.ObjectManager.Objects.Where(b => b.IsValid && b.IsGameObject)
                    .Select(b => b as WoWGameObject)
                    .Where(b => b.IsTransport)
                    .FirstOrDefault();
            {
                Print("Teleport Transport : {0}", obj.Name);
            }

            var dwMovementData = Manager.LocalPlayer.MovementField;

            var location = Manager.LocalPlayer.Location;

            // pPlayer->GetMovementData(), 0, 255, 0, 0 );
            //int __thiscall CMovement__SetTransportAndSend_909(void *this, int a2, int a3, int a4, int a5)
            _CMovement__SetTransportAndSend_909 = Manager.Memory.RegisterDelegate<delegate_CMovement__SetTransportAndSend_909>((IntPtr)offset_CMovement__SetTransportAndSend_909);
            _CMovement__SetTransportAndSend_909((uint)dwMovementData, (uint)obj.Guid, 255, 1);


            //pPlayer->GetMovementData(), pTarget->GetGUID(), 255, 1 );
            //signed int __thiscall CMovement__ForceSetTransportInt(int this, WGUID a2, int a3, int a4)
            _CMovement__ForceSetTransportInt = Manager.Memory.RegisterDelegate<delegate_CMovement__ForceSetTransportInt>((IntPtr)offset_CMovement__ForceSetTransportInt);
            _CMovement__ForceSetTransportInt((uint)dwMovementData, 0, 255, 1, 0);
            //Log.WriteLine("-OnStart()-");



            location.Z = location.Z + 20f;
            Manager.LocalPlayer.SetLocation(location);


        }

        public override void OnTick()
        {
            //Log.WriteLine("-OnTick()-");

        }

        public override void OnTerminate()
        {
            //Log.WriteLine("-OnTerminate()-");
        }


        // DWORD_PTR _this, UINT64 uTransportGUID, UINT unk1, UINT unk2, UINT unk3 
        // pPlayer->GetMovementData(), 0, 255, 0, 0 );
        //int __thiscall CMovement__SetTransportAndSend_909(void *this, int a2, int a3, int a4, int a5)
        public static uint offset_CMovement__SetTransportAndSend_909 = 0x6F0C70;
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int delegate_CMovement__SetTransportAndSend_909(uint MovementData, uint GUID, int a3, int a4);
        private delegate_CMovement__SetTransportAndSend_909 _CMovement__SetTransportAndSend_909;

        //pPlayer->GetMovementData(), pTarget->GetGUID(), 255, 1 );
        //signed int __thiscall CMovement__ForceSetTransportInt(int this, WGUID a2, int a3, int a4)
        public static uint offset_CMovement__ForceSetTransportInt = 0x6EC400;
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int delegate_CMovement__ForceSetTransportInt(uint MovementData, uint GUID, int a3, int a4, int a5);
        private delegate_CMovement__ForceSetTransportInt _CMovement__ForceSetTransportInt;




      



    }

















    #endregion

}