using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public class InteriorLayoutFileReader : SWGFileReader<InteriorLayoutFile>
    {
        private InteriorLayoutFile Load(IFFFile iffFile)
        {
            if (iffFile == null) throw new ArgumentNullException("iffFile");

            var interiorLayoutItems = new List<InteriorLayoutFile.InteriorLayoutItem>();
            foreach (var node in iffFile.Root.Descendents("NODE"))
            using (var stream = new MemoryStream(node.Data))
            {
                string @object = stream.ReadString();
                string cell = stream.ReadString();

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

                interiorLayoutItems.Add(new InteriorLayoutFile.InteriorLayoutItem
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

        public override InteriorLayoutFile Load(Stream stream)
        {
            var iffFile = new IFFFileReader().Load(stream);
            return Load(iffFile);
        }
    }
}
