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
                W = stream.ReadSingle(),
                X = stream.ReadSingle(),
                Y = stream.ReadSingle(),
                Z = stream.ReadSingle(),
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

        public static string ToFormatString(this float f)
        {
            return string.Format("{0:0.######}", f).PadRight(9);
        }

        public static string ToFormatString(this Vector3 v)
        {
            return string.Format("{0} {1} {2}", v.X.ToFormatString(), v.Y.ToFormatString(), v.Z.ToFormatString());
        }

        public static string ToFormatString(this Vector2 v)
        {
            return string.Format("{0} {1}", v.X.ToFormatString(), v.Y.ToFormatString());
        }

        public static string ToFormatString(this Quaternion q)
        {
            return string.Format("{0} {1} {2} {3}", q.X.ToFormatString(), q.Y.ToFormatString(), q.Z.ToFormatString(), q.W.ToFormatString());
        }
    }
}
