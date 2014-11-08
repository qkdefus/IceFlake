﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using IceFlake.Client.Patchables;
using IceFlake.DirectX;

namespace IceFlake.Client.Objects
{
    public class WoWLocalPlayer : WoWPlayer
    {
        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int ClickToMoveDelegate(
            IntPtr thisPointer, int clickType, ref ulong interactGuid, ref Location clickLocation, float precision);

        #endregion

        public static ClickToMoveDelegate ClickToMoveFunction;

        private static SetFacingDelegate _setFacing;

        private static IsClickMovingDelegate _isClickMoving;

        private static StopCTMDelegate _stopCTM;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate bool CanUseItemDelegate(IntPtr thisObj, IntPtr itemSparseRec, out GameError error);
        private static CanUseItemDelegate _canUseItem;

        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //private delegate int GetRuneReadyDelegate(int slot);
        //private static GetRuneReadyDelegate _getRuneReday;

        public WoWLocalPlayer(IntPtr pointer)
            : base(pointer)
        {
            if (Direct3D.FrameCount == 0)
            {
                Log.WriteLine("LocalPlayer:");
                Log.WriteLine("\tLevel {0} {1} {2}", Level, Race, Class);
                Log.WriteLine("\tHealth: {0}/{1} ({2}%)", Health, MaxHealth, (int)HealthPercentage);
                Log.WriteLine("\t{0}: {1}/{2} ({3}%)", "Mana", Power, MaxPower, PowerPercentage);
                Log.WriteLine("\tPosition: {0}", Location);
            }
        }

        public bool IsClickMoving
        {
            get
            {
                if (_isClickMoving == null)
                    _isClickMoving =
                        Manager.Memory.RegisterDelegate<IsClickMovingDelegate>(
                            (IntPtr)Pointers.LocalPlayer.IsClickMoving);

                return _isClickMoving(Pointer);
            }
        }

        public Location Corpse
        {
            get { return Manager.Memory.Read<Location>((IntPtr)Pointers.LocalPlayer.CorpsePosition); }
        }

        public int UnusedTalentPoints
        {
            get { return WoWScript.Execute<int>("UnitCharacterPoints(\"player\")", 0); }
        }

        public int Combopoints
        {
            get { return Manager.Memory.Read<int>((IntPtr)Pointers.LocalPlayer.ComboPoints); }
        }

        public IEnumerable<QuestLogEntry> Quests
        {
            get
            {
                for (int i = 0; i < 25; i++)
                    if (GetAbsoluteDescriptor<uint>((int)WoWPlayerFields.PLAYER_QUEST_LOG_1_1 * 0x4 + (i * 0x14)) > 0)
                        yield return GetAbsoluteDescriptor<QuestLogEntry>((int)WoWPlayerFields.PLAYER_QUEST_LOG_1_1 * 0x4 + (i * 0x14));
            }
        }

        public IEnumerable<WoWUnit> Totems
        {
            get
            {
                return Manager.ObjectManager.Objects
                    .Where(obj => obj.IsValid && obj.IsUnit)
                    .OfType<WoWUnit>()
                    .Where(unit => unit.IsTotem && unit.CreatedBy == Guid);
                //.OfType<WoWTotem>();
            }
        }

        #region Items

        public IEnumerable<WoWItem> Items
        {
            get
            {
                return Manager.ObjectManager.Objects
                    .Where(obj => obj.IsValid && obj.IsItem)
                    .OfType<WoWItem>()
                    .Where(item => item.OwnerGuid == this.Guid);
            }
        }

        public IEnumerable<WoWItem> BackpackItems
        {
            get { return Enumerable.Range(0, 16).Select(i => GetBackpackItem(i)).Where(item => item != null && item.IsValid); }
        }

        public IEnumerable<WoWItem> InventoryItems
        {
            get { return BackpackItems.Concat(InventoryContainers.SelectMany(x => x.Items)); }
        }

        public IEnumerable<WoWItem> EquippedItems
        {
            get { return Enumerable.Range((int)EquipSlot.Start, (int)EquipSlot.End + 1).Select(eq => GetEquippedItem(eq)).Where(item => item != null && item.IsValid); }
        }

        #endregion

        #region Containers

        public IEnumerable<WoWContainer> InventoryContainers
        {
            get
            {
                return
                    Enumerable.Range((int)BagSlot.Bag1, (int)BagSlot.Bag4 + 1).Select(bs => WoWContainer.GetBagByIndex(bs))
                        .Where(container => container.IsValid);
            }
        }

        public IEnumerable<WoWContainer> BankContainers
        {
            get
            {
                return
                    Enumerable.Range((int)BagSlot.Bank1, (int)BagSlot.Bank7 + 1).Select(
                        bs => WoWContainer.GetBagByIndex(bs)).Where(container => container != null && container.IsValid);
            }
        }

        public IEnumerable<WoWContainer> AllContainers
        {
            get
            {
                return
                    Enumerable.Range((int)BagSlot.Bag1, (int)BagSlot.Bank7 + 1).Select(
                        bs => WoWContainer.GetBagByIndex(bs)).Where(container => container != null && container.IsValid);
            }
        }

        #endregion

        #region Item helpers

        public WoWItem GetBackpackItem(int slot)
        {
            var guid = GetAbsoluteDescriptor<ulong>((int)WoWPlayerFields.PLAYER_FIELD_PACK_SLOT_1 * 4 + (slot * 8));
            return Manager.ObjectManager.GetObjectByGuid(guid) as WoWItem;
        }

        public WoWItem GetEquippedItem(EquipSlot slot)
        {
            return GetEquippedItem((int)slot);
        }

        public WoWItem GetEquippedItem(int slot)
        {
            var guid = GetAbsoluteDescriptor<ulong>((int)WoWPlayerFields.PLAYER_FIELD_INV_SLOT_HEAD * 4 + (slot * 8));
            return Manager.ObjectManager.GetObjectByGuid(guid) as WoWItem;
        }

        public bool CanUseItem(WoWItem item, out GameError error)
        {
            return CanUseItem(WoWItem.GetItemRecordPointerFromId(item.Entry), out error);
        }

        public bool CanUseItem(IntPtr pointer, out GameError error)
        {
            if (_canUseItem == null)
                _canUseItem = Manager.Memory.RegisterDelegate<CanUseItemDelegate>((IntPtr)Pointers.Item.CanUseItem);
            return _canUseItem(this.Pointer, pointer, out error);
        }

        #endregion

        #region Class specific

        //public bool IsRuneReady(RuneSlot slot)
        //{
        //    return _getRuneReday((int)slot) <= 0;
        //}

        // TODO: Implement this
        //public byte GetRuneCount(int index)
        //{
        //    //if (index >= 6)
        //    //{
        //    //    throw new IndexOutOfRangeException(StyxResources.This_function_only_takes_vaues_from_0_5);
        //    //}
        //    //Memory arg_23_0 = ObjectManager.Wow;
        //    //uint[] array = new uint[]
        //    //{
        //    //StyxWoW.Offsets.#Bb(5053)
        //    //};
        //    //uint num = arg_23_0.ReadRelative<uint>(array);
        //    //result = ((((ulong)num & (ulong)(1L << (index & 31))) != 0uL) ? 1 : 0);
        //}

        //public RuneType GetRuneType(int index)
        //{
        //    //RuneType result;
        //    //try
        //    //{
        //    //    if (index >= 6)
        //    //    {
        //    //        throw new IndexOutOfRangeException(StyxResources.This_function_only_takes_vaues_from_0_5);
        //    //    }
        //    //    Memory arg_27_0 = ObjectManager.Wow;
        //    //    uint[] array = new uint[]
        //    //    {
        //    //        StyxWoW.Offsets.#Bb(5052) + (uint)(4 * index)
        //    //    };
        //    //    uint num = arg_27_0.ReadRelative<uint>(array);
        //    //    result = (RuneType)num;
        //    //}
        //    //catch (Exception arg_6E_0)
        //    //{
        //    //    uint[] array;
        //    //    uint num;
        //    //    #3.#1c(arg_6E_0, num, array, this, index);
        //    //    throw;
        //    //}
        //    //return result;
        //}

        //public byte GetRuneCount(RuneType type)
        //{
        //    byte result;
        //    byte b = 0;
        //    for (int i = 0; i < 6; i++)
        //    {
        //        if (GetRuneType(i) == type)
        //            b += GetRuneCount(i);
        //    }
        //}

        //public int BloodRunesReady
        //{
        //    get
        //    {
        //        int ret = 0;
        //        if (IsRuneReady(RuneSlot.Blood1)) ret++;
        //        if (IsRuneReady(RuneSlot.Blood2)) ret++;
        //        return ret;
        //    }
        //}

        //public int FrostRunesReady
        //{
        //    get
        //    {
        //        int ret = 0;
        //        if (IsRuneReady(RuneSlot.Frost1)) ret++;
        //        if (IsRuneReady(RuneSlot.Frost2)) ret++;
        //        return ret;
        //    }
        //}

        //public int UnholyRunesReady
        //{
        //    get
        //    {
        //        int ret = 0;
        //        if (IsRuneReady(RuneSlot.Unholy1)) ret++;
        //        if (IsRuneReady(RuneSlot.Unholy2)) ret++;
        //        return ret;
        //    }
        //}

        #endregion

        #region LUA Helpers

        public void StartAttack()
        {
            WoWScript.Execute("StartAttack()");
        }

        #region Movement

        public void Ascend()
        {
            WoWScript.ExecuteNoResults("JumpOrAscendStart()");
        }

        public void Jump()
        {
            WoWScript.ExecuteNoResults("JumpOrAscendStart()");
        }

        public void Descend()
        {
            WoWScript.ExecuteNoResults("SitStandOrDescendStart()");
        }

        public void MoveBackward()
        {
            WoWScript.ExecuteNoResults("MoveBackwardStart()");
        }

        public void MoveForward()
        {
            WoWScript.ExecuteNoResults("MoveForwardStart()");
        }

        public void StopMoving()
        {
            WoWScript.ExecuteNoResults(
                "AscendStop() DescendStop() MoveBackwardStop() MoveForwardStop() StrafeLeftStop() StrafeRightStop()");
        }

        public void StrafeLeft()
        {
            WoWScript.ExecuteNoResults("StrafeLeftStart()");
        }

        public void StrafeRight()
        {
            WoWScript.ExecuteNoResults("StrafeRightStart()");
        }

        public void Dismount()
        {
            WoWScript.ExecuteNoResults("Dismount()");
        }

        #endregion

        #endregion

        #region Click To Move

        public void ClickToMove(Location target, ClickToMoveType type = ClickToMoveType.Move, ulong guid = 0, float precision = 0.1f)
        {
            if (ClickToMoveFunction == null)
                ClickToMoveFunction =
                    Manager.Memory.RegisterDelegate<ClickToMoveDelegate>((IntPtr)Pointers.LocalPlayer.ClickToMove);

            Helper.ResetHardwareAction();
            ClickToMoveFunction(Pointer, (int)type, ref guid, ref target, 0.1f);
        }

        public void StopCTM()
        {
            if (_stopCTM == null)
                _stopCTM = Manager.Memory.RegisterDelegate<StopCTMDelegate>((IntPtr)Pointers.LocalPlayer.StopCTM);

            _stopCTM(Pointer);
        }

        /*/
        public void CtmTo(Location target)
        {
            //Injection.InjectionManager.ClickToMove(X, Y, Z, 4, 0, 14); // loaction // action // guid // precision
            //QK.Core.Injection.InjectionManager.UpdateLastHardwareAction();

            if (ClickToMoveFunction == null)
                ClickToMoveFunction =
                    Manager.Memory.RegisterDelegate<ClickToMoveDelegate>((IntPtr)Pointers.LocalPlayer.ClickToMove);

            Helper.ResetHardwareAction();
            ClickToMoveFunction(Pointer, (int)4, 0, ref target, 0.1f);
        }

        public void CtmToAttackRange(float X, float Y, float Z)
        {
            Injection.InjectionManager.ClickToMove(X, Y, Z, 0xB, ObjectManager.ObjectManager.Target.GUID, 14);
            QK.Core.Injection.InjectionManager.UpdateLastHardwareAction();
        }

        public void CtmLoot(float X, float Y, float Z, UInt64 GUID)
        {
            Injection.InjectionManager.ClickToMove(X, Y, Z, 0x6, GUID, 14);
            QK.Core.Injection.InjectionManager.UpdateLastHardwareAction();
        }

        public void CtmSkin(float X, float Y, float Z, UInt64 GUID)
        {
            Injection.InjectionManager.ClickToMove(X, Y, Z, 0x9, GUID, 14);
            QK.Core.Injection.InjectionManager.UpdateLastHardwareAction();
        }

        public void CtmInteractObject(float X, float Y, float Z, UInt64 GUID)
        {
            Injection.InjectionManager.ClickToMove(X, Y, Z, 0x7, GUID, 14);
            QK.Core.Injection.InjectionManager.UpdateLastHardwareAction();
        }

        public void CtmInteractNpc(float X, float Y, float Z, UInt64 GUID)
        {
            Injection.InjectionManager.ClickToMove(X, Y, Z, 0x5, GUID, 14);
            QK.Core.Injection.InjectionManager.UpdateLastHardwareAction();
        }

        public void CtmFaceDestination(float X, float Y, float Z)
        {
            Injection.InjectionManager.ClickToMove(X, Y, Z, 0x2, 0, 14);
            QK.Core.Injection.InjectionManager.UpdateLastHardwareAction();
        }

        public void CtmFaceTarget(float X, float Y, float Z, UInt64 GUID)
        {
            Injection.InjectionManager.ClickToMove(X, Y, Z, 0x1, GUID, 14);
            QK.Core.Injection.InjectionManager.UpdateLastHardwareAction();
        }

        public void CtmStop()
        {
            Injection.InjectionManager.ClickToMove(ObjectManager.ObjectManager.Me.X, ObjectManager.ObjectManager.Me.Y, ObjectManager.ObjectManager.Me.Z, 4, 0, 14);
            QK.Core.Injection.InjectionManager.UpdateLastHardwareAction();
        }
        /*/
        #endregion

        public void FaceCtm(WoWUnit obj)
        {
            float num2 = ((float)Math.Atan2((double)(obj.Location.Y - Manager.LocalPlayer.Location.Y), (double)(obj.Location.X - Manager.LocalPlayer.Location.X))) - Manager.LocalPlayer.R;
            if (num2 < 0f)
            {
                num2 = -num2;
            }
            if (num2 > 0.1f)
            {
                ClickToMove(obj.Location, ClickToMoveType.FaceTarget, obj.Guid, 0.5f);
            }
        }

















        public void LookAt(Location loc)
        {
            Location local = Location;
            var diffVector = new Location(loc.X - local.X, loc.Y - local.Y, loc.Z - local.Z);
            SetFacing(diffVector.Angle);
        }

        public void SetFacing(float angle)
        {
            //if (_setFacing == null)
            //    _setFacing = Manager.Memory.RegisterDelegate<SetFacingDelegate>((IntPtr)Pointers.LocalPlayer.SetFacing);

            //float pitch = (Manager.LocalPlayer.P + 5f);

            if (_setFacing == null)
                _setFacing = Manager.Memory.RegisterDelegate<SetFacingDelegate>((IntPtr)Pointers.LocalPlayer.SetFacing);

            const float pi2 = (float)(Math.PI * 2);
            if (angle < 0.0f)
                angle += pi2;
            if (angle > pi2)
                angle -= pi2;
            //_setFacing(Pointer, Helper.PerformanceCount, angle);
            //_setFacing(Manager.Memory.Read<IntPtr>(Pointer + 216), angle);
            _setFacing(Pointer, Helper.PerformanceCount, angle);
        }

        #region Nested type: IsClickMovingDelegate

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate bool IsClickMovingDelegate(IntPtr thisObj);

        #endregion

        #region Nested type: SetFacingDelegate

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void SetFacingDelegate(IntPtr thisObj, uint time, float facing);
        //private delegate void SetFacingDelegate(IntPtr thisObj, float pitch, float facing);


        //private delegate void SetFacingDelegate(IntPtr thisObj, float facing);


        #endregion

        #region Nested type: StopCTMDelegate

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void StopCTMDelegate(IntPtr thisObj);

        #endregion
    }
}