using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common
{
    public class InteriorLayoutFile
    {
        public class InteriorLayoutItem
        {
            public string Object { get; internal set; }
            public string Cell { get; internal set; }
            public Single[,] Matrix { get; internal set; }
            public Single[] W { get; internal set; }

            internal InteriorLayoutItem() { }

            public override string ToString()
            {
                return string.Format("Object={0}, Cell={1}", Object, Cell);
            }
        }

        public IEnumerable<InteriorLayoutItem> Items { get; internal set; }
        internal InteriorLayoutFile() { }

        public static InteriorLayoutFile Load(IFFFile iffFile)
        {
            if (iffFile == null) throw new ArgumentNullException("iffFile");

            var interiorLayoutItems = new List<InteriorLayoutItem>();
            foreach (var node in iffFile.Root.Descendents("NODE"))
            using (var stream = new MemoryStream(node.Data))
            {
                string @object = stream.ReadNullTerminatedString();
                string cell = stream.ReadNullTerminatedString();

                var matrix = new Single[3, 3];
                var w = new Single[3];
                matrix[0, 0] = stream.ReadSingle();
                matrix[0, 1] = stream.ReadSingle();
                matrix[0, 2] = stream.ReadSingle();
                w[0] = stream.ReadSingle();
                matrix[1, 0] = stream.ReadSingle();
                matrix[1, 1] = stream.ReadSingle();
                matrix[1, 2] = stream.ReadSingle();
                w[1] = stream.ReadSingle();
                matrix[2, 0] = stream.ReadSingle();
                matrix[2, 1] = stream.ReadSingle();
                matrix[2, 2] = stream.ReadSingle();
                w[2] = stream.ReadSingle();

                interiorLayoutItems.Add(new InteriorLayoutItem
                {
                    Object = @object,
                    Cell = cell,
                    Matrix = matrix,
                    W = w
                });
            }

            return new InteriorLayoutFile
            {
                Items = interiorLayoutItems
            };
        }

        public static InteriorLayoutFile Load(Stream stream)
        {
            var iffFile = IFFFile.Load(stream);
            return Load(iffFile);
        }

        public override string ToString()
        {
            return string.Format("Items.Count={0}", Items.Count());
        }
    }
}
