using System.Drawing;
using System.Linq;
using IceFlake.Client;
using IceFlake.Client.Objects;
using IceFlake.Client.Scripts;
using IceFlake.DirectX;
//using IceFlake.D3D;
using SlimDX;

namespace IceFlake.Scripts
{
    public class QKFailHPScript : Script
    {
        public QKFailHPScript()
            : base("Drawing", "_QKFailHPScript")
        {
        }

        public override void OnStart()
        {
            if (!Manager.ObjectManager.IsInGame)
                return;

            Log.WriteLine("MAXHP " + Manager.LocalPlayer.ObjEnd);

            Log.WriteLine("LocalPlayer.Pointer " + Manager.LocalPlayer.Pointer);
            Log.WriteLine("LocalPlayer.Pointer HeX " + ConvertToHexString((uint)Manager.LocalPlayer.Pointer));


        }

        public override void OnTick()
        {
            if (!Manager.ObjectManager.IsInGame)
                return;




        }

        public override void OnTerminate()
        {
        }








        public static string ConvertToHexString(uint value)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder("0x");
            builder.Append(System.Convert.ToString(value, 16).ToUpper());
            return builder.ToString();
        }
    }
}
