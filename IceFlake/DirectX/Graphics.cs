
// #if SLIMDX
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;

namespace IceFlake.DirectX
{
    // TODO: Implement
    public class Graphics
    {
        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>

        public Graphics()
        {
            gPreparedFrame = false;
        }

        private bool gPreparedFrame
        {
            get;
            set;
        }

        public void Render(params IResource[] resources)
        {
            if (!gPreparedFrame)
            {
                // Add rendering
                gPreparedFrame = true;
            }
        }

        //public static SlimDX.Direct3D9.StateBlock StateBlock = new SlimDX.Direct3D9.StateBlock(Device device, StateBlockType.All);

        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>

        /// <summary>
        /// Creates a solid color Brush object based on a hex color code string.
        /// </summary>
        /// <param name="sHex">The hex color to use.</param>
        /// <param name="Alpha">The alpha color value.</param>
        public static System.Drawing.Color HexColor(string sHex, int Alpha = 255)
        {
            sHex = sHex.ToLower().Trim();
            if (sHex.StartsWith("0x"))
                sHex = sHex.Substring(2);
            try
            {
                int red = Convert.ToInt32(sHex.Substring(0, 2), 16);
                int green = Convert.ToInt32(sHex.Substring(2, 2), 16);
                int blue = Convert.ToInt32(sHex.Substring(4, 2), 16);

                if (Alpha > 255)
                    return System.Drawing.Color.FromArgb(red, green, blue);
                else
                    return System.Drawing.Color.FromArgb(Alpha, red, green, blue);
            }
            catch { }

            return System.Drawing.Color.Black;
        }

        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>

        //private void SetTarget(Vector3 target, float yaw = 0, float pitch = 0, float roll = 0)
        //{
        //    var worldMatrix = Matrix.Translation(target) * Matrix.RotationYawPitchRoll(yaw, pitch, roll);
        //    D3D.Device.SetTransform(TransformState.World, worldMatrix);
        //}

        /// <summary>
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// ///////////////////////////////
        /// </summary>
    }

}
// #endif
