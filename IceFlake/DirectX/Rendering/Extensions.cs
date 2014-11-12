using SlimDX;
using SlimDX.Direct3D9;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IceFlake.DirectX
{
    internal static class Extensions
    {
        public static Vector3 WorldToScreen(this Vector3 v, Viewport viewport, Matrix worldViewProjection, out bool onScreen)
        {
            Vector3 vector = Vector3.Project(v, (float)viewport.X, (float)viewport.Y, (float)viewport.Width, (float)viewport.Height, viewport.MinZ, viewport.MaxZ, worldViewProjection);
            onScreen = vector.Z < viewport.MaxZ;
            return vector;
        }

        public static Vector4 WorldToScreen(this Vector4 v, Viewport viewport, Matrix worldViewProjection, out bool onScreen)
        {
            Vector3 vector = new Vector3(v.X, v.Y, v.Z);
            Vector3 vector2 = Vector3.Project(vector, (float)viewport.X, (float)viewport.Y, (float)viewport.Width, (float)viewport.Height, viewport.MinZ, viewport.MaxZ, worldViewProjection);
            onScreen = vector2.Z < viewport.MaxZ;
            return new Vector4(vector2.X, vector2.Y, vector2.Z, v.W);
        }
    }
}

