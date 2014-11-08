﻿using IceFlake.Client;
using IceFlake.Client.Objects;
using IceFlake.Client.Scripts;
using SlimDX;
using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using D3D = IceFlake.DirectX.Direct3D;

namespace IceFlake.Scripts
{
    #region DrawUnitsScript

    public class DrawUnitsScript : Script
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PositionColored
        {
            public static readonly VertexFormat FVF = VertexFormat.Position | VertexFormat.Diffuse;
            public static readonly int Stride = Vector3.SizeInBytes + sizeof(int);

            public Vector3 Position;
            public int Color;

            public PositionColored(Vector3 pos, int col)
            {
                Position = pos;
                Color = col;
            }
        }

        public DrawUnitsScript() 
            : base("QK", "_QKDrawUnits")
        {
            colorGreen = Color.FromArgb(0x8f, 0, 0xff, 0);
            colorRed = Color.FromArgb(0x8f, 0xff, 0, 0);
            colorBlue = Color.FromArgb(0x8f, 0, 0, 0xff);
        }

        private Color colorGreen;
        private Color colorRed;
        private Color colorBlue;

        public override void OnStart()
        {
            if (!Manager.ObjectManager.IsInGame)
            {
                Stop();
                return;
            }
        }

        public override void OnTick()
        {
            foreach (var u in Manager.ObjectManager.Objects.Where(x => x.IsValid && x.IsUnit).OfType<WoWUnit>())
            {
                if (u == null || !u.IsValid)
                    continue;

                var color = (!(u.IsFriendly || u.IsNeutral) ? colorRed : colorGreen);
                DrawCircle(u.Location, 3f, color, color);
                DrawLine(Manager.LocalPlayer.Location.ToVector3(), u.Location.ToVector3(), colorBlue);
            }
        }

        private void DrawCircle(Location loc, float radius, Color innerColor, Color outerColor, int complexity = 24, bool isFilled = true)
        {
            var vertices = new List<PositionColored>();
            if (isFilled)
                vertices.Add(new PositionColored(Vector3.Zero, innerColor.ToArgb()));

            double stepAngle = (Math.PI * 2) / complexity;
            for (int i = 0; i <= complexity; i++)
            {
                double angle = (Math.PI * 2) - (i * stepAngle);
                float x = (float)(radius * Math.Cos(angle));
                float y = (float)(-radius * Math.Sin(angle));
                vertices.Add(new PositionColored(new Vector3(x, y, 0), outerColor.ToArgb()));
            }

            var buffer = vertices.ToArray();

            SetTarget(loc.ToVector3() + new Vector3(0, 0, 0.3f));

            D3D.Device.DrawUserPrimitives(PrimitiveType.TriangleFan, buffer.Length - 2, buffer);
        }

        private void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            var vertices = new List<PositionColored>();

            vertices.Add(new PositionColored(from, color.ToArgb()));
            vertices.Add(new PositionColored(to, color.ToArgb()));

            var buffer = vertices.ToArray();

            SetTarget(Vector3.Zero);

            D3D.Device.DrawUserPrimitives(PrimitiveType.LineStrip, vertices.Count - 1, buffer);
        }

        private void SetTarget(Vector3 target, float yaw = 0, float pitch = 0, float roll = 0)
        {
            var worldMatrix = Matrix.Translation(target) * Matrix.RotationYawPitchRoll(yaw, pitch, roll);
            D3D.Device.SetTransform(TransformState.World, worldMatrix);
        }


    }

    #endregion
}
