using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Wxv.Swg.Common
{
    public static class XnaExtensions
    {
        public static Vector3 ReadVector3Single(this Stream stream)
        {
            return new Vector3
            {
                X = stream.ReadSingle(),
                Y = stream.ReadSingle(),
                Z = stream.ReadSingle()
            };
        }

        public static Vector2 ReadVector2Single(this Stream stream)
        {
            return new Vector2
            {
                X = stream.ReadSingle(),
                Y = stream.ReadSingle()
            };
        }

        public static Quaternion ReadQuaternionSingle(this Stream stream)
        {
            return new Quaternion
            {
                X = stream.ReadSingle(),
                Y = stream.ReadSingle(),
                Z = stream.ReadSingle(),
                W = stream.ReadSingle()
            };
        }

        public static Color ReadColorARGBSingle(this Stream stream)
        {
            return new Color
            {
                A = (byte) (stream.ReadSingle() * 255f),
                R = (byte) (stream.ReadSingle() * 255f),
                G = (byte) (stream.ReadSingle() * 255f),
                B = (byte) (stream.ReadSingle() * 255f)
            };
        }

        public static Color ReadColorARGBByte(this Stream stream)
        {
            return new Color
            {
                A = (byte) stream.ReadByte(),
                R = (byte) stream.ReadByte(),
                G = (byte) stream.ReadByte(),
                B = (byte) stream.ReadByte(),
            };
        }
    }
}
