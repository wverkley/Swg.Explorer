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

        public static string ToFormatString(this float f, int pad = 0)
        {
            var result = string.Format("{0:0.######}", f);
            if (pad > 0) result = result.PadRight(pad);
            return result;
        }

        public static string ToFormatString(this byte b, int pad = 0)
        {
            var result = string.Format("{0:0.######}", b);
            if (pad > 0) result = result.PadRight(pad);
            return result;
        }

        public static string ToFormatString(this Vector3 v, int pad = 0)
        {
            var result = string.Format("{0} {1} {2}", v.X.ToFormatString(pad), v.Y.ToFormatString(pad), v.Z.ToFormatString(pad));
            if (pad > 0) result = result.PadRight(pad);
            return result;
        }

        public static string ToFormatString(this Vector2 v, int pad = 0)
        {
            var result = string.Format("{0} {1}", v.X.ToFormatString(pad), v.Y.ToFormatString(pad));
            if (pad > 0) result = result.PadRight(pad);
            return result;
        }

        public static string ToFormatString(this Quaternion q, int pad = 0)
        {
            var result = string.Format("{0} {1} {2} {3}", q.X.ToFormatString(pad), q.Y.ToFormatString(pad), q.Z.ToFormatString(pad), q.W.ToFormatString(pad));
            if (pad > 0) result = result.PadRight(pad);
            return result;
        }

        public static string ToFormatString(this Color c, int pad = 0)
        {
            var result = string.Format("{0} {1} {2} {3}", c.A.ToFormatString(pad), c.R.ToFormatString(pad), c.G.ToFormatString(pad), c.B.ToFormatString(pad));
            if (pad > 0) result = result.PadRight(pad);
            return result;
        }
    }
}
