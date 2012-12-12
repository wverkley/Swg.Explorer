using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common
{
    public struct TextureCoord
    {
        public double U { get; set; }
        public double V { get; set; }

        public override string ToString()
        {
            return string.Format("U:{0:F3},V:{1:F3}", U, V);
        }

        public override int GetHashCode()
        {
            return U.GetHashCode() ^ V.GetHashCode();
        }

        public static bool Equals(TextureCoord a, TextureCoord b)
        {
            return a.U == b.U && a.V == b.V;
        }

        public override bool Equals(object obj)
        {
            return obj != null && Equals(this, (TextureCoord)obj);
        }

        public static TextureCoord Load(Stream stream)
        {
            return new TextureCoord
            {
                U = stream.ReadSingle(),
                V = stream.ReadSingle()
            };
        }
    }

}
