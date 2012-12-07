using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public class StringFileReader : SWGFileReader<StringFile>
    {
        public override StringFile Load(Stream stream)
        {
            var header = stream.ReadInt32();
            var byteFlag = (byte) stream.ReadByte();
            var nextIndex = stream.ReadInt32();
            var count = stream.ReadInt32();

            var items = new StringFile.StringFileItem[count];
            for (var i = 0; i < count; i++)
            {
                var id = stream.ReadInt32();
                stream.ReadInt32(); // ?
                var length = stream.ReadInt32() * 2;
                var data = stream.ReadBytes(length);
                var value = Encoding.ASCII.GetString(Encoding.Convert(Encoding.Unicode, Encoding.ASCII, data));

                items[i] = new StringFile.StringFileItem
                {
                    Id = id,
                    Value = value
                };
            }
            for (var i = 0; i < count; i++)
            {
                var id = stream.ReadInt32();
                var length = stream.ReadInt32();
                var name = Encoding.ASCII.GetString(stream.ReadBytes(length));
                var item = items.FirstOrDefault(i0 => i0.Id == id);
                if (item != null)
                    items[i].Name = name;
            }

            return new StringFile
            {
                Header = header,
                ByteFlag = byteFlag,
                Items = items
            };
        }
    }
}
